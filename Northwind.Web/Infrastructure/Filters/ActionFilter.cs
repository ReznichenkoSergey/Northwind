using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Northwind.Web.Infrastructure.Configs;

namespace Northwind.Web.Infrastructure.Filters
{
    public class ActionFilter : IActionFilter
    {
        private readonly ILogger<ActionFilter> _logger;
        private readonly IOptions<FilterProfileConfig> _option;

        public ActionFilter(ILogger<ActionFilter> logger,
            IOptions<FilterProfileConfig> option) 
        {
            _logger = logger;
            _option = option;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            WriteLog($"{context.RouteData.Values["controller"]}\\{context.RouteData.Values["action"]} ends");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            WriteLog($"{context.RouteData.Values["controller"]}\\{context.RouteData.Values["action"]} starts");
        }

        private void WriteLog(string message)
        {
            if (_option.Value.Active)
            {
                _logger.LogWarning(message);
            }
        }
    }
}
