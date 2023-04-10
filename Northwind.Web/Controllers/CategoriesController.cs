using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Northwind.Database;
using Northwind.Web.Infrastructure.Helpers;
using System.Threading.Tasks;

namespace Northwind.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly NorthwindContext _context;
        private readonly IOptions<QueryOptionsConfig> _options;

        public CategoriesController(ILogger<CategoriesController> logger,
            NorthwindContext context,
            IOptions<QueryOptionsConfig> options)
        {
            _logger = logger;
            _context = context;
            _options = options;
        }
        public async Task<IActionResult> Categories()
        {
            var categories = await _context
                .Categories
                .AsNoTracking()
                .ToListAsync();

            return View(categories);
        }
    }
}
