using ImageProcess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models.Images.ImagesViewModels
{
    public class AnalyzedImagesViewModel : BaseViewModel
    {
        public ICollection<ImageModel> AnalyzedImages { get; set; }
    }
}
