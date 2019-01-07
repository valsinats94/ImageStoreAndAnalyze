using ImageProcess.Models;

namespace ImageStoreAndAnalyze.Models.Images.ConcreteModels.AnalyzeModels
{
    public class PhotoItem : Image
    {
        public int PhotoID { get; set; }
        public string Description { get; set; }
    }
}
