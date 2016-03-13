using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BR.EzTravel.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString ListGroupLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                            string controllerName, string area = "")
        {
            return ListGroupLink(htmlHelper, linkText, actionName, controllerName, new { area = area });
        }

        public static MvcHtmlString ListGroupLink(this HtmlHelper htmlHelper, string linkText, string actionName,
                                            string controllerName, object routeValues)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            if (controllerName.ToLower() == currentController.ToLower() && actionName.ToLower() == currentAction.ToLower())
                return LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, new { @class = "list-group-item active" });
            else
                return LinkExtensions.ActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, new { @class = "list-group-item" });
        }

    }
}
