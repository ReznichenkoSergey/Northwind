using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Northwind.Web.Controllers;
using Northwind.Web.Models;
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
        public void IndexShouldLogInformationAndReturnCorrectValue()
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

        [Fact]
        public void ErrorShouldLogInformationAndReturnCorrectValue()
        {
            //Arrange
            DefaultHttpContext httpContext = InitHttpContext();

            var controller = new HomeController(_logger.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContext;

            //Act
            var result = controller.Error();

            //Assert
            result.Should().BeOfType<ViewResult>();
            var actual = Assert.IsType<ViewResult>(result);
            Assert.IsType<ErrorViewModel>(actual.Model);
        }

        private static DefaultHttpContext InitHttpContext()
        {
            var headerDictionary = new HeaderDictionary();
            var features = new FeatureCollection();
            var requestFeature = new HttpRequestFeature()
            {
                Headers = headerDictionary,
            };
            features.Set<IHttpRequestFeature>(requestFeature);
            var httpContext = new DefaultHttpContext(features);
            httpContext.TraceIdentifier = Guid.NewGuid().ToString();
            return httpContext;
        }
    }
}
