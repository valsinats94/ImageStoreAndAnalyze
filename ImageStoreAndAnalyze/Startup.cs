using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ImageStoreAndAnalyze.Data;
using ImageStoreAndAnalyze.Models;
using ImageStoreAndAnalyze.Services;
using Microsoft.Extensions.Logging;
using ImageStoreAndAnalyze.Interfaces.Services;
using ImageStoreAndAnalyze.Data.DatabaseServices;
using ImageStoreAndAnalyze.Services.ImageAnalyzeServices;

namespace ImageStoreAndAnalyze
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var sp = services.BuildServiceProvider();
            // Add application services.
            RegisterApplicationServices(services);
            
            //services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddTransient<SeedDefaultData>();

            services.AddMvc();
        }

        private void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddSession();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IImageDatabaseService, ImageDatabaseService>();
            services.AddTransient<IFamilyDatabaseService, FamilyDatabaseService>();
            services.AddTransient<IFamilyRequestsDatabaseService, FamilyRequestsDatabaseService>();
            services.AddTransient<IAnalyzeMyImgService, AnalyzeMyImgService>();
            services.AddTransient<IUploadToAnalyzerService, UploadToAnalyzerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, SeedDefaultData seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
