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

namespace Racing.Moto.Web.Areas.Admin.Controllers
{
    public class LotteryController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 历史开奖

        public ActionResult History()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetHistory(SearchModel searchModel)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new PKService().GetPKs(searchModel);
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
    }
}