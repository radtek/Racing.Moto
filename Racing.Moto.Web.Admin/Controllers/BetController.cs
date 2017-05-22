using NLog;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Admin.Controllers
{
    [Authorize]
    public class BetController : AdminBaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        // 即時注單信息
        public ActionResult Info(int id)
        {
            var isAdmin = new UserRoleService().IsAdmin(LoginUser.UserRoles.ToList());
            if (!isAdmin)
            {
                return Redirect("/News/Index");
            }

            ViewBag.Type = id;  // 1: 冠亚军, 2: 3-6名, 3: 7-10名

            return View();
        }

        public JsonResult GetBetInfo()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new BetService().GetBetStatistic();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }
    }
}