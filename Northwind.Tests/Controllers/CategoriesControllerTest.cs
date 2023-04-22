using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Northwind.Database;
using Northwind.Database.Tables;
using Northwind.Tests.Fixtures;
using Northwind.Web.Controllers;
using Northwind.Web.Models;
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

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CategoriesShouldReturnNotFoundWhenIdInvalid(int? id)
        {
            //Arrange
            await AddCategories(_context);
            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.UploadImage(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UploadImageShouldReturnNotFoundWhenIdNotFound()
        {
            //Arrange
            var expectedId = int.MaxValue;
            await AddCategories(_context);
            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.UploadImage(expectedId);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UploadImageShouldReturnCorrectValue()
        {
            //Arrange
            await AddCategories(_context);
            var category = await _context.Categories.FirstAsync();
            var expectedId = category.CategoryId;
            var expectedMimeType = "image/bmp";
            var expectedFileName = $"Image_{expectedId}.bmp";

            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.UploadImage(expectedId);

            //Assert
            var viewResult = Assert.IsType<FileStreamResult>(result);
            viewResult.ContentType.Should().Be(expectedMimeType);
            viewResult.FileDownloadName.Should().Be(expectedFileName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task EditShouldReturnNotFoundWhenIdInvalid(int? id)
        {
            //Arrange
            await AddCategories(_context);
            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.Edit(id);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditShouldReturnNotFoundWhenIdNotFound()
        {
            //Arrange
            var expectedId = int.MaxValue;
            await AddCategories(_context);
            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.Edit(expectedId);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditShouldReturnCorrectValue()
        {
            //Arrange
            await AddCategories(_context);
            var category = await _context.Categories.FirstAsync();
            var expectedId = category.CategoryId;
            
            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.Edit(expectedId);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Category>(viewResult.Model);
            model.Should().NotBeNull();
            model.CategoryId.Should().Be(expectedId);
        }

        [Fact]
        public async Task EditShouldReturnNotFoundWhenVMIdNotFound()
        {
            //Arrange
            var expectedId = int.MaxValue;
            await AddCategories(_context);
            var expectedVM = new CategoryPictureViewModel()
            {
                Id = expectedId,
                Picture = new FormFile()
                {
                    FileName = "fileName.bmp",
                    ContentType = "image/bmp",
                    Length = 1
                }
            };
            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.Edit(expectedVM);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditShouldReturnRedirectToCategoriesPage()
        {
            //Arrange
            await AddCategories(_context);
            var category = await _context.Categories.FirstAsync();
            var expectedId = category.CategoryId;
            var expectedVM = new CategoryPictureViewModel()
            {
                Id = expectedId,
                Picture = new FormFile()
                {
                    FileName = "fileName.bmp",
                    ContentType = "image/bmp",
                    Length = 1                    
                }
            };

            var controller = new CategoriesController(_logger.Object, _context);

            //Act
            var result = await controller.Edit(expectedVM);

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            viewResult.ActionName.Should().Be("Categories");
        }

        private async Task AddCategories(NorthwindContext _context)
        {
            _context.Categories.AddRange(
                new Category { CategoryId = 1, CategoryName = "Name 1", Picture = new byte[] { } },
                new Category { CategoryId = 2, CategoryName = "Name 1", Picture = new byte[] { } }
            );
            await _context.SaveChangesAsync();
        }

    }
}
