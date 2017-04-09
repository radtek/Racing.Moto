using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Racing.Moto.Web.Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Admin_default_1",
                url: "{controller}/{action}/{id}/{cid}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional, cid = UrlParameter.Optional },
                namespaces: new string[] { "Racing.Moto.Web.Admin.Controllers" }
            );

            routes.MapRoute(
                name: "Admin_default_2",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new string[] { "Racing.Moto.Web.Admin.Controllers" }
            );
        }
    }
}
