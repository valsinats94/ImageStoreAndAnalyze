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

        [Display(Name = "Family name")]
        public string FamilyName { get; set; }

        [Display(Name = "Family main image")]
        public byte[] Image { get; set; }

        #endregion
    }
}
