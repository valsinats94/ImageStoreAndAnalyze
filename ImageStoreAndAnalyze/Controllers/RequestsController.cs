using ImageStoreAndAnalyze.Interfaces;
using ImageStoreAndAnalyze.Interfaces.Services;
using ImageStoreAndAnalyze.Models;
using ImageStoreAndAnalyze.Models.RequestsViewModels;
using ImageStoreAndAnalyze.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Controllers
{
    [Route("[controller]/[action]")]
    public class RequestsController : Controller
    {
        private readonly ILogger logger;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IServiceProvider serviceProvider;

        public RequestsController
(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IServiceProvider serviceProvider)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        [TempData]
        public string StatusMessage { get; set; }

        private void AddErrors(string error)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        [HttpGet]
        public async Task<IActionResult> Requests(string returnUrl = null)
        {
            var userOnly = await userManager.GetUserAsync(User);
            if (userOnly == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            IFamilyRequestsDatabaseService familyRequestsDatabaseService = serviceProvider.GetService(typeof(IFamilyRequestsDatabaseService)) as IFamilyRequestsDatabaseService;
            ApplicationUser user = familyRequestsDatabaseService.GetUserIncludesUserFamilies(userOnly) as ApplicationUser;
            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;


            var model = new RequestsViewModel()
            {
                ApplicationUser = user,
                StatusMessage = StatusMessage,
                FamiliesUserAdministratorOf = familyDatabaseService.GetUserAdminFamiliesWithMainImageAndRequests(user),
                Families = familyDatabaseService.GetAllFamiliesAndRelatedData(),

            };
            foreach (IFamily family in model.FamiliesUserAdministratorOf)
            {
                foreach (FamilyRequest requst in familyRequestsDatabaseService.GetFamilyRequests(family)
                                                                                    .Where(fr => !fr.IsProcessed))
                {
                    if (model.FamilyRequests == null)
                        model.FamilyRequests = new List<FamilyRequest>();

                    model.FamilyRequests.Add(requst);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFamilyRequest(string familyGuid)
        {
            var userOnly = await userManager.GetUserAsync(User);
            if (userOnly == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            IFamilyRequestsDatabaseService familyRequestsDatabaseService = serviceProvider.GetService(typeof(IFamilyRequestsDatabaseService)) as IFamilyRequestsDatabaseService;
            ApplicationUser user = familyRequestsDatabaseService.GetUserIncludesUserFamilies(userOnly) as ApplicationUser;
            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;

            Guid familyIdentifier;
            if (!Guid.TryParse(familyGuid, out familyIdentifier))
            {
                AddErrors("You send request unsuccessfully");
                return RedirectToAction(nameof(Requests));
            }

            FamilyRequest familyRequest = new FamilyRequest()
            {
                RequestByUser = user,
                RequestedFamily = familyDatabaseService.GetFamilyByGuid(familyIdentifier) as Family,
                SendDate = DateTime.Now,
                ProcessResult = Utilities.ProcessResultTypes.UnProcessed,
                Guid = Guid.NewGuid()
            };

            try
            {
                familyRequestsDatabaseService.AddFamilyRequest(familyRequest);
            }
            catch
            {
                AddErrors("You send request unsuccessfully");
                return RedirectToAction(nameof(Requests));
            }
            
            StatusMessage = "You send request successfully";
            return RedirectToAction(nameof(Requests));
        }

        [HttpPost]
        public async Task<IActionResult> ProcessFamilyRequest(string requestGuid, int isApproved)
        {
            var userOnly = await userManager.GetUserAsync(User);
            if (userOnly == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            Guid familyRequestGuid;
            if (!Guid.TryParse(requestGuid, out familyRequestGuid))
            {
                StatusMessage = "You cannot process request.";
                return RedirectToAction(nameof(Requests));
            }

            IFamilyRequestsDatabaseService familyRequestsDatabaseService = serviceProvider.GetService(typeof(IFamilyRequestsDatabaseService)) as IFamilyRequestsDatabaseService;
            FamilyRequest familyRequest = familyRequestsDatabaseService.GetFamilyRequestAndFamilyByGuid(familyRequestGuid);

            familyRequest.IsProcessed = true;
            familyRequest.ProcessedByUser = userOnly;
            familyRequest.ProcessedDate = DateTime.Now;
            familyRequest.ProcessResult = isApproved == 1 ? Utilities.ProcessResultTypes.Approved : Utilities.ProcessResultTypes.Rejected;

            try
            {
                familyRequestsDatabaseService.Save();
            }
            catch
            {
                StatusMessage = "You cannot process request.";
                return RedirectToAction(nameof(Requests));
            }
            
            familyRequest.RequestedFamily.FamilyUsers.Add(new FamilyUsers()
            {
                User = familyRequest.RequestByUser,
                Family = familyRequest.RequestedFamily
            });

            try
            {
                familyRequestsDatabaseService.Save();
            }
            catch
            {
                AddErrors("Request was processed but user was not added successfully.");
                return RedirectToAction(nameof(Requests));
            }

            StatusMessage = "You approved request successfully";
            return RedirectToAction(nameof(Requests));
        }
    }
}
