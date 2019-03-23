using CoreStudy.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoreStudy.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task IndexTest()
        {
            //Arrange
            HomeController controller = new HomeController();

            //Act
            ViewResult result = await controller.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
        }
    }
}
