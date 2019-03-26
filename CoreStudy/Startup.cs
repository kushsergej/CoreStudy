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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region Custom Services registration
            services.AddDbContext<NorthwindContext>(options => 
            {
                options.UseSqlServer(configuration.GetConnectionString("NorthwindDatabase"));
            });

            services.AddRouting();

            services.AddScoped<IGetFileLoggerProvider, GetFileLoggerProvider>();
            #endregion
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory  loggerFactory, IGetFileLoggerProvider fileLoggerProvider)
        {
            loggerFactory.AddProvider(fileLoggerProvider.GetProvider(configuration["LogFilePath"]));
            ILogger logger = loggerFactory.CreateLogger<Startup>();

            #region App start logging
            logger.LogInformation($"    >>>>>>>>>>>>>>>>>");
            logger.LogInformation($"    Start application");
            logger.LogInformation($"    Application location    >>>     {Directory.GetCurrentDirectory()}");
            logger.LogInformation($"    Read configuration (current configuration values)   >>>     see below:");
            logger.LogInformation(GetConfigSectionValue(configuration));
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

            #region Exception imitation
            //app.Run(async (context) =>
            //{
            //    int x = 0;
            //    int y = 8 / x;
            //    await context.Response.WriteAsync($"Result = {y}");
            //});
            #endregion

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

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


        //recursive function for getting config settings
        private string GetConfigSectionValue(IConfiguration configuration)
        {
            string result = "";

            foreach (var section in configuration.GetChildren())
            {
                result += "\"" + section.Key + "\" : ";

                if (section.Value != null)
                {
                    result += "\"" + section.Value + "\",\n";
                }
                else
                {
                    string subSection = GetConfigSectionValue(section);
                    result += "{\n" + subSection + "},\n";
                }
            }

            return result;
        }
    }
}
