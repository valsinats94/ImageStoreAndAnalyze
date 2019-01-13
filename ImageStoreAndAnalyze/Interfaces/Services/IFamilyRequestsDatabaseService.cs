using ImageStoreAndAnalyze.Models;
using System;
using System.Collections.Generic;

namespace ImageStoreAndAnalyze.Interfaces.Services
{
    public interface IFamilyRequestsDatabaseService
    {
        void AddFamilyRequest(FamilyRequest familyRequest);

        void DeleteFamilyRequest(FamilyRequest familyRequest);

        ICollection<FamilyRequest> GetFamilyRequests(IFamily family);

        ICollection<FamilyRequest> GetFamilyRequestsByUserRequested(IUser user);

        ICollection<FamilyRequest> GetFamilyRequestsProcessedByUser(IUser user);

        IUser GetUserIncludesUserFamilies(IUser user);

        FamilyRequest GetFamilyRequestByGuid(Guid guid);

        FamilyRequest GetFamilyRequestAndFamilyByGuid(Guid guid);

        int Save();
    }
}
