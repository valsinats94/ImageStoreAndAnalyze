using ImageProcess.Models;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Interfaces.Services
{
    public interface IAnalyzeMyImgService 
    {
        Task AnalyzeInnerImages(List<ImageModel> imagesForAnalyse);

        Task<List<ImageTag>> RunAsyncAnalyzeImage(string id);

        IEnumerable<ImageTag> ParseJsonResultToImageTags(string json);
    }
}
