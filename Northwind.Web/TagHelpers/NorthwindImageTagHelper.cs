using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Northwind.Web.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "northwind-id")]
    public class NorthwindImageTagHelper : TagHelper
    {
        [HtmlAttributeName("northwind-id")]
        public int CategoryId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("href", $"images/{CategoryId}");
        }
    }
}
