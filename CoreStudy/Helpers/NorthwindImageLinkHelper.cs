using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Helpers
{
    public static class NorthwindImageLinkHelper
    {
        //this method creates an Html Helper, which produce link to image in format "images/{image_id}"
        public static IHtmlContent NorthwindImageLink(this IHtmlHelper html, int imageId, string linkText)
        {
            return html.RouteLink(linkText, new { controller = "Categories", action = "GetPictureById", id = imageId });
        }
    }
}
