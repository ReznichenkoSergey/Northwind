using Northwind.Database.Tables;
using System.Linq;

namespace Northwind.Web.Infrastructure.Helpers
{
    public static class LinqExtentions
    {
        public static IQueryable<Product> TakeWithLimit(this IQueryable<Product> source, int topLimit)
        {
            if (topLimit == 0)
                return source;
            return source.Take(topLimit);
        }
    }
}
