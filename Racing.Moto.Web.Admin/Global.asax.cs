using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Racing.Moto.Web.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);   // 必须先注册Api的路由
            RouteConfig.RegisterRoutes(RouteTable.Routes);          // 再注册普通Controller的路由, 否则api的路由无效
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //OnlineHttpModule
            Services.Mvc.OnlineHttpModule.Register();

            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}
