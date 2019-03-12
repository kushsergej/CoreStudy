using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreStudy.Models;
using Microsoft.Extensions.Configuration;

namespace CoreStudy.Controllers
{
    public class ProductsController : Controller
    {
        #region DI
        private readonly NorthwindContext db;
        private readonly IConfiguration configuration;

        public ProductsController(NorthwindContext context, IConfiguration configs)
        {
            db = context;
            configuration = configs;
        }
        #endregion


        // GET: Products/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string M = configuration["M"];
            
            if (!Int32.TryParse(M, out int m))
            {
                return NotFound();
            };

            //using Eager Loading (.Include().ThenInclude()...)
            IQueryable<Products> allProducts = db.Products.Include(p => p.Category).Include(p => p.Supplier);

            if (m != 0)
            {
                allProducts = allProducts.Take(Math.Abs(m));
            }

            return View(await allProducts.ToListAsync());
        }


        //GET: Products/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //Suppliers and Categories should be rendered as dropdown list
            ViewData["SupplierId"] = new SelectList(await db.Suppliers.ToListAsync(), "SupplierId", "CompanyName");
            ViewData["CategoryId"] = new SelectList(await db.Categories.ToListAsync(), "CategoryId", "CategoryName");
            return View();
        }


        //POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName", "SupplierId", "CategoryId", "QuantityPerUnit", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "ReorderLevel", "Discontinued")] Products product)
        {
            //if product is valid
            if (ModelState.IsValid)
            {
                await db.AddAsync(product);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SupplierId"] = new SelectList(await db.Suppliers.ToListAsync(), "SupplierId", "CompanyName", product.SupplierId);
            ViewData["CategoryId"] = new SelectList(await db.Categories.ToListAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
    }
}
