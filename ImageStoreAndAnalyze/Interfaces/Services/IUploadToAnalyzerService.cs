using ImageProcess.Models;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Interfaces.Services
{
    public interface IUploadToAnalyzerService
    {
        ICollection<StatusResult> UploadResult { get; set; }

        Task<StatusResult> RunAsyncUpload(List<ImageModel> images);

        StatusResult ParseUploadResult(string json, List<ImageModel> originalImages);
    }
}
