using ImageStoreAndAnalyze.Models;
using ImageStoreAndAnalyze.Models.Images.ConcreteModels.AnalyzeModels;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImageProcess.Models
{
    public class ImageModel : Image
    {
        #region Declarations

        private string imagePath;
        private IList<ImageTag> imageTags;

        #endregion

        #region Constructors

        public ImageModel()
        {
        }

        #endregion

        #region Properties

        [Key]
        public int ImageId { get; set; }

        public string FileName { get; set; }

        public string ImagePath
        {
            get
            {
                if (imagePath == null)
                    imagePath = string.Empty;

                return imagePath;
            }
            set
            {
                if (value == imagePath)
                    return;

                imagePath = value;
                //GetImageData(imagePath);
            }
        }

        public string ImageUrl { get; set; }

        public IList<ImageTag> ImageTags
        {
            get
            {
                if (imageTags == null)
                    imageTags = new List<ImageTag>();

                return imageTags;
            }
            set
            {
                if (value == imageTags)
                    return;

                imageTags = value;
            }
        }

        public bool IsProcessed { get; set; }

        public Family Family { get; set; }

        public Guid Guid { get; set; }

        public ApplicationUser User { get; set; }
        #endregion

        #region Methods

        //private void GetImageData(string imagePath)
        //{
        //    if (string.IsNullOrEmpty(imagePath))
        //        return;

        //    if (!File.Exists(imagePath))
        //    {
        //        DeleteImageFromDB(this);
        //        return;
        //    }

        //    //if (!ImageHelperService.IsImageFile(imagePath))
        //    //    throw new FileNotAnImageException();

        //    ImageData = File.ReadAllBytes(imagePath);
        //}

        //private void DeleteImageFromDB(ImageModel imageModel)
        //{
        //    IImageDatabaseService imageDBService = serviceProvider.GetService(typeof(IImageDatabaseService)) as IImageDatabaseService;
        //    try
        //    {
        //        imageDBService.DeleteImageFromDatabase(imageModel);
        //    }
        //    catch
        //    {
        //        return;
        //    }

        //}

        #endregion
    }
}
