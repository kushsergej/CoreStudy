using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Helpers
{
    [HtmlTargetElement("a", Attributes = "northwind-id")]
    public class NorthwindTagHelper: TagHelper
    {
        public string NorthwindId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            output.Attributes.SetAttribute("href", $"images/{NorthwindId}");

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
