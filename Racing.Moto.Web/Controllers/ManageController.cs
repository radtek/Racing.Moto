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
            var model = new UserCreditModel();

            try
            {
                model = new UserService().GetUserCredit(LoginUser.UserId);
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }

            return View(model);
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

                if (status == (int)LoginStatus.InvalidPassword)
                {
                    result.Success = false;
                    result.Message = "原始密码错误";
                }

                // 保存邮箱
                if (model.Email != LoginUser.Email)
                {
                    new UserService().SaveEmail(LoginUser.UserId, model.Email);
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

        /// <summary>
        /// 用户.今日已结/未结明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetUserBonusReport(UserReportSearchModel model)
        {
            var result = new ResponseResult();

            try
            {
                //result.Data = new BetService().GetUserBetReport(model);// 取Bet
                result.Data = new BetService().GetUserBetItemReport(model); // 取BetItem
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;

                _logger.Info(ex);
            }

            return Json(result);
        }

        /// <summary>
        /// 用户.今日已结/未结明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult GetUserBonusReportStatistics(UserReportSearchModel model)
        {
            var result = new ResponseResult();

            try
            {
                //result.Data = new BetService().GetUserBetReportStatistics(model);// 取Bet
                result.Data = new BetService().GetUserBetItemReportStatistics(model);// 取BetItem
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;

                _logger.Info(ex);
            }

            return Json(result);
        }

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

        #region 历史记录

        public ActionResult History()
        {
            return View();
        }

        #endregion
    }
}