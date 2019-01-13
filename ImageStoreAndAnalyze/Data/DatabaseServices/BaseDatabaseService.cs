using ImageStoreAndAnalyze.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public class BaseDatabaseService
    {
        protected ApplicationDbContext context;
        protected IServiceProvider serviceProvider;

        protected BaseDatabaseService(DbContext context, IServiceProvider serviceProvider)
        {
            this.context = context as ApplicationDbContext;
            this.serviceProvider = serviceProvider;
        }

        public IUser GetUserIncludesUserFamilies(IUser user)
        {
            return context.Users
                .Include(u => u.FamilyUsers)
                .FirstOrDefault(u => u.SecurityStamp == user.SecurityStamp);
        }
        
        public int Save()
        {
            return context.SaveChanges();
        }
    }
}
