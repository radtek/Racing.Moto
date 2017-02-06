using NLog;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 所有用户

        public ActionResult All()
        {
            ViewBag.UserType = UserType.All;

            return View();
        }

        #endregion

        #region 总代理

        public ActionResult GeneralAgent()
        {
            ViewBag.UserType = UserType.GeneralAgent;

            return View();
        }

        #endregion

        #region 代理

        public ActionResult Agent()
        {
            ViewBag.UserType = UserType.Agent;

            return View();
        }

        #endregion

        #region 会员

        public ActionResult Member()
        {
            ViewBag.UserType = UserType.Member;

            return View();
        }

        #endregion

        #region 游客

        public ActionResult Vistor()
        {
            ViewBag.UserType = UserType.Vistor;

            return View();
        }

        #endregion


        public JsonResult GetUsers(UserSearchModel searchModel)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new UserService().GetUsers(searchModel);
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