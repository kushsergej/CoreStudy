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
    public class ProductsController : Controller
    {
        #region DI
        private readonly NorthwindContext db;

        public ProductsController(NorthwindContext context)
        {
            db = context;
        }
        #endregion


        // GET: Products/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //using Eager Loading (.Include().ThenInclude()...)
            var allProducts = db.Products.Include(p => p.Category).Include(p => p.Supplier);
            return View(await allProducts.ToListAsync());
        }
    }
}
