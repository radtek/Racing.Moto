using NLog;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Controllers
{
    [Authorize]
    public class MotoController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        // 竞技场
        [AllowAnonymous]
        public ActionResult Arena()
        {
            return View();
        }


        #region 下注

        public ActionResult Bet()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCurrentPKInfo()
        {
            var result = new ResponseResult();

            try
            {
                var pkModel = new PKService().GetCurrentPKModel();
                var pkRates = new PKRateService().GetPKRateModels(pkModel.PK.PKId);
                var bets = new BetService().GetBets(pkModel.PK.PKId, LoginUser.UserId);
                var rebates = new UserRebateService().GetUserRebates(LoginUser.UserId);
                result.Data = new
                {
                    PKModel = pkModel,
                    PKRates = pkRates,
                    Bets = bets,
                    Rebates = rebates
                };
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveBets(int pkId, List<Bet> bets)
        {
            var result = new ResponseResult();

            try
            {
                // 验证下注时间是否合法, 超过当前PK的开盘时期则非法, 提示已封盘
                var isOpening = new PKService().IsOpening(pkId);
                if (!isOpening)
                {
                    result.Success = false;
                    result.Message = MessageConst.PK_IS_NOT_OPEN;
                }
                else
                {
                    var userExtendService = new UserExtensionService();

                    // 查验余额
                    var userExtend = userExtendService.GetUserUserExtension(LoginUser.UserId);
                    var betAmount = bets.Sum(b => b.Amount);
                    if (betAmount > userExtend.Amount)
                    {
                        result.Success = false;
                        result.Code = MessageConst.USER_BALANCE_IS_NOT_ENOUGH_CODE;//余额不足
                        result.Data = userExtend.Amount;
                        result.Message = MessageConst.USER_BALANCE_IS_NOT_ENOUGH + " 当前余额 : " + userExtend.Amount;

                        PKBag.LoginUser.UserExtension.Amount = userExtend.Amount;
                    }
                    else
                    {
                        // 下注
                        new BetService().SaveBets(pkId, LoginUser.UserId, bets);

                        // 更新余额
                        userExtendService.MinusAmount(LoginUser.UserId, betAmount);
                        LoginUser.UserExtension.Amount = LoginUser.UserExtension.Amount - betAmount;

                        // 回传给前台, 更新余额
                        result.Data = LoginUser.UserExtension.Amount;
                    }
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

        /// <summary>
        /// 取用户某期已下注金额
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="pkId">期Id</param>
        /// <returns>已下注金额</returns>
        [HttpPost]
        public JsonResult GetSumAmountsByPkId(int userId, int pkId)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new BetService().GetSumAmounts(userId, pkId);
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

        public JsonResult Sample()
        {
            var result = new ResponseResult();

            try
            {

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