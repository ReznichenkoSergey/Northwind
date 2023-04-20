using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Northwind.Database;
using Northwind.Database.Tables;
using Northwind.Tests.Fixtures;
using Northwind.Web.Controllers;
using Northwind.Web.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Northwind.Tests
{
    public class ProductsControllerTest
    {
        private readonly Mock<ILogger<ProductsController>> _logger;
        private readonly Mock<IOptions<QueryOptionsConfig>> _options;
        private readonly NorthwindContext _context;
        private const int PRODUCTS_AMOUNT = 30;
        
        public ProductsControllerTest()
        {
            _logger = new Mock<ILogger<ProductsController>>();
            _options = new Mock<IOptions<QueryOptionsConfig>>();
            var mocks = new DbContextMock();
            _context = mocks.InitDbContext();
            AddProducts(_context, PRODUCTS_AMOUNT);
        }

        [Fact]
        public async Task IndexShouldReturnAllProducts()
        {
            //Arrange
            _options
                .SetupGet(x => x.Value)
                .Returns(new QueryOptionsConfig()
                {
                    TopLimit = 0
                });
            var controller = new ProductsController(_context, _options.Object, _logger.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.ViewData.Model);
            model.Should().HaveCount(PRODUCTS_AMOUNT);
        }

        [Fact]
        public async Task IndexShouldReturnLimitProducts()
        {
            //Arrange
            var expectedAmount = 10;
            _options
                .SetupGet(x => x.Value)
                .Returns(new QueryOptionsConfig()
                {
                    TopLimit = expectedAmount
                });
            var controller = new ProductsController(_context, _options.Object, _logger.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.ViewData.Model);
            model.Should().HaveCount(expectedAmount);
        }

        [Fact]
        public async Task CreateShouldAddProduct()
        {
            //Arrange
            var expected = new Product()
            {
                ProductId = 100,
                ProductName = "ProductName 100"
            };            
            var controller = new ProductsController(_context, _options.Object, _logger.Object);

            //Act
            var result = await controller.Create(expected);

            //Assert
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 100);
            product.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();
            _logger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(1));
        }

        [Fact]
        public async Task CreateShouldNotAddProduct()
        {
            //Arrange
            var expected = new Product()
            {
                ProductId = 100,
                ProductName = string.Empty
            };
            var controller = new ProductsController(_context, _options.Object, _logger.Object);
            controller.ModelState.AddModelError("Key", "Error");

            //Act
            var result = await controller.Create(expected);

            //Assert
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 100);
            product.Should().BeNull();
            result.Should().BeOfType<ViewResult>();
            _logger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task EditShouldUpdateProduct()
        {
            //Arrange
            var expectedName = "UpdatedName";
            var updated = _context.Products.First();
            updated.ProductName = expectedName;

            var controller = new ProductsController(_context, _options.Object, _logger.Object);

            //Act
            var result = await controller.Edit(updated.ProductId, updated);

            //Assert
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == updated.ProductId);
            product.ProductName.Should().Be(expectedName);
            result.Should().BeOfType<RedirectToActionResult>();
            _logger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(1));
        }

        [Fact]
        public async Task EditShouldReturnNotFoundWhenProductIdIsNotTheSame()
        {
            //Arrange
            var updated = new Product()
            {
                ProductId = 10000,
                ProductName = "UpdatedName"
            };

            var controller = new ProductsController(_context, _options.Object, _logger.Object);

            //Act
            var result = await controller.Edit(1, updated);

            //Assert
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == updated.ProductId);
            result.Should().BeOfType<NotFoundResult>();
            product.Should().BeNull();
            _logger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(1));
        }

        [Fact]
        public async Task EditShouldReturnNotFoundWhenProductIsNotValid()
        {
            //Arrange
            var updated = new Product()
            {
                ProductId = 10000,
                ProductName = "UpdatedName"
            };

            var controller = new ProductsController(_context, _options.Object, _logger.Object);
            controller.ModelState.AddModelError("Key", "Error");

            //Act
            var result = await controller.Edit(updated.ProductId, updated);

            //Assert
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == updated.ProductId && p.ProductName.Equals(updated.ProductName));
            product.Should().BeNull();
            result.Should().BeOfType<ViewResult>();
            _logger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Never);
        }

        private void AddProducts(NorthwindContext context, int limit)
        {
            _context.Products.RemoveRange(context.Products);
            _context.Products.AddRange(Enumerable.Range(1, limit)
                .Select(x => new Product
                {
                    ProductId = x,
                    ProductName = $"Name {x}"
                }));
            _context.SaveChanges();
        }

    }
}
