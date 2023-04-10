using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Northwind.Database;
using Northwind.Web.Infrastructure.Helpers;
using Northwind.Web.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Northwind.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext _context;
        private readonly IOptions<QueryOptionsConfig> _options;

        public HomeController(ILogger<HomeController> logger, 
            NorthwindContext context, 
            IOptions<QueryOptionsConfig> options)
        {
            _logger = logger;
            _context = context;
            _options = options;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _context
                .Categories
                .AsNoTracking()
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Products()
        {
            var topAmount = _options.Value.TopLimit;
            var products = await _context
                .Products
                .TakeWithLimit(topAmount)
                .Include(x=>x.Category)
                .Include(x=>x.Supplier)
                .AsNoTracking()
                .ToListAsync();

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
