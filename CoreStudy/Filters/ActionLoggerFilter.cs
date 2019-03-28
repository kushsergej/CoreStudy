using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Filters
{
    public class ActionLoggerFilter : Attribute, IAsyncActionFilter
    {
        #region DI
        private readonly ILogger logger;
        private readonly bool filterSwitchEnabled;

        public ActionLoggerFilter(IConfiguration configuration, ILogger<ActionLoggerFilter> logger)
        {
            this.logger = logger;
            filterSwitchEnabled = bool.Parse(configuration["FilterSwitch"]);
        }
        #endregion


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();

            if (filterSwitchEnabled)
            {
                logger.LogInformation($"Controller='{controller}', Action='{action}' started");
            }
            
            await next();

            if (filterSwitchEnabled)
            {
                logger.LogInformation($"Controller='{controller}', Action='{action}' ended");
            }
        }
    }
}
