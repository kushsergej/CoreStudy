using CoreStudy.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Services.Implementations
{
    public class AppStartLogger : IAppStartLogger
    {
        #region DI
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public AppStartLogger(ILogger<Startup> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }
        #endregion

        public void Log()
        {
            logger.LogInformation($"    >>>>>>>>>>>>>>>>>");
            logger.LogInformation($"    Start application");
            logger.LogInformation($"    Application location    >>>     {Directory.GetCurrentDirectory()}");
            logger.LogInformation($"    Read configuration (current configuration values)   >>>     see below:");
            logger.LogInformation(GetConfigSectionValue(configuration));
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
