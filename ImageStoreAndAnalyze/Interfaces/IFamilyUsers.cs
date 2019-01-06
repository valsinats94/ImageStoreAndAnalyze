using ImageStoreAndAnalyze.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Interfaces
{
    public interface IFamilyUsers
    {
        int FamilyID { get; set; }
        //IFamily Family { get; set; }

        string ApplicationUserId { get; set; }
        //IUser User { get; set; }
    }
}
