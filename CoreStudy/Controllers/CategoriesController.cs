using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreStudy.Models;

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
            return View(await db.Categories.ToListAsync());
        }
    }
}
