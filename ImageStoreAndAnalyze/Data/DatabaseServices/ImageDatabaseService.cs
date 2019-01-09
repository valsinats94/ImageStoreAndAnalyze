using ImageProcess.Models;
using ImageStoreAndAnalyze.Interfaces;
using ImageStoreAndAnalyze.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public class ImageDatabaseService : BaseDatabaseService, IImageDatabaseService
    {
        public ImageDatabaseService(ApplicationDbContext context, IServiceProvider serviceProvider)
            :base(context, serviceProvider)
        {
        }

    //    public ImageDatabaseService(DbContext context)
    //: base(context, null)
    //    {
    //    }

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
    }
}
