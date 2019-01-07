using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortMImage.Models.AnalyzeModels
{
    public class ImageAnalyzeResult
    {
        #region Declarations

        private IList<ImageTag> imageTags;

        #endregion

        #region Constructors

        public ImageAnalyzeResult() { }

        #endregion

        #region Properties

        public int? TaggingId { get; set; }

        public string ImageName { get; set; }

        public IList<ImageTag> ImageTags
        {
            get
            {
                if (imageTags == null)
                    imageTags = new List<ImageTag>();

                return imageTags;
            }
            set
            {
                if (value == imageTags)
                    return;

                imageTags = value;
            }
        }

        #endregion
    }
}
