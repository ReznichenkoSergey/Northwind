using Microsoft.EntityFrameworkCore;
using Northwind.Database;
using System;

namespace Northwind.Tests.Fixtures
{
    internal class DbContextMock
    {
        internal NorthwindContext InitDbContext()
        {
            var options = new DbContextOptionsBuilder<NorthwindContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new NorthwindContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }
    }
}
