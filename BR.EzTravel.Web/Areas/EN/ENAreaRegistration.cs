using System.Web.Mvc;

namespace BR.EzTravel.Web.Areas.EN
{
    public class ENAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EN";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EN_default",
                "EN/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional }
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "BR.EzTravel.Web.Areas.EN.Controllers" }
            );
        }
    }
}