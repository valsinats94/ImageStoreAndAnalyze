using ImageProcess.Models;
using ImageStoreAndAnalyze.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public class ImageDatabaseService : BaseDatabaseService, IImageDatabaseService
    {
        public ImageDatabaseService(DbContext context, IServiceProvider serviceProvider)
            :base(context, serviceProvider)
        {
        }

        public void AddImageToDatabase(ImageModel imageModel)
        {
            if (imageModel == null)
            {
                throw new ArgumentNullException(nameof(imageModel));
            }

            context.Images.Add(imageModel);
            context.SaveChangesAsync();
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
    }
}
