using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Middleware
{
    public class ImageCachingMiddleware
    {
        private readonly RequestDelegate next;

        public ImageCachingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {


            await next.Invoke(context);
        }
    }
}
