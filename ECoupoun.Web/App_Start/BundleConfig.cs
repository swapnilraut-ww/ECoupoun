using System;
using System.Web;
using System.Web.Optimization;

namespace ECoupoun.Web
{
    public class BundleConfig
    {
        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/core").Include(
                      "~/Scripts/script.js",
                      "~/Scripts/CustomJs/GlobalJs.js",
                            "~/Scripts/nav.js",
                                  "~/Scripts/move-top.js",
                                  "~/Scripts/easing.js",
                                  "~/Scripts/nav-hover.js",
                                  "~/Scripts/jquery.flexslider.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(              
                      "~/Content/css/style.css",
                      "~/Content/css/menu.css",
                      "~/Content/css/flexslider.css",
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/CustomLayout.css"
                      ));
        }
    }
}
