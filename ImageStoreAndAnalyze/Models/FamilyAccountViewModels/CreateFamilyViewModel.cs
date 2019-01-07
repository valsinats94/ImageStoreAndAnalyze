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
        public byte[] Image { get; set; }

        #endregion
    }
}
