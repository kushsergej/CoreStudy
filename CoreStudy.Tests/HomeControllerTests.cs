using CoreStudy.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CoreStudy.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexViewResultIsNotNull()
        {
            //Arrange
            var controller = new HomeController();

            //Act
            var result = controller.Index();

            //Assert
            Assert.NotNull(result);
        }
    }
}
