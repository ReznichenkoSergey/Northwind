using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Web.Infrastructure.Configs;

namespace Northwind.Web.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureOptionModels(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QueryOptionsConfig>(configuration.GetSection("QueryOptionsConfig"));
            services.Configure<FilterProfileConfig>(configuration.GetSection("FilterProfileConfig"));
        }
    }
}
