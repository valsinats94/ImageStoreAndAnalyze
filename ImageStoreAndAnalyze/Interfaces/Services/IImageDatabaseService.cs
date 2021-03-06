﻿using ImageProcess.Models;
using ImageStoreAndAnalyze.Models;
using SortMImage.Models.AnalyzeModels;
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

        IImage GetImageByGuid(Guid guid);

        ICollection<ImageModel> GetProcessedImagesByUserUploaded(IUser user);

        ImageModel GetImageByName(string imageName);

        ImageModel GetImageByImageData(byte[] imageData);

        List<ImageModel> GetAllImages();

        void UpdateImageTagsByImageName(string imageName, List<ImageTag> imageTags);

        void UpdateImageTagsByImageName(Guid imageGuid, List<ImageTag> imageTags);

        void UpdateImageTagsByImageData(byte[] imageData, List<ImageTag> imageTags);

        int Save();
    }
}
