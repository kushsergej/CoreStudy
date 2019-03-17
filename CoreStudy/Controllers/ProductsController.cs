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

        public ProductsController(NorthwindContext context, IConfiguration configuration)
        {
            db = context;
            this.configuration = configuration;
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


        //GET: Products/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Products product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["SupplierId"] = new SelectList(await db.Suppliers.ToListAsync(), "SupplierId", "CompanyName", product.SupplierId);
            ViewData["CategoryId"] = new SelectList(await db.Categories.ToListAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }


        //POST: Products/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId", "ProductName", "SupplierId", "CategoryId", "QuantityPerUnit", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "ReorderLevel", "Discontinued")] Products product)
        {
            //check consistency of URL id
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(product);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //check, does such Product not exists
                    if (! await IsProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["SupplierId"] = new SelectList(await db.Suppliers.ToListAsync(), "SupplierId", "CompanyName", product.SupplierId);
            ViewData["CategoryId"] = new SelectList(await db.Categories.ToListAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }


        private async Task<bool> IsProductExists(int id)
        {
            return await db.Products.AnyAsync(p => p.ProductId == id);
        }
    }
}
