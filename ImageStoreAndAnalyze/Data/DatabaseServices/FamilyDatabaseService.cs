using ImageStoreAndAnalyze.Interfaces;
using ImageStoreAndAnalyze.Interfaces.Services;
using ImageStoreAndAnalyze.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public class FamilyDatabaseService : BaseDatabaseService, IFamilyDatabaseService
    {
        public FamilyDatabaseService(ApplicationDbContext context, IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
        }

        public void AddFamily(IFamily family)
        {
            if (family == null)
            {
                throw new ArgumentNullException(nameof(family));
            }

            context.Families.Add(family as Family);
            context.SaveChanges();
        }

        public void RemoveFamily(IFamily family)
        {
            if (family == null)
            {
                throw new ArgumentNullException(nameof(family));
            }

            context.Families.Remove(family as Family);
            context.SaveChangesAsync();
        }

        public ICollection<Family> GetAllFamiliesAndRelatedData()
        {
            return context.Families
                        .Include(f => f.FamilyAdministrator)
                        .Include(f => f.FamilyUsers)
                        .Include(f => f.MainImage).ToList();
        }

        public void ChangeFamilyAdmin(IFamily family, IUser user)
        {
            if (family == null)
            {
                throw new ArgumentNullException(nameof(family));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Family contextFamily = context.Families.FirstOrDefaultAsync(f => f.ID == (family as Family).ID).Result;
            if (contextFamily == null)
                throw new ArgumentNullException(nameof(family));

            user.IsFamilyAdmin = true;
            contextFamily.FamilyAdministrator = user as ApplicationUser;
            context.SaveChanges();
        }

        public ICollection<IFamily> GetUserAdminFamilies(IUser userAdmin)
        {
            return context.Families.Where(f => f.FamilyAdministrator.Id == userAdmin.Id).Cast<IFamily>().ToList();
        }

        public ICollection<IFamily> GetUserFamiliesMemberOf(IUser user)
        {
            return context.Families.Where(f => f.FamilyUsers.Any(fu => fu.ApplicationUserId == user.Id)).Cast<IFamily>().ToList();
        }

        public ICollection<IFamily> GetUserFamiliesMemberOfWithAdminAndImages(IUser user)
        {
            return context.Families.Where(f => f.FamilyUsers.Any(fu => fu.ApplicationUserId == user.Id))
                .Include(f => f.FamilyAdministrator)
                .Include(f => f.Images)
                .Cast<IFamily>().ToList();
        }

        public ICollection<IFamily> GetUserAdminFamiliesWithMainImage(IUser userAdmin)
        {
            return context.Families.Where(f => f.FamilyAdministrator.Id == userAdmin.Id)
                        .Include(f => f.MainImage)
                        .Include(f => f.FamilyAdministrator).Cast<IFamily>().ToList();
        }

        public ICollection<IFamily> GetUserAdminFamiliesWithMainImageAndRequests(IUser userAdmin)
        {
            return context.Families.Where(f => f.FamilyAdministrator.Id == userAdmin.Id)
                        .Include(f => f.MainImage)
                        .Include(f => f.FamilyAdministrator)
                        .Include(f => f.FamilyRequests).Cast<IFamily>().ToList();
        }

        public ICollection<IFamily> GetUserFamiliesMemberOfWithMainImage(IUser user)
        {
            return context.Families.Where(f => f.FamilyUsers.Any(fu => fu.ApplicationUserId == user.Id)).Include(f => f.MainImage).Cast<IFamily>().ToList();
        }

        public IFamily GetFamilyByGuid(Guid guid)
        {
            return context.Families.FirstOrDefault(f => f.Guid == guid);
        }

        public void RemoveFamilyMemeber(IFamily familyParam, IUser member)
        {
            Family family = context.Families.Include(f => f.FamilyUsers).FirstOrDefault(f => f.Guid == familyParam.Guid);
            FamilyUsers familyUsers = family.FamilyUsers.FirstOrDefault(fu => fu.User.SecurityStamp == member.SecurityStamp);
            family.FamilyUsers.Remove(familyUsers);

            context.SaveChanges();
        }

        public ICollection<ApplicationUser> GetFamilyMemebers(IFamily familyParam)
        {
            Family family = context.Families.Include(f => f.FamilyUsers).FirstOrDefault(f => f.Guid == familyParam.Guid);
            var ids = family.FamilyUsers.SelectMany(f => new List<string> { f.ApplicationUserId }).ToList();

            return context.Users.Where(u => ids.Contains(u.Id.ToString())).ToList();
        }
    }
}
