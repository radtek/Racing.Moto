using Racing.Moto.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Racing.Moto.Services.Mvc
{
    public class AdminBaseController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (LoginUser != null)
            {
                var userRoles = new UserRoleService().GetUserRoles(LoginUser.UserId);

                var adminRoleNames = new string[] { RoleConst.Role_Name_Admin, RoleConst.Role_Name_General_Agent, RoleConst.Role_Name_Agent };
                if (!userRoles.Where(r => adminRoleNames.Contains(r.Role.RoleName)).Any())
                {
                    var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    var homeUrl = "/";
                    var url = !string.IsNullOrEmpty(returnUrl.TrimEnd('/')) ? homeUrl + "?returnUrl=" + returnUrl : homeUrl;
                    filterContext.HttpContext.Response.Redirect(url);
                }
            }
        }
    }
}
