using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImageStoreAndAnalyze.Models;
using ImageProcess.Models;

namespace ImageStoreAndAnalyze.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        DbSet<Family> Families { get; set; }
        DbSet<ImageModel> Images { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FamilyUsers>()
                .HasKey(fu => new { fu.FamilyID, fu.ApplicationUserId });

            builder.Entity<FamilyUsers>()
                .HasOne(fu => fu.Family)
                .WithMany(u => u.FamilyUsers)
                .HasForeignKey(fu => fu.FamilyID);

            builder.Entity<FamilyUsers>()
                .HasOne(u => u.User)
                .WithMany(fu => fu.FamilyUsers)
                .HasForeignKey(u => u.ApplicationUserId);

            builder.Entity<Family>()
                .HasOne(a => a.FamilyAdministrator);

            builder.Entity<Family>()
                .HasOne(i => i.MainImage);
        }

        private void CreateUserRoles()
        {
            
        }
    }
}
