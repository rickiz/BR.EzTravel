using System.Web.Mvc;

namespace BR.EzTravel.Web.Areas.CN
{
    public class ZHAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CN";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CN_default",
                "CN/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "BR.EzTravel.Web.Areas.CN.Controllers" }
            );
        }
    }
}