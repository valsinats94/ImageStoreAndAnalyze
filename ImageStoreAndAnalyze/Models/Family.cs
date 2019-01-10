using ImageProcess.Models;
using ImageStoreAndAnalyze.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models
{
    public class Family : IFamily
    {
        private ICollection<FamilyUsers> familyUsers;

        [Key]
        public int ID { get; set; }

        [Required]
        public string FamilyName { get; set; }

        public ICollection<FamilyUsers> FamilyUsers
        {
            get
            {
                if (familyUsers == null)
                    familyUsers = new List<FamilyUsers>();

                return familyUsers;
            }

            set => familyUsers = value;
        }

        [Required]
        public ApplicationUser FamilyAdministrator { get; set; }

        public ImageModel MainImage { get; set; }
        public ICollection<ImageModel> Images { get; set; }

        public Guid Guid { get; set; }
        
    }
}
