using System.Web;
using System.Web.Optimization;

namespace BR.EzTravel.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/tinymce").Include(
                      "~/Scripts/tinymce/tinymce.js"));

            bundles.Add(new ScriptBundle("~/bundles/map").Include(
                      "~/Scripts/map.js"));

            bundles.Add(new ScriptBundle("~/bundles/all").Include(
                     "~/Scripts/idangerous.swiper.min.js",
                     "~/Scripts/jquery.viewportchecker.min.js",
                     "~/Scripts/isotope.pkgd.min.js",
                     "~/Scripts/jquery.mousewheel.min.js",
                     "~/Scripts/jquery.circliful.min.js",
                     "~/Scripts/jquery.classycountdown.min.js",
                     "~/Scripts/jquery.throttle.min.js",
                     "~/Scripts/jquery.knob.min.js",
                     //"~/Scripts/map.js",
                     "~/Scripts/all.js"));

            bundles.Add(new ScriptBundle("~/bundles/eztravel").Include(
                "~/Scripts/underscore.js",
                "~/Scripts/rating.js",
                "~/Scripts/eztravel.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     "~/Scripts/dropzone/dropzone.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery-ui.structure.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/font-awesome.css",
                      "~/Content/rating.css",
                      "~/Content/custom.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                      "~/Content/themes/base/all.css"));

            bundles.Add(new StyleBundle("~/Content/offcanvas").Include(
                      "~/Content/offcanvas.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/validation").Include(
                      "~/Content/validation.css"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                     "~/Scripts/dropzone/basic.css",
                     "~/Scripts/dropzone/dropzone.css"));

            bundles.Add(new StyleBundle("~/Content/siteadmin").Include(
                      "~/Content/site-admin.css"));
        }
    }
}
