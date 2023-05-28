using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Database;
using Northwind.Database.Tables;
using System;
using System.Collections.Generic;
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

        [HttpGet("Categories")]
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

        [HttpGet("Products")]
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
    }
}
