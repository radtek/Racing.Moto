using NLog;
using Racing.Moto.Core.Crypto;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Racing.Moto.Web.ApiControllers
{
    [EnableCors("*", "*", "*")]
    public class WechatController : ApiController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 用户下注
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult AddBet(BetModel model)
        {
            var result = new ResponseResult();

            try
            {
                var user = new UserService().GetUserByUserName(model.userName);

                if (user == null)
                {
                    result.Success = false;
                    result.Message = MessageConst.USER_NOT_EXIST;

                    return result;
                }
                else
                {
                    if (CryptoUtils.Decrypt(user.Password) != model.passWd)
                    {
                        result.Success = false;
                        result.Message = MessageConst.USER_INVALID_USERNAME_OR_PASSWORD;

                        return result;
                    }
                }

                // 验证下注时间是否合法, 超过当前PK的开盘时期则非法, 提示已封盘
                var isOpening = new PKService().IsOpening(model.issue);
                if (!isOpening)
                {
                    result.Success = false;
                    result.Message = MessageConst.PK_IS_NOT_OPEN;

                    return result;
                }
                else
                {
                    var userExtendService = new UserExtensionService();

                    // 查验余额
                    var userExtend = userExtendService.GetUserExtension(user.UserId);

                    var betAmount = model.score;
                    if (betAmount > userExtend.Amount)
                    {
                        result.Success = false;
                        result.Code = MessageConst.USER_BALANCE_IS_NOT_ENOUGH_CODE;//余额不足
                        result.Data = userExtend.Amount;
                        result.Message = MessageConst.USER_BALANCE_IS_NOT_ENOUGH + " 当前余额 : " + userExtend.Amount;

                        try
                        {
                            PKBag.LoginUser.UserExtension.Amount = userExtend.Amount;
                        }
                        catch { }
                    }
                    else
                    {
                        var bets = new List<Bet> {
                            new Bet {
                                OrderNo = model.orderId,
                                PKId = model.issue,
                                UserId = userExtend.UserId,
                                Rank = model.rank,
                                Num = model.value,
                                Amount = model.score
                            }
                        };
                        // 下注
                        new BetService().SaveBets(model.issue, userExtend.UserId, bets, false);

                        // 更新余额
                        userExtendService.MinusAmount(userExtend.UserId, betAmount);
                        try
                        {
                            PKBag.LoginUser.UserExtension.Amount = userExtend.Amount - betAmount;
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);

                result.Success = false;
                result.Message = MessageConst.System_Error;
            }

            return result;
        }


        /// <summary>
        /// 取当前期
        /// </summary>
        [HttpPost]
        public IssueModel GetCurrentIssue()
        {
            var result = new IssueModel();

            try
            {
                var pk = new PKService().GetCurrentPK();
                if (pk != null)
                {
                    result.issue = pk.PKId;
                    result.blockingTime = pk.BeginTime.AddSeconds(pk.OpeningSeconds);
                    result.lotteryTime = pk.EndTime.AddSeconds(-pk.LotterySeconds);
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }

            return result;
        }
    }
}