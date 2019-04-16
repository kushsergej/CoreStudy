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
    [Route("api")]
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
        //Invoke-RestMethod https://localhost:44396/api -Method GET
        [HttpGet]
        public async Task<IActionResult> Get()
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

            return await Task.FromResult(Json(new { categories, products }));
        }


        //GET: <site_root>/api/2
        //Invoke-RestMethod https://localhost:44396/api/2 -Method GET
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            Categories categoryItem = await db.Categories.FindAsync(id);

            if (categoryItem == null)
            {
                return NotFound();
            }

            //skip first 78 garbage bytes
            byte[] img = categoryItem.Picture.Skip(78).ToArray();

            return await Task.FromResult(File(img, "application/octet-stream", $"{id}.bmp"));
        }


        //POST: <site_root>/api
        //Invoke-RestMethod https://localhost:44396/api -Method POST -Body (@{ProductName = "test"} | ConvertTo-Json) -ContentType "application/json"
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Products product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            await db.Products.AddAsync(product);
            await db.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = product.ProductId }, product);
        }


        //PUT: <site_root>/api/2
        //Invoke-RestMethod https://localhost:44396/api/2 -Method PUT -Body (@{ ProductId = 2; ProductName = "Chang"} | ConvertTo-Json) -ContentType "application/json"
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]Products product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            if (product == null)
            {
                return BadRequest();
            }

            if (!db.Products.Any(p => p.ProductId == product.ProductId))
            {
                return NotFound();
            }

            db.Update(product);
            await db.SaveChangesAsync();
            return CreatedAtAction("Get", new { id = product.ProductId }, product);
        }


        //DELETE: <site_root>/api/1087
        //Invoke-RestMethod https://localhost:44396/api/1087 -Method DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Products product = await db.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return Ok(product);
        }
    }
}
