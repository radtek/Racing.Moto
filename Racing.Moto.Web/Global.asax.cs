using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using Racing.Moto.Web.Jobs;
using System.Web.Optimization;

namespace Racing.Moto.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Services.Mvc.OnlineHttpModule.Register();

            MvcHandler.DisableMvcResponseHeader = true;

            //启动定时任务
            JobScheduler.Start();
        }
    }
}