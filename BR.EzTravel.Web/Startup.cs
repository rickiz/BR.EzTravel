using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BR.EzTravel.Web.Startup))]
namespace BR.EzTravel.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
