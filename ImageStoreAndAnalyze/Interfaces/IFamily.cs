using ImageProcess.Models;
using ImageStoreAndAnalyze.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Interfaces
{
    public interface IFamily
    {
        string FamilyName { get; set; }
        
        ApplicationUser FamilyAdministrator { get; set; }

        ImageModel MainImage { get; set; }

        ICollection<ImageModel> Images { get; set; }

        Guid Guid { get; set; }
    }
}
