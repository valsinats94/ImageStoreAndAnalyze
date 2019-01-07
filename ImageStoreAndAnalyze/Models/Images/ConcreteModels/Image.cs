using ImageStoreAndAnalyze.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcess.Models
{
    public abstract class Image
    {
        #region Declarations

        private byte[] imageData;
        private DateTime uploadedOn;

        #endregion

        #region Properties

        public byte[] ImageData
        {
            get
            {
                if (imageData == null)
                    imageData = new byte[byte.MaxValue];

                return imageData;
            }
            set
            {
                if (value == imageData)
                    return;

                imageData = value;
            }
        }

        public string Name { get; set; }

        public DateTime UploadedOn
        {
            get
            {
                if (uploadedOn == DateTime.MinValue || uploadedOn == DateTime.MaxValue)
                    uploadedOn = DateTime.Now;

                return uploadedOn;
            }
            set
            {
                if (value == null || value == uploadedOn)
                    return;

                if (value == DateTime.MinValue || value == DateTime.MaxValue)
                    return;

                uploadedOn = value;
            }
        }

        #endregion
    }
}
