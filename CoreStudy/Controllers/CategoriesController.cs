using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreStudy.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CoreStudy.Controllers
{
    public class CategoriesController : Controller
    {
        #region DI
        private readonly NorthwindContext db;
        private readonly IConfiguration configuration;
        
        public CategoriesController(NorthwindContext context, IConfiguration configuration)
        {
            db = context;
            this.configuration = configuration;
        }
        #endregion


        // GET: Categories/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await Task.FromResult(db.Categories.ToList()));
        }


        // GET: Categories/GetPictureById/2
        [HttpGet]
        public async Task<IActionResult> GetPictureById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            byte[] img = null;

            //check, does requested file already exists in a locally cache
            string checkingFile = Path.Combine(Directory.GetCurrentDirectory(), configuration["Caching:CacheFolderPath"], $"{id}.bmp");
            if (System.IO.File.Exists(checkingFile))
            {
                //skip first 78 garbage bytes
                img = (await System.IO.File.ReadAllBytesAsync(checkingFile))
                    .Skip(78).ToArray();
            }
            else
            {
                Categories categoryItem = await db.Categories.FindAsync(id);

                if (categoryItem == null)
                {
                    return NotFound();
                }

                //skip first 78 garbage bytes
                img = categoryItem.Picture.Skip(78).ToArray();
            }

            ViewBag.categoryId = id;

            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("image_id", id.ToString());

            return View(img);
        }


        //POST: Categories/UploadFile/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(int? categoryId, IFormFile uploadImage)
        {
            if (categoryId == null)
            {
                return NotFound();
            }

            Categories categoryItem = await db.Categories.FindAsync(categoryId);

            if (categoryItem == null)
            {
                return NotFound();
            }

            if (uploadImage == null)
            {
                return BadRequest();
            }

            //read uploadFile to byte[]
            byte[] pic = null;
            using (BinaryReader reader = new BinaryReader(uploadImage.OpenReadStream()))
            {
                pic = reader.ReadBytes((int) uploadImage.Length);
            }

            //add first 78 garbage bytes
            byte[] trashedPic = new byte[pic.Length + 78];
            pic.CopyTo(trashedPic, 78);

            categoryItem.Picture = trashedPic;
            db.Update(categoryItem);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(GetPictureById), new { id = categoryId });
        }
    }
}
