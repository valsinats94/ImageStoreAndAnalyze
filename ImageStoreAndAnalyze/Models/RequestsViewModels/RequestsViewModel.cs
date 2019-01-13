using ImageStoreAndAnalyze.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models.RequestsViewModels
{
    public class RequestsViewModel : BaseViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }

        public IList<FamilyRequest> FamilyRequests { get; set; }

        public ICollection<FamilyRequest> NotProcessedFamilyRequests { get; set; }

        public ICollection<IFamily> FamiliesUserAdministratorOf { get; set; }

        public ICollection<Family> Families { get; set; }
    }
}
