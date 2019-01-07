using ImageStoreAndAnalyze.Interfaces;
using ImageStoreAndAnalyze.Interfaces.Services;
using ImageStoreAndAnalyze.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public class FamilyDatabaseService : BaseDatabaseService, IFamilyDatabaseService
    {
        public FamilyDatabaseService(DbContext context, IServiceProvider serviceProvider) 
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
            context.SaveChangesAsync();
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

            contextFamily.FamilyAdministrator = user as ApplicationUser;
            context.SaveChangesAsync();
        }
    }
}
