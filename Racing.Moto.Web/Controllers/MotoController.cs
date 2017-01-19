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
    public class MotoController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        // 竞技场
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
        public JsonResult SaveBets(int pkId, List<Bet> bets)
        {
            var result = new ResponseResult();

            try
            {
                // 验证下注时间是否合法, 超过当前PK的开盘时期则非法, 提示已封盘[TODO]

                // 下注
                bets.ForEach(b =>
                {
                    b.PKId = pkId;
                    b.UserId = LoginUser.UserId;
                });
                new BetService().AddBets(bets);
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