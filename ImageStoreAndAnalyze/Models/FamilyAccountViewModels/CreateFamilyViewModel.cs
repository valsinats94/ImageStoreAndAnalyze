using ImageProcess.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models.FamilyAccountViewModels
{
    public class CreateFamilyViewModel : BaseViewModel
    {
        #region Declaration

        #endregion

        #region Properties

        [Display(Name = "Family Name")]
        public string FamilyName { get; set; }

        [Display(Name = "Family Main Image")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
        public IFormFile Image { get; set; }

        public ImageModel ImageModel { get; set; }

        #endregion
    }
}
