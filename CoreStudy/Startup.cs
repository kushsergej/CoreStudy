﻿using System;
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

            services.AddDbContext<NorthwindContext>(options => 
            {
                options.UseSqlServer(configuration.GetConnectionString("NorthwindDatabase"));
            });


            //Module 1. Task 5 (logging)
            ILoggerProvider LogFileProvider = new FileLoggerProvider(Path.Combine(Directory.GetCurrentDirectory(), configuration["LogFilePath"]));

            //add FileLoggerProvider to services (will be avaialable inside Configure method)
            services.AddLogging(configure =>
            {
                configure.AddConfiguration(configuration.GetSection("Logging"));
                configure.ClearProviders();
                configure.AddConsole();
                configure.AddDebug();
                configure.AddProvider(LogFileProvider);
            });

            //write logs from ConfigureServices directly
            var logger = LogFileProvider.CreateLogger("Startup");
            logger.LogInformation($"");
            logger.LogInformation($"    Start application       >>>     {DateTime.Now}");
            logger.LogInformation($"    Application location    >>>     {Directory.GetCurrentDirectory()}");
            logger.LogInformation($"    Read configuration (current configuration values):");
            logger.LogInformation(GetConfigSectionValue(configuration));
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
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
