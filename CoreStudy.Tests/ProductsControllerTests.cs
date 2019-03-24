using CoreStudy.Controllers;
using CoreStudy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoreStudy.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task IndexTest()
        {
            //Arrange
            IQueryable<Products> productsList = new List<Products>
            {
                new Products {ProductId = 1, ProductName = "A", SupplierId = 1, CategoryId = 1, QuantityPerUnit = "1", UnitPrice = 1, UnitsInStock = 1, UnitsOnOrder = 1,ReorderLevel = 1, Discontinued = true }
            }.AsQueryable();

            var productsMock = new Mock<DbSet<Products>>();
            productsMock.As<IQueryable<Products>>().Setup(p => p.Provider).Returns(productsList.Provider);
            productsMock.As<IQueryable<Products>>().Setup(p => p.Expression).Returns(productsList.Expression);
            productsMock.As<IQueryable<Products>>().Setup(p => p.ElementType).Returns(productsList.ElementType);
            productsMock.As<IQueryable<Products>>().Setup(p => p.GetEnumerator()).Returns(productsList.GetEnumerator());

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(config => config["M"]).Returns("5");

            var contextMock = new Mock<NorthwindContext>();
            contextMock
                .Setup(context => context.Products)
                .Returns(productsMock.Object);

            var controller = new ProductsController(contextMock.Object, configMock.Object);

            //Act
            ViewResult result = await controller.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Products>>(viewResult.Model);
            Assert.Equal(productsList.Count(), model.Count());
        }


        [Fact]
        public async Task CreateWasSuccessfulTest()
        {
            //Arrange
            var productsMock = new Mock<DbSet<Products>>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(config => config["M"]).Returns("5");

            var newProduct = new Products { ProductName = "A", SupplierId = 1, CategoryId = 1, QuantityPerUnit = "1", UnitPrice = 1, UnitsInStock = 1, UnitsOnOrder = 1, ReorderLevel = 1, Discontinued = true };

            var contextMock = new Mock<NorthwindContext>();
            contextMock
                .Setup(context => context.Products)
                .Returns(productsMock.Object)
                .Verifiable();

            var controller = new ProductsController(contextMock.Object, configMock.Object);
            
            //Act
            var result = await controller.Create(newProduct);

            //Assert
            Assert.NotNull(result);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }


        [Fact]
        public async Task EditWasNonSuccessfulTest()
        {
            //Arrange
            var productsMock = new Mock<DbSet<Products>>();

            var configMock = new Mock<IConfiguration>();
            configMock.Setup(config => config["M"]).Returns("5");

            int editingProductid = -1;

            var contextMock = new Mock<NorthwindContext>();
            contextMock
                .Setup(context => context.Products.Find(editingProductid))
                .Returns(null as Products);

            var controller = new ProductsController(contextMock.Object, configMock.Object);
            
            //Act
            var result = await controller.Edit(editingProductid);

            //Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }
    }
}
