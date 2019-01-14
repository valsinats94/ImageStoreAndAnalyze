using ImageProcess.Models;
using ImageStoreAndAnalyze.Exceptions;
using ImageStoreAndAnalyze.Interfaces.Services;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Services.ImageAnalyzeServices
{
    public class AnalyzeUrlService : IAnalyzeUrlService
    {
        IServiceProvider serviceProvider;

        public AnalyzeUrlService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task RunAsyncURL(List<ImageModel> images)
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

                string getContent = BuildGetContent(images);
                HttpResponseMessage response = await client.GetAsync(getContent.ToString());

                HttpContent content = response.Content;
                string result = await content.ReadAsStringAsync();

                if (result.ToLower().Contains("unsuccessful"))
                    throw new UrlNotFoundException();

                List<ImageAnalyzeResult> analyzeResults = ImageAnalyzeService.ParseImageAnalyzeResults(result).ToList();

                Console.WriteLine(result);
                Console.ReadLine();
            }
        }

        private string BuildGetContent(List<ImageModel> images)
        {
            StringBuilder getContent = new StringBuilder();
            getContent.Append("tagging?");
            getContent.Append("url=" + images.FirstOrDefault().ImageUrl);
            for (int i = 1; i < images.Count; i++)
            {
                getContent.Append("&url=" + images[i].ImageUrl);
            }

            return getContent.ToString();
        }
    }
}
