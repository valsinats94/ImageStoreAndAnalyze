using ImageProcess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using ImageStoreAndAnalyze.Models.Images.ConcreteModels.AnalyzeModels;

namespace ImageStoreAndAnalyze.Services
{
    public class ImageHelperService
    {
        //public static bool IsImageFile(string fileName)
        //{
        //    string targetExtension = Path.GetExtension(fileName);
        //    if (string.IsNullOrEmpty(targetExtension))
        //        return false;
        //    else
        //        targetExtension = "*" + targetExtension.ToLowerInvariant();

        //    List<string> recognisedImageExtensions = new List<string>();

        //    foreach (ImageCodecInfo imageCodec in ImageCodecInfo.GetImageEncoders())
        //        recognisedImageExtensions.AddRange(imageCodec.FilenameExtension.ToLowerInvariant().Split(";".ToCharArray()));

        //    foreach (string extension in recognisedImageExtensions)
        //    {
        //        if (extension.Equals(targetExtension))
        //            return true;
        //    }

        //    return false;
        //}

        public static List<ImageModel> ParsePhotoItemsToImageModels(List<PhotoItem> photoItems, IServiceProvider serviceProvider)
        {
            List<ImageModel> imageModels = new List<ImageModel>();
            foreach (PhotoItem item in photoItems)
            {
                ImageModel imageModel = new ImageModel();
                imageModel.Name = item.Description;
                imageModel.FileName = item.Name;
                imageModel.UploadedOn = item.UploadedOn;
                imageModel.ImageData = item.ImageData;

                imageModels.Add(imageModel);
            }

            return imageModels;
        }

        //public static string SaveImageToDisk(ImageModel image)
        //{
        //    ImageDatabaseService imgDbService = new ImageDatabaseService();

        //    StringBuilder saveDirectory = new StringBuilder();
        //    saveDirectory.Append(Path.GetPathRoot(Environment.SystemDirectory));
        //    saveDirectory.Append(@"SortedImages\");
        //    saveDirectory.Append(@"ImagesFromService\");
        //    if (!Directory.Exists(saveDirectory.ToString()))
        //        Directory.CreateDirectory(saveDirectory.ToString());

        //    string imagePath = saveDirectory.ToString() + image.Name + ".jpg";
        //    using (System.Drawing.Image imageDrwaing = System.Drawing.Image.FromStream(new MemoryStream(image.ImageData)))
        //    {
        //        imageDrwaing.Save(imagePath, ImageFormat.Jpeg);  // Or Png
        //    }

        //    image.ImagePath = imagePath;
        //    imgDbService.UpdateImagePathByImageData(image.ImageData, image.ImagePath);

        //    return imagePath;
        //}
    }
}
