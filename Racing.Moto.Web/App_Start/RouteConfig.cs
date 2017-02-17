﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Racing.Moto.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default1",
                url: "{controller}/{action}/{id}/{cid}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, cid = UrlParameter.Optional },
                namespaces: new string[] { "Racing.Moto.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default2",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Racing.Moto.Web.Controllers" }
            );
        }
    }
}
