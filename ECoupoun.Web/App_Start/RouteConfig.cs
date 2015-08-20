using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECoupoun.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ParentCategory",
                url: "{parentCategory}",
                defaults: new
                {
                    controller = "Product",
                    action = "Index",
                    product = "{id}"
                }
            );

            // Sub Category/
            routes.MapRoute(
                name: "Category",
                url: "{parentCategory}/{categoryName}",
                defaults: new
                {
                    controller = "Product",
                    action = "Index",
                    product = "{id}"
                }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );


            //if role is Administrator
            routes.MapRoute(
                "DefaultAdmin", // Route name
                "Admin/{controller}/{action}/{id}", // URL with parameters
                new { Area = "Admin", controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
