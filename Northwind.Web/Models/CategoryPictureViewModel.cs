using Microsoft.AspNetCore.Http;
namespace Northwind.Web.Models
{
    public class CategoryPictureViewModel
    {
        public int Id { get; set; }
        public IFormFile Picture { get; set; }
    }
}
