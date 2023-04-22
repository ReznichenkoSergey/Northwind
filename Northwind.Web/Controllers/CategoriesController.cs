using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Database;
using Northwind.Web.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly NorthwindContext _context;
        private const string MimeType = "image/bmp";

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

        [HttpGet]
        [Route("image/{id}")]
        [Route("[controller]/uploadimage/{id}")]
        public async Task<IActionResult> UploadImage(int? id)
        {
            if (id == null || id.Value <= 0)
            {
                return NotFound();
            }

            var category = await _context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id.Value);

            if (category?.Picture == null)
            {
                return NotFound();
            }

            var stream = new MemoryStream(category.Picture.Skip(78).ToArray());
            return File(stream, MimeType, $"Image_{id.Value}.bmp");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id.Value <= 0)
            {
                return NotFound();
            }

            var category = await _context
                    .Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryPictureViewModel formFile)
        {
            var category = await _context
                    .Categories
                    .FirstOrDefaultAsync(x => x.CategoryId == formFile.Id);

            if (category == null)
            {
                return NotFound();
            }

            using var binaryReader = new BinaryReader(formFile.Picture.OpenReadStream());
            category.Picture = binaryReader.ReadBytes((int)formFile.Picture.Length);
            await _context.SaveChangesAsync();

            return RedirectToAction("Categories");
        }
    }
}
