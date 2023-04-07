using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Database;
using Northwind.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext _context;

        public HomeController(ILogger<HomeController> logger, NorthwindContext context)
        {
            _logger = logger;
            _context = context;
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
            var products = await _context
                .Products
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
