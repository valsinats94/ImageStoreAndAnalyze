using ImageStoreAndAnalyze.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models
{
    public class FamilyUsers : IFamilyUsers
    {
        public int FamilyID { get; set; }
        public Family Family { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
