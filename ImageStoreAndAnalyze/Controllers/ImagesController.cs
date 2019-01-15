using ImageProcess.Models;
using ImageStoreAndAnalyze.Interfaces;
using ImageStoreAndAnalyze.Interfaces.Services;
using ImageStoreAndAnalyze.Models;
using ImageStoreAndAnalyze.Models.Images.ImagesViewModels;
using ImageStoreAndAnalyze.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ImagesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger logger;
        private readonly UrlEncoder urlEncoder;
        private readonly IServiceProvider serviceProvider;

        public ImagesController(
          UserManager<ApplicationUser> userManager,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IServiceProvider serviceProvider)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.urlEncoder = urlEncoder;
            this.serviceProvider = serviceProvider;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Images(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;
            var model = new ImagesViewModel()
            {
                StatusMessage = StatusMessage,
                FamiliesMemberOf = familyDatabaseService.GetUserFamiliesMemberOf(user)
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadImages(ImagesViewModel imagesViewModel)
        {
            if (!imagesViewModel.FamilyGuid.HasValue)
            {
                StatusMessage = "Error! Please, choose family you are member of.";
                return RedirectToAction(nameof(ImagesByFamilies));
            }

            if (imagesViewModel.UserImages == null || imagesViewModel.UserImages.Count <= 0)
            {
                StatusMessage = "Error! Please, choose images for upload.";
                return RedirectToAction(nameof(ImagesByFamilies));
            }

            Guid familyId = imagesViewModel.FamilyGuid.Value;
            var user = userManager.GetUserAsync(User);
            IFamily family = (serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService).GetFamilyByGuid(familyId);

            foreach (var image in imagesViewModel.UserImages)
            {
                var imageData = FileHelpers.ProcessFormFile(image, logger, StatusMessage);


                ImageModel imageModel = new ImageModel()
                {
                    ImageData = imageData.Result,
                    FileName = image.FileName,
                    UploadedOn = DateTime.Now,
                    Name = image.FileName,
                    Guid = Guid.NewGuid(),
                    User = user.Result,
                    Family = family as Family,
                };

                IImageDatabaseService imageDatabaseService = serviceProvider.GetService(typeof(IImageDatabaseService)) as IImageDatabaseService;
                imageDatabaseService.AddImageToDatabase(imageModel);
            }

            StatusMessage = $"You uploaded images successfully to family {family.FamilyName}";
            logger.Log(LogLevel.Information, $"User uploaded images successfully.");

            return RedirectToAction(nameof(ImagesByFamilies));
        }

        [HttpGet]
        public async Task<IActionResult> ImagesByFamilies(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;


            var model = new ImagesByFamiliesViewModel()
            {
                StatusMessage = StatusMessage,
                FamiliesMemberOf = familyDatabaseService.GetUserFamiliesMemberOfWithAdminAndImages(user)
            };

            var subModel = new ImagesViewModel()
            {
                StatusMessage = StatusMessage,
                FamiliesMemberOf = familyDatabaseService.GetUserFamiliesMemberOf(user)
            };

            model.ImagesViewModel = subModel;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeImage(string imageGuid)
        {
            IImageDatabaseService imageDatabaseService = serviceProvider.GetService(typeof(IImageDatabaseService)) as IImageDatabaseService;
            Guid imageGuidParsed;

            if (!Guid.TryParse(imageGuid, out imageGuidParsed))
            {
                StatusMessage = "Error in analyzing image. Please choose another one or try again later.";
                RedirectToAction(nameof(ImagesByFamilies));
            }

            ImageModel imageModel = imageDatabaseService.GetImageByGuid(imageGuidParsed) as ImageModel;

            try
            {
                StatusResult statusResult = UploadImages(new List<ImageModel>() { imageModel });
                AnalyzeImageWithAnalyzer(statusResult, imageDatabaseService);
            }
            catch
            {
                StatusMessage = "Error in analyzing image. Please choose another one or try again later.";
                logger.Log(LogLevel.Critical, "Exception in analyzing image. Problem with Imagga Services");
                RedirectToAction(nameof(ImagesByFamilies));
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AnalyzedImages()
        {
            IImageDatabaseService imageDatabaseService = serviceProvider.GetService(typeof(IImageDatabaseService)) as IImageDatabaseService;

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var model = new AnalyzedImagesViewModel()
            {
                AnalyzedImages = imageDatabaseService.GetProcessedImagesByUserUploaded(user)
            };

            return View(model);
        }

        #region Helpers

        private void AnalyzeImageWithAnalyzer(StatusResult statusResult, IImageDatabaseService imageDatabaseService)
        {
            IAnalyzeMyImgService analyzeImageService = serviceProvider.GetService(typeof(IAnalyzeMyImgService)) as IAnalyzeMyImgService;

            foreach (ImageFromResult image in statusResult.Images)
            {
                ImageModel tmpImg = imageDatabaseService.GetImageByImageData(image.OriginalImage.ImageData);
                if (tmpImg == null)
                {
                    tmpImg = image.OriginalImage;
                    tmpImg.ImageTags = analyzeImageService.RunAsyncAnalyzeImage(image.Id).Result;
                    tmpImg.IsProcessed = true;
                    imageDatabaseService.AddImageToDatabase(tmpImg);
                }
                else if (!tmpImg.IsProcessed || tmpImg.ImageTags == null || tmpImg.ImageTags.Count <= 0)
                {
                    tmpImg.ImageTags = analyzeImageService.RunAsyncAnalyzeImage(image.Id).Result;
                    tmpImg.IsProcessed = true;
                    imageDatabaseService.Save();
                }
            }
        }

        private StatusResult UploadImages(List<ImageModel> images)
        {
            StatusResult statusResult = new StatusResult();
            IUploadToAnalyzerService uploadToAnalyzerService = serviceProvider.GetService(typeof(IUploadToAnalyzerService)) as IUploadToAnalyzerService;
            List<string> tempPathImages = new List<string>();
            foreach (var image in images)
            {
                tempPathImages.Add(FileHelpers.SaveFileToTemp(image));
            }

            try
            {
                statusResult = uploadToAnalyzerService.RunAsyncUpload(images).Result;
            }
            finally
            {
                foreach (var image in tempPathImages)
                {
                    FileHelpers.DeleteTempFile(image);
                }
            }

            return statusResult;
        }

        #endregion
    }
}
