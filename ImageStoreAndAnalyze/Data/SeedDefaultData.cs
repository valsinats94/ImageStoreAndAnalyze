using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageStoreAndAnalyze.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ImageStoreAndAnalyze.Data
{
    public class SeedDefaultData
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;

        public SeedDefaultData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureSeedDataAsync()
        {
            if (await _userManager.FindByEmailAsync("someone@someone.com") == null)
            {
                ApplicationUser administrator = new ApplicationUser()
                {
                    UserName = "someone@someone.com",
                    Email = "someone@someone.com"
                };

                await _userManager.CreateAsync(administrator, "Passw0rd123!");
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));

                IdentityResult result = await _userManager.AddToRoleAsync(administrator, "Administrator");
            }
        }

        private void CreateRoles()
        {
            

        }
    }
}
