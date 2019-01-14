using ImageStoreAndAnalyze.Interfaces;
using ImageStoreAndAnalyze.Interfaces.Services;
using ImageStoreAndAnalyze.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public class FamilyRequestsDatabaseService : BaseDatabaseService, IFamilyRequestsDatabaseService
    {
        public FamilyRequestsDatabaseService(ApplicationDbContext context, IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {

        }

        public void AddFamilyRequest(FamilyRequest familyRequest)
        {
            if (familyRequest == null)
            {
                throw new ArgumentNullException(nameof(familyRequest));
            }

            context.FamilyRequests.Add(familyRequest);
            context.SaveChanges();
        }

        public void DeleteFamilyRequest(FamilyRequest familyRequest)
        {
            FamilyRequest request = context.FamilyRequests.FirstOrDefault(fr => fr.Id == familyRequest.Id);

            if (request == null)
                return;

            request.IsDeleted = true;
            context.SaveChanges();
        }

        public FamilyRequest GetFamilyRequestByGuid(Guid guid)
        {
            return context.FamilyRequests              
                .FirstOrDefault(fr => fr.Guid == guid);
        }

        public FamilyRequest GetFamilyRequestAndFamilyByGuid(Guid guid)
        {
            return context.FamilyRequests
                .Include(fr => fr.RequestedFamily)
                .FirstOrDefault(fr => fr.Guid == guid);
        }

        public ICollection<FamilyRequest> GetFamilyRequests(IFamily family)
        {
            return context.FamilyRequests.Where(fr => fr.RequestedFamily.Guid == family.Guid).ToList();
        }

        public ICollection<FamilyRequest> GetFamilyRequestsByUserRequested(IUser user)
        {
            return context.FamilyRequests.Where(fr => fr.RequestByUser.SecurityStamp == user.SecurityStamp).ToList();
        }

        public ICollection<FamilyRequest> GetFamilyRequestsProcessedByUser(IUser user)
        {
            return context.FamilyRequests.Where(fr => fr.ProcessedByUser.SecurityStamp == user.SecurityStamp).ToList();
        }

        public ICollection<FamilyRequest> GetNotProcessedFamilyRequests(IFamily family)
        {
            return context.FamilyRequests.Where(fr => fr.RequestedFamily.Guid == family.Guid
                                                && fr.IsProcessed == false).ToList();
        }
    }
}
