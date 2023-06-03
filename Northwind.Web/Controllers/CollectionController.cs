using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Database;
using Northwind.Database.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly ILogger<CollectionController> _logger;
        private readonly NorthwindContext _context;
        public CollectionController(ILogger<CollectionController> logger,
            NorthwindContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Categories", Name = nameof(GetCategoryListAsync))]
        public async Task<IList<Category>> GetCategoryListAsync(int maxAmount = 10)
        {
            try
            {
                var collection = await _context.Categories
                    .AsNoTracking()
                    .Take(maxAmount)
                    .ToListAsync();
                return collection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        [HttpGet("Category/Image/{id}", Name = nameof(GetImageByIdAsync))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> GetImageByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound();
                }

                var category = await _context
                    .Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CategoryId == id);

                if (category?.Picture == null)
                {
                    return NotFound();
                }

                var stream = new MemoryStream(category.Picture.Skip(78).ToArray());
                return File(stream, "image/bmp", $"Image_{category.CategoryId}.bmp");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("Category/Image", Name = nameof(SetImageByIdAsync))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SetImageByIdAsync([FromBody] ImageDto imageDto)
        {
            try
            {
                if (imageDto?.CategoryId <= 0)
                {
                    return NotFound();
                }

                var category = await _context
                    .Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CategoryId == imageDto.CategoryId);

                if (category == null)
                {
                    return NotFound();
                }
                category.Picture = imageDto.Content;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Products", Name = nameof(GetProductListAsync))]
        public async Task<IList<Product>> GetProductListAsync(int maxAmount = 10)
        {
            try
            {
                var collection = await _context.Products
                    .AsNoTracking()
                    .Take(maxAmount)
                    .ToListAsync();
                return collection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        [HttpPost("Products", Name = nameof(AddProductAsync))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> AddProductAsync([FromBody] ProductDto product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("Object is null");
                }

                _context
                    .Products
                    .Add(new Product()
                    {
                        CategoryId = product.CategoryId,
                        ProductName = product.Name,
                        SupplierId = product.SupplierId,
                        QuantityPerUnit = product.QuantityPerUnit,
                        UnitPrice = product.UnitPrice,
                        UnitsOnOrder = product.UnitsOnOrder,
                        ReorderLevel = product.ReorderLevel,
                        Discontinued = product.Discontinued
                    });
                await _context.SaveChangesAsync();

                return Ok("Added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Products", Name = nameof(UpdateProductAsync))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> UpdateProductAsync([FromBody] ProductDto product)
        {
            if (product == null || product.Id <= 0)
            {
                return NotFound();
            }

            var current = await _context
                .Products
                .FirstOrDefaultAsync(x => x.ProductId == product.Id);
            if (current == null)
            {
                return NotFound();
            }

            current.ProductName = product.Name;
            current.SupplierId = product.SupplierId;
            current.QuantityPerUnit = product.QuantityPerUnit;
            current.CategoryId = product.CategoryId;
            current.UnitPrice = product.UnitPrice;
            current.UnitsOnOrder = product.UnitsOnOrder;
            current.ReorderLevel = product.ReorderLevel;
            current.Discontinued = product.Discontinued;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Updated successfully");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Products", Name = nameof(DeleteProductAsync))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            _context
                .Products
                .Remove(product);
            try
            {
                await _context.SaveChangesAsync();

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class ImageDto
    {
        public int CategoryId { get; set; }
        public byte[] Content { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public int SupplierId { get; set; }
        public int CategoryId { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal UnitPrice { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
    }

}
