using ImageProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Interfaces.Services
{
    public interface IImageDatabaseService
    {
        void AddImageToDatabase(ImageModel imageModel);

        void DeleteImageFromDatabase(ImageModel imageModel);

        IImage GetFamilyByGuid(Guid guid);
    }
}
