using Microsoft.AspNetCore.Mvc;
using System;

namespace Northwind.Web.Views.Components
{
    [ViewComponent]
    public class BreadcrumbsViewComponent : ViewComponent
    {
        public string Invoke()
        {
            var path = HttpContext.Request.Path.Value.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            switch(path.Length)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return "Home > Entity";
                case 2:
                case 3:
                    return $"Home > Entity > {Map(path[1])}";
                default:
                    return $"Home{HttpContext.Request.Path.Value.Replace("/", " > ")}";
            }
        }

        private string Map(string value) => value.ToLower() switch
        {
            "create" => "Create New",
            "edit" => "Edit",
            _ => value
        };

        
    }
}
