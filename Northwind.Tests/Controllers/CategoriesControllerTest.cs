using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Northwind.Database;
using Northwind.Database.Tables;
using Northwind.Tests.Fixtures;
using Northwind.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
namespace Northwind.Tests
{
    public class CategoriesControllerTest
    {
        private readonly Mock<ILogger<CategoriesController>> _logger;
        private readonly NorthwindContext _context;
        
        public CategoriesControllerTest()
        {
            _logger = new Mock<ILogger<CategoriesController>>();
            var mocks = new DbContextMock();
            _context = mocks.InitDbContext();
        }

        [Fact]
        public async Task CategoriesShouldReturnCorrectValue()
        {
            //Arrange
            await AddCategories(_context);
            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.Categories();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.ViewData.Model);
            model.Should().HaveCount(2);
        }

        private async Task AddCategories(NorthwindContext _context)
        {
            _context.Categories.AddRange(
                new Category { CategoryId = 1, CategoryName = "Name 1" },
                new Category { CategoryId = 2, CategoryName = "Name 1" }
            );
            await _context.SaveChangesAsync();
        }

    }
}
