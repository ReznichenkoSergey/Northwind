using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Database;
using System.Threading.Tasks;

namespace Northwind.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly NorthwindContext _context;

        public CategoriesController(ILogger<CategoriesController> logger,
            NorthwindContext context)
        {
            _logger = logger;
            _context = context;
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
