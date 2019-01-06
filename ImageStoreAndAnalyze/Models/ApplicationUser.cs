using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ImageStoreAndAnalyze.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser, IUser
    {
        public ICollection<FamilyUsers> FamilyUsers { get; set; }

        public bool IsFamilyAdmin { get; set; }
    }
}
