using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CoreStudy.Models;
using Microsoft.Extensions.Logging;
using CoreStudy.Services.Implementations;
using System.IO;
using Microsoft.AspNetCore.Diagnostics;
using CoreStudy.Services.Interfaces;
using Microsoft.AspNetCore.Routing.Constraints;
using CoreStudy.Middleware;
using CoreStudy.Filters;
using Microsoft.AspNetCore.Identity;

namespace CoreStudy
{
    public class Startup
    {
        #region DI
        private readonly IConfiguration configuration;
        
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #endregion


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //set up global filter
            services.AddMvc(options => options.Filters.Add<ActionLoggerFilter>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region Custom Services registration
            services.AddDbContext<NorthwindContext>(options => 
            {
                options.UseSqlServer(configuration.GetConnectionString("NorthwindDatabase"));
            });
            services.AddDbContext<ApplicationIdentityContext>(options => 
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityDatabase"));
            });
            
            services.AddRouting();
            services.AddSwaggerDocument(config => 
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "CoreStudy API";
                    document.Info.Description = "A study ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "Siarhei Kushniaruk",
                        Email = "Siarhei_Kushniaruk@epam.com"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "Use under LICX"
                    };
                };
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options => 
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).
            AddEntityFrameworkStores<ApplicationIdentityContext>();

            services.AddScoped<IGetFileLoggerProvider, GetFileLoggerProvider>();
            services.AddScoped<IAppStartLogger, AppStartLogger>();
            #endregion
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory  loggerFactory, IGetFileLoggerProvider fileLoggerProvider, IAppStartLogger appStartLogger)
        {
            #region Log Application start
            loggerFactory.AddProvider(fileLoggerProvider.GetProvider(configuration["LogFilePath"]));
            ILogger logger = loggerFactory.CreateLogger<Startup>();
            appStartLogger.Log();
            #endregion

            //env.EnvironmentName = EnvironmentName.Production;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //custom error handler
                app.UseExceptionHandler(errorApp =>
                {
                    //adds a terminal middleware delegate to the app's request pipeline
                    errorApp.Run(async context =>
                    {
                        IExceptionHandlerPathFeature error = context.Features.Get<IExceptionHandlerPathFeature>();
                        string path = error?.Path;
                        Exception stacktrace = error?.Error;

                        context.Response.StatusCode = 500;
                        context.Response.ContentType = $"text/html";
                        await context.Response.WriteAsync($"<html> <body>");
                        await context.Response.WriteAsync($"<h1> ERROR! </h1>");
                        await context.Response.WriteAsync($"<p>{DateTime.Now}:  Exception has been raised on {path} </p> <hr />");
                        await context.Response.WriteAsync($"<p> {stacktrace} </p> <hr />");
                        await context.Response.WriteAsync($"</body> </html>");

                        logger.LogInformation($"Exception has been raised on {path}.");
                        logger.LogInformation($"{stacktrace}");
                    });
                });
                app.UseHsts();
            }
            //app.UseMiddleware<ExceptionImitationMiddleware>();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseMiddleware<ImageCachingMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "images",
                    template: "images/{id:required:int:range(1,8)}",
                    defaults: new { controller = "Categories", action = "GetPictureById" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
