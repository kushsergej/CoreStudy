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
    public class CategoriesControllerTests
    {
        [Fact]
        public async Task IndexTest()
        {
            //Arrange
            IQueryable<Categories> categoriesList = new List<Categories>
            {
                new Categories { CategoryId = 1, CategoryName = "Milk", Description = "Milky food", Picture = new byte[0] },
                new Categories { CategoryId = 2, CategoryName = "Bread", Description = "Fresh bread", Picture = new byte[0] },
                new Categories { CategoryId = 3, CategoryName = "Sweets", Description = "Delicious", Picture = new byte[0] }
            }.AsQueryable();

            var categoriesMock = new Mock<DbSet<Categories>>();
            categoriesMock.As<IQueryable<Categories>>().Setup(p => p.Provider).Returns(categoriesList.Provider);
            categoriesMock.As<IQueryable<Categories>>().Setup(p => p.Expression).Returns(categoriesList.Expression);
            categoriesMock.As<IQueryable<Categories>>().Setup(p => p.ElementType).Returns(categoriesList.ElementType);
            categoriesMock.As<IQueryable<Categories>>().Setup(p => p.GetEnumerator()).Returns(categoriesList.GetEnumerator());

            var contextMock = new Mock<NorthwindContext>();
            contextMock
                .Setup(context => context.Categories)
                .Returns(categoriesMock.Object);

            var configMock = new Mock<IConfiguration>();
            
            var controller = new CategoriesController(contextMock.Object, configMock.Object);
            
            //Act
            ViewResult result = await controller.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Categories>>(viewResult.Model);
            Assert.Equal(categoriesList.Count(), model.Count());
        }
    }
}
