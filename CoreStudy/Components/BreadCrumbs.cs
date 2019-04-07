using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Components
{
    public class BreadCrumbs: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string controller = RouteData.Values["controller"].ToString();
            string action = RouteData.Values["action"].ToString();
            
            string result = $"<h4> <span style='color: black'> Home > </span> <span style='color: red'> {controller} > </span>";
            if (action != "Index")
            {
                result += $"<span style='color: yellow'> {action} > </span>";
            }
            result += $"</h4>";

            return View("Default", new HtmlContentViewComponentResult(new HtmlString(result)));
        }
    }
}
