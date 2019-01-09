using ImageProcess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Utilities
{
    public class FileHelpers
    {
        public static async Task<byte[]> ProcessFormFile(IFormFile formFile, ILogger logger, string statusMessage, ModelStateDictionary modelState = null)
        {
            var fieldDisplayName = string.Empty;
            string statusMessageResult = string.Empty;

            // Use reflection to obtain the display name for the model property associated with this IFormFile. 
            MemberInfo property =
                typeof(ImageModel).GetProperty(
                    formFile.Name.Substring(formFile.Name.IndexOf(".") + 1));

            if (property != null)
            {
                var displayAttribute =
                    property.GetCustomAttribute(typeof(DisplayAttribute))
                        as DisplayAttribute;

                if (displayAttribute != null)
                {
                    fieldDisplayName = $"{displayAttribute.Name} ";
                }
            }
            
            // Check the file length and don't bother attempting to read it if the file contains no content.
            if (formFile.Length == 0)
            {
                statusMessageResult = $"The {fieldDisplayName}file ({formFile.FileName}) is empty.";
                modelState?.AddModelError(formFile.Name, statusMessageResult);

                return null;
            }
            //else if (formFile.Length > 1048576)
            //{
            //    modelState.AddModelError(formFile.Name,
            //        $"The {fieldDisplayName}file ({fileName}) exceeds 1 MB.");
            //}
            try
            {
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    var fileContents = ms.ToArray();

                    // Check the content length in case the file's only content was a BOM
                    if (fileContents.Length > 0)
                    {
                        return fileContents;
                    }
                    else
                    {
                        statusMessageResult = $"The {fieldDisplayName}file ({formFile.FileName}) is empty.";
                        modelState?.AddModelError(formFile.Name, statusMessageResult);
                    }
                }
            }
            catch (Exception ex)
            {
                statusMessageResult = $"The {fieldDisplayName}file ({formFile.FileName}) upload failed. " +
                    $"Please contact the Help Desk for support. Error: {ex.Message}";

                modelState?.AddModelError(formFile.Name, statusMessageResult);
                logger?.LogInformation(statusMessageResult);
            }

            return null;
        }
    }
}
