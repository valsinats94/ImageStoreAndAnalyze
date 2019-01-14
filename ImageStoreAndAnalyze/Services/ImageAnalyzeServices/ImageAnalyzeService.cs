using Newtonsoft.Json.Linq;
using SortMImage.Models.AnalyzeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Services.ImageAnalyzeServices
{
    public class ImageAnalyzeService
    {
        public static IEnumerable<ImageAnalyzeResult> ParseImageAnalyzeResults(string json)
        {
            IList<ImageAnalyzeResult> parsedResults = new List<ImageAnalyzeResult>();

            //var jsonObject = JObject.Parse(json);
            ////var jsonObject = Json.Decode(json);
            
            //foreach (var result in jsonObject)
            //{
            //    ImageAnalyzeResult imageAnalyzeResult = new ImageAnalyzeResult()
            //    {
            //        TaggingId = result.tagging_id,
            //        ImageName = result.image,
            //        ImageTags = ParseTagsArrayToAnalyzeResultTags((DynamicJsonArray)result.tags)
            //    };

            //    parsedResults.Add(imageAnalyzeResult);
            //}

            return parsedResults;
        }

        public static IList<ImageTag> ParseTagsArrayToAnalyzeResultTags(IList<JToken> tags)
        {
            IList<ImageTag> imageTags = new List<ImageTag>();

            foreach (var tag in tags)
            {
                ImageTag imageTag = tag.ToObject<ImageTag>();
                imageTags.Add(imageTag);
            }

            return imageTags;
        }
    }
}
