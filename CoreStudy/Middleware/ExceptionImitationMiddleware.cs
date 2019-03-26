using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Middleware
{
    public class ExceptionImitationMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionImitationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            int x = 0;
            int y = 8 / x;
            await context.Response.WriteAsync($"Result = {y}");

            await next.Invoke(context);
        }
    }
}
