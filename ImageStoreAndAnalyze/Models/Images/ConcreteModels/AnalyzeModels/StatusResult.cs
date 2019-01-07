using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMImage.Models.AnalyzeModels
{
    public class StatusResult
    {
        private IList<ImageFromResult> images;
        //private IList<ImageModel> originalImages;

        public string Status { get; set; }

        public IList<ImageFromResult> Images
        {
            get
            {
                if (images == null)
                    images = new List<ImageFromResult>();

                return images;
            }
            set
            {
                if (value == images)
                    return;

                images = value;
            }
        }

        //public IList<ImageModel> OriginalIMages
        //{
        //    get
        //    {
        //        if (originalImages == null)
        //            originalImages = new List<ImageModel>();

        //        return originalImages;
        //    }
        //    set
        //    {
        //        if (value == originalImages)
        //            return;

        //        originalImages = value;
        //    }
        //}

    }
}
