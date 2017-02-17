using System.Web.Mvc;

namespace Racing.Moto.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default_1",
                "Admin/{controller}/{action}/{id}/{cid}",
                new { action = "Index", id = UrlParameter.Optional, cid = UrlParameter.Optional },
                new string[] { "Racing.Moto.Web.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "Admin_default_2",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "Racing.Moto.Web.Areas.Admin.Controllers" }
            );
        }
    }
}