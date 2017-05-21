using Racing.Moto.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Services.Mvc
{
    public class AdminBaseController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //if (LoginUser != null)
            //{
            //    // 按角色过滤
            //    var userRoles = new UserRoleService().GetUserRoles(LoginUser.UserId);

            //    var adminRoleNames = new string[] { RoleConst.Role_Name_Admin, RoleConst.Role_Name_General_Agent, RoleConst.Role_Name_Agent };
            //    if (!userRoles.Where(r => adminRoleNames.Contains(r.Role.RoleName)).Any())
            //    {
            //        var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl.ToLower().TrimEnd('/');
            //        var isManageUrl = returnUrl.Contains("/manage");
            //        var loginUrl = isManageUrl ? "/Admin/Account/Login" : "/Account/Login";
            //        //var loginUrl = "/Account/Login";
            //        var rdm = Guid.NewGuid().ToString("N");//防止浏览器缓存登录页面
            //        var url = !string.IsNullOrEmpty(returnUrl)
            //            ? loginUrl + "?returnUrl=" + HttpUtility.UrlEncode(returnUrl + "&r=" + rdm)
            //            : loginUrl + "?r=" + rdm;

            //        filterContext.HttpContext.Response.Redirect(url);
            //    }
            //}
        }
    }
}
