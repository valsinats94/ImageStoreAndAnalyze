using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMImage.Models.AnalyzeModels
{
    public class ImageFromResult
    {
        public string Id { get; set; }

        public string ImageName { get; set; }

        public ImageModel OriginalImage { get; set; }
    }
}
