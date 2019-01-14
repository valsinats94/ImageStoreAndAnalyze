using ImageProcess.Models;
using Newtonsoft.Json;
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

        [JsonProperty("filename")]
        public string ImageName { get; set; }

        public ImageModel OriginalImage { get; set; }
    }
}
