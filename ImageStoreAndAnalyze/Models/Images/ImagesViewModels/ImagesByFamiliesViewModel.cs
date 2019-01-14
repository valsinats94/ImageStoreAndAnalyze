using ImageStoreAndAnalyze.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models.Images.ImagesViewModels
{
    public class ImagesByFamiliesViewModel : BaseViewModel
    {
        public ICollection<IFamily> FamiliesMemberOf { get; set; }

        public ImagesViewModel ImagesViewModel { get; set; }
    }
}
