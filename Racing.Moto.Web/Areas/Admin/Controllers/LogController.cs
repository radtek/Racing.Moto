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
    public class LogController : AdminBaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 登录日志

        /// <summary>
        /// 登录日志
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginRecord()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetLoginLogRecords(SearchModel searchModel)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new LoginLogService().GetLoginLogRecords(searchModel);
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