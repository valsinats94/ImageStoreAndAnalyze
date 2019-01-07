using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Data.DatabaseServices
{
    public abstract class BaseDatabaseService
    {
        protected ApplicationDbContext context;
        protected IServiceProvider serviceProvider;

        protected BaseDatabaseService(DbContext context, IServiceProvider serviceProvider)
        {
            this.context = context as ApplicationDbContext;
            this.serviceProvider = serviceProvider;
        }
    }
}
