using System.ComponentModel.DataAnnotations;

namespace SortMImage.Models.AnalyzeModels
{
    public class ImageTag
    {
        [Key]
        public int Id { get; set; }

        public decimal Confidence { get; set; }

        public string Tag { get; set; }
    }
}
