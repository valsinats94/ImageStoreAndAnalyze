using ImageProcess.Models;
using ImageStoreAndAnalyze.Interfaces;
using ImageStoreAndAnalyze.Interfaces.Services;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public class ImageDatabaseService : BaseDatabaseService, IImageDatabaseService
    {
        public ImageDatabaseService(ApplicationDbContext context, IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
        }

        public void AddImageToDatabase(ImageModel imageModel)
        {
            if (imageModel == null)
            {
                throw new ArgumentNullException(nameof(imageModel));
            }

            context.Images.Add(imageModel);
            context.SaveChanges();
        }

        public void DeleteImageFromDatabase(ImageModel imageModel)
        {
            if (imageModel == null)
            {
                throw new ArgumentNullException(nameof(imageModel));
            }

            context.Images.Remove(imageModel);
            context.SaveChangesAsync();
        }

        public IImage GetFamilyByGuid(Guid guid)
        {
            return context.Images.FirstOrDefault(f => f.Guid == guid);
        }

        public ImageModel GetImageByName(string imageName)
        {
            return context.Images.FirstOrDefault(img => img.Name == imageName);
        }

        public ImageModel GetImageByImageData(byte[] imageData)
        {
            List<ImageModel> dbImages = GetAllImages();
            foreach (ImageModel img in dbImages)
            {
                if (img.ImageData.Count() == imageData.Count() && img.ImageData.SequenceEqual(imageData))
                    return img;

            }

            return null;
        }

        public List<ImageModel> GetAllImages()
        {
            List<ImageModel> retrievedImages = new List<ImageModel>();

            foreach (ImageModel image in context.Images)
            {
                context.Entry(image).Collection(img => img.ImageTags).Load();
                retrievedImages.Add(image);
            }

            return retrievedImages;
        }

        public void UpdateImageTagsByImageName(string imageName, List<ImageTag> imageTags)
        {
            ImageModel image = context.Images.FirstOrDefault(img => img.Name.Equals(imageName));
            if (image != null)
            {
                image.ImageTags = imageTags;
                context.SaveChanges();
            }
        }

        public void UpdateImageTagsByImageName(Guid imageGuid, List<ImageTag> imageTags)
        {
            ImageModel image = context.Images.FirstOrDefault(img => img.Guid == imageGuid);
            if (image != null)
            {
                image.ImageTags = imageTags;
                context.SaveChanges();
            }
        }

        public void UpdateImageTagsByImageData(byte[] imageData, List<ImageTag> imageTags)
        {
            ImageModel image = GetImageByImageData(imageData);
            if (image != null)
            {
                image.ImageTags = imageTags;
                context.SaveChanges();
            }
        }
    }
}
