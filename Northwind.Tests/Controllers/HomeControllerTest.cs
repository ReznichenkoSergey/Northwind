using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Northwind.Web.Controllers;
using System;
using Xunit;

namespace Northwind.Tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly Mock<ILogger<HomeController>> _logger;

        public HomeControllerTest()
        {
            _logger = new Mock<ILogger<HomeController>>();
        }

        [Fact]
        public void IndexShouldLogInformation()
        {
            //Arrange
            var controller = new HomeController(_logger.Object);

            //Act
            var result = controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
            _logger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(1));
        }
    }
}
