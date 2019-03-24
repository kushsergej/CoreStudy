using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreStudy.Models;
using Microsoft.Extensions.Logging;

namespace CoreStudy.Controllers
{
    public class CategoriesController : Controller
    {
        #region DI
        private readonly NorthwindContext db;
        
        public CategoriesController(NorthwindContext context)
        {
            db = context;
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

            Categories categoryItem = await db.Categories.FindAsync(id);

            if (categoryItem == null)
            {
                return NotFound();
            }

            return View(categoryItem);
        }
    }
}
