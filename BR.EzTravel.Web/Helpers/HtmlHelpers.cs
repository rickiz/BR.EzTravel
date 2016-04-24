using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BR.EzTravel.Web.Properties;

namespace BR.EzTravel.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString ListGroupLink(this HtmlHelper htmlHelper, string linkText, string actionName, string area = "")
        {
            return ListGroupLink(htmlHelper, linkText, actionName, new { area = area });
        }

        public static MvcHtmlString ListGroupLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            if (actionName.ToLower() == currentAction.ToLower())
                return LinkExtensions.ActionLink(htmlHelper, linkText, actionName, currentController, routeValues, new { @class = "list-group-item active" });
            else
                return LinkExtensions.ActionLink(htmlHelper, linkText, actionName, currentController, routeValues, new { @class = "list-group-item" });
        }

        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                            string controllerName, string area = "")
        {
            return MenuLink(htmlHelper, linkText, actionName, controllerName, new { area = area });
        }

        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                            string controllerName, object routeValues)
        {
            var actionLink = LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, null);

            if (actionLink == MvcHtmlString.Empty)
                return MvcHtmlString.Empty;

            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            var builder = new TagBuilder("li")
            {
                InnerHtml = actionLink.ToHtmlString()
            };

            if (controllerName.ToLower() == currentController.ToLower())
                builder.AddCssClass("active");

            return new MvcHtmlString(builder.ToString());
        }

        public static string UploadedImageContent(this UrlHelper urlHelper, string contentPath)
        {
            //if (string.IsNullOrEmpty(contentPath))
            //    return urlHelper.Content(Settings.Default.ImageUploadPath);

            if (string.IsNullOrEmpty(contentPath))
                return "";

            return urlHelper.Content(Path.Combine(Settings.Default.ImageUploadPath, contentPath));
        }

        public static string SetMenuActive(this HtmlHelper htmlHelper, string menuController)
        {
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            return menuController.ToLower() == currentController.ToLower() ? "active" : "";
        }

        public static string ContentAbsUrl(this UrlHelper url, string relativeContentPath)
        {
            Uri contextUri = HttpContext.Current.Request.Url;

            var baseUri = string.Format("{0}://{1}{2}", contextUri.Scheme,
               contextUri.Host, contextUri.Port == 80 ? string.Empty : ":" + contextUri.Port);

            return string.Format("{0}{1}", baseUri, VirtualPathUtility.ToAbsolute(relativeContentPath));
        }
    }
}
