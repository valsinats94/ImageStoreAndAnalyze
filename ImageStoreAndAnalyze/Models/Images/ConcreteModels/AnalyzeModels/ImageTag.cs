using ImageProcess.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SortMImage.Models.AnalyzeModels
{
    public class ImageTag
    {
        [Key]
        public int Id { get; set; }

        [JsonProperty("confidence")]
        public decimal Confidence { get; set; }

        [JsonProperty("name")]
        public string Tag { get; set; }

        public ImageModel Image { get; set; }
    }
}
