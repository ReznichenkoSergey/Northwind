using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Web.TagHelpers
{
    public static class NorthwindImageLinkHelper
    {
        public static IHtmlContent NorthwindImageLink(this IHtmlHelper helper, int imageId, string text)
        {
            return new HtmlString(string.Format("<a href='images/{0}'>{1}</a>", imageId, text));

        }
    }
}
