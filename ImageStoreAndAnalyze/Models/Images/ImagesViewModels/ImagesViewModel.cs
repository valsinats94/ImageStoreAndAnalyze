using ImageProcess.Models;
using ImageStoreAndAnalyze.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models.Images.ImagesViewModels
{
    public class ImagesViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Please select family you are member of.")]
        public Guid? FamilyGuid { get; set; }

        public ICollection<IFamily> FamiliesMemberOf { get; set; }

        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public ICollection<IFormFile> UserImages { get; set; }
    }
}
