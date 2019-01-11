using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models.FamilyAccountViewModels
{
    public class RefuseFamilyAdminViewModel : BaseViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<ApplicationUser> FamilyMembers { get; set; }

        public Guid FamilyRefuseGuid { get; set; }
    }
}
