using ImageStoreAndAnalyze.Interfaces;
using System;
using System.Collections.Generic;

namespace ImageStoreAndAnalyze.Models.FamilyAccountViewModels
{
    public class FamiliesManagementViewModel : BaseViewModel
    {
        private Guid guidToRemove;
        
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<IFamily> FamiliesAdminOf { get; set; }

        public ICollection<IFamily> FamiliesMemberOf { get; set; }

        public ICollection<Guid> FamiliesToLeave { get; set; }

        public Guid GuidToRemove
        {
            get
            {
                return guidToRemove;
            }
            set
            {
                guidToRemove = value;
                if (FamiliesToLeave == null)
                    FamiliesToLeave = new List<Guid>();

                FamiliesToLeave.Add(value);
            }
        }
    }
}
