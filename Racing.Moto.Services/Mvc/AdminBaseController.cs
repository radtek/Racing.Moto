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

                var adminRoleNames = new string[] { DBConst.Role_Name_Admin, DBConst.Role_Name_General_Agent, DBConst.Role_Name_Agent };
                if (!userRoles.Where(r => adminRoleNames.Contains(r.Role.RoleName)).Any())
                {
                    var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    var loginUrl = "/Account/Login";
                    var url = !string.IsNullOrEmpty(returnUrl) ? loginUrl + "?returnUrl=" + returnUrl : loginUrl;
                    filterContext.HttpContext.Response.Redirect(url);
                }
            }
        }
    }
}
