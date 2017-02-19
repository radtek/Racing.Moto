using NLog;
using Racing.Moto.Data.Models;
using Racing.Moto.Services.Caches;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using Racing.Moto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Membership;

namespace Racing.Moto.Web.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 信用资料

        public ActionResult Credit()
        {
            return View();
        }

        #endregion

        #region 修改密码

        public ActionResult ChangePassword()
        {
            return View();
        }
        public JsonResult SaveChangePassword(ChangePasswordModel model)
        {
            var result = new ResponseResult();

            try
            {
                var status = SqlMembershipProvider.Provider.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                
                if(status == (int)LoginStatus.InvalidPassword)
                {
                    result.Success = false;
                    result.Message = "原始密码错误";
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        #endregion

        #region 未结明细

        public ActionResult Outstanding()
        {
            return View();
        }

        #endregion

        #region 今日已结

        public ActionResult Payment()
        {
            return View();
        }

        #endregion

        #region 规则

        public ActionResult Rule()
        {
            return View();
        }

        #endregion
    }
}