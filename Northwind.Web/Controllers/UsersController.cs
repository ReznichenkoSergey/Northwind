using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Web.Areas.Identity.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly ApplicationContext _context;
        public UsersController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var emailList = await _context
                .Users
                .Select(u => u.Email)
                .AsNoTracking()
                .ToListAsync();

            return View(emailList);
        }
    }
}
