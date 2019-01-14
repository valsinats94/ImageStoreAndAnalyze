using ImageProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Interfaces.Services
{
    public interface IAnalyzeUrlService
    {
        Task RunAsyncURL(List<ImageModel> images);
    }
}
