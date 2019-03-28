using CoreStudy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Middleware
{
    public class ImageCachingMiddleware
    {
        #region DI
        private readonly RequestDelegate next;
        private readonly string cacheFolderPath;
        private readonly int maxCountOfCachedImages;
        private readonly int cacheExpirationTimeSec;

        public ImageCachingMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            cacheFolderPath = Path.Combine(Directory.GetCurrentDirectory(), configuration["Caching:CacheFolderPath"]);
            maxCountOfCachedImages = int.Parse(configuration["Caching:MaxCountOfCachedImages"]);
            cacheExpirationTimeSec = int.Parse(configuration["Caching:CacheExpirationTimeSec"]);
        }
        #endregion


        public async Task InvokeAsync(HttpContext context, NorthwindContext db)
        {
            await next.Invoke(context);

            await CleanCacheIfRequired();

            if (context.Response.ContentType == "application/octet-stream")
            {
                if (! await MaxCountOfCachedImagesReached())
                {
                    await SaveFileLocally(context, db);
                }
            }
        }


        private async Task SaveFileLocally(HttpContext context, NorthwindContext db)
        {
            string image_id = context.Response.Headers["image_id"];
            string path = Path.Combine(cacheFolderPath, $"{image_id}.bmp");
            
            Categories categoryItem = await db.Categories.FindAsync(int.Parse(image_id));
            byte[] img = categoryItem.Picture;

            File.WriteAllBytes(path, img);
        }


        private async Task<bool> MaxCountOfCachedImagesReached()
        {
            int currentCount = Directory.GetFiles(cacheFolderPath).Count();

            return (currentCount >= maxCountOfCachedImages);
        }


        private async Task CleanCacheIfRequired()
        {
            foreach (string image in Directory.GetFiles(cacheFolderPath))
            {
                DateTime creationTime = File.GetLastWriteTime(image);
                double secondsSinceCreated = (DateTime.Now).Subtract(creationTime).TotalSeconds;

                if (secondsSinceCreated >= cacheExpirationTimeSec)
                {
                    File.Delete(image);
                }
            }
        }
    }
}
