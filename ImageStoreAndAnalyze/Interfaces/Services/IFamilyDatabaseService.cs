using ImageStoreAndAnalyze.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Interfaces.Services
{
    public interface IFamilyDatabaseService
    {
        void AddFamily(IFamily family);

        void RemoveFamily(IFamily family);

        ICollection<Family> GetAllFamiliesAndRelatedData();

        void ChangeFamilyAdmin(IFamily family, IUser user);

        IFamily GetFamilyByGuid(Guid guid);

        ICollection<IFamily> GetUserAdminFamilies(IUser userAdmin);

        ICollection<IFamily> GetUserFamiliesMemberOf(IUser user);

        ICollection<IFamily> GetUserFamiliesMemberOfWithAdminAndImages(IUser user);

        ICollection<IFamily> GetUserAdminFamiliesWithMainImage(IUser userAdmin);

        ICollection<IFamily> GetUserAdminFamiliesWithMainImageAndRequests(IUser userAdmin);

        ICollection<IFamily> GetUserFamiliesMemberOfWithMainImage(IUser user);

        void RemoveFamilyMemeber(IFamily family, IUser member);

        ICollection<ApplicationUser> GetFamilyMemebers(IFamily familyParam);
    }
}
