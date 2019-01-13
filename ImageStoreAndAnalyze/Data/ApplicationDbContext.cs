using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImageStoreAndAnalyze.Models;
using ImageProcess.Models;
using SortMImage.Models.AnalyzeModels;
using ImageStoreAndAnalyze.Interfaces;

namespace ImageStoreAndAnalyze.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        public DbSet<Family> Families { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<ImageTag> ImageTags { get; set; }

        public DbSet<FamilyRequest> FamilyRequests { get; set; }

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

            builder.Entity<ImageModel>()
                .HasOne(i => i.Family)
                .WithMany(f => f.Images);

            builder.Entity<ImageModel>()
                .HasMany(im => im.ImageTags)
                .WithOne(it => it.Image);

            builder.Entity<FamilyRequest>()
                .HasOne(fr => fr.RequestByUser)
                .WithMany(u => u.FamilyRequests);

            builder.Entity<FamilyRequest>()
                .HasOne(fr => fr.ProcessedByUser);

            builder.Entity<FamilyRequest>()
                .HasOne(fr => fr.RequestedFamily)
                .WithMany(f => f.FamilyRequests);
        }

        private void CreateUserRoles()
        {
            
        }
    }
}
