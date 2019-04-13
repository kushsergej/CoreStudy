using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreStudy.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreStudy.Controllers
{
    public class ApiController : Controller
    {
        #region DI
        private readonly NorthwindContext db;

        public ApiController(NorthwindContext context)
        {
            db = context;
        }
        #endregion


        // GET: <site_root>/api
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var categories = db.Categories.Select(p => new
            {
                id = p.CategoryId,
                name = p.CategoryName,
                p.Description
            });

            var products = db.Products.Select(p => new
            {
                id = p.ProductId,
                name = p.ProductName,
                p.QuantityPerUnit,
                p.UnitPrice,
                p.UnitsInStock,
                p.UnitsOnOrder,
                p.ReorderLevel,
                p.Discontinued,
                p.Category.CategoryName,
                p.Supplier.CompanyName
            });

            return await Task.FromResult(Json(new {categories, products}));
        }
    }
}
