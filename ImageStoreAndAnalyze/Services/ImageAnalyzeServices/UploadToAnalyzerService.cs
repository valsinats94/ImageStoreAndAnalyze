using ImageProcess.Models;
using ImageStoreAndAnalyze.Exceptions;
using ImageStoreAndAnalyze.Interfaces.Services;
using Newtonsoft.Json.Linq;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Services.ImageAnalyzeServices
{
    public class UploadToAnalyzerService : IUploadToAnalyzerService
    {
        #region Declarations

        private ObservableCollection<StatusResult> uploadResult;

        #endregion

        #region Properties

        public ICollection<StatusResult> UploadResult
        {
            get
            {
                if (uploadResult == null)
                    uploadResult = new ObservableCollection<StatusResult>();

                return uploadResult;
            }
            set
            {
                if (value == uploadResult)
                    return;

                uploadResult = value as ObservableCollection<StatusResult>;
            }
        }

        #endregion

        #region Methods

        public async Task<StatusResult> RunAsyncUpload(List<ImageModel> images)
        {
            if (images == null || images.Count == 0)
                throw new NoImagesForAnalyzingException();

            if (images.Count > 5)
                throw new MaxParallelAnalyzingImagesException();

            string apiKey = "acc_aaac6c75b4db2a4";
            string apiSecret = "df9d2861fdca6bc2d5a97aac409c6360";

            string basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", apiKey, apiSecret)));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.imagga.com/v1/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Basic {0}", basicAuthValue));

                MultipartFormDataContent uploads = new MultipartFormDataContent();
                foreach (ImageModel image in images)
                {
                    uploads.Add(new ByteArrayContent(image.ImageData), image.Name, image.ImagePath);
                }

                HttpResponseMessage uploadResponse = await client.PostAsync("content", uploads);

                HttpContent content = uploadResponse.Content;
                string result = await content.ReadAsStringAsync();

                StatusResult statusResult = ParseUploadResult(result, images);
                UploadResult.Add(statusResult);

                Console.WriteLine(result);
                
                return statusResult;
            }
        }

        public StatusResult ParseUploadResult(string json, List<ImageModel> originalImages)
        {
            var newtonJson = JObject.Parse(json);
            IList<JToken> resultsUploaded = newtonJson["uploaded"].Children().ToList();

            StatusResult statusResult = newtonJson.ToObject<StatusResult>();

            if (!statusResult.Status.ToLower().Equals("error"))
                statusResult.Images = ParseImagesFromJson(resultsUploaded, originalImages).ToList();

            return statusResult;
        }

        private IEnumerable<ImageFromResult> ParseImagesFromJson(IList<JToken> resultsUploaded, List<ImageModel> originalImages)
        {
            IList<ImageFromResult> images = new List<ImageFromResult>();

            foreach (var result in resultsUploaded)
            {
                ImageFromResult image = result.ToObject<ImageFromResult>();
                image.OriginalImage = originalImages.FirstOrDefault(img =>
                    img.Name.Split('.').FirstOrDefault() == image.ImageName.Split('.').FirstOrDefault());

                images.Add(image);
            }
            
            return images;
        }

        #endregion
    }
}
