using NLog;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region Report
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetSearchReportData()
        {
            var result = new ResponseResult();

            try
            {
                //期數
                var historyPKs = new PKService().GetSettlementPKs(30);

                result.Data = new
                {
                    HistoryPKs = historyPKs
                };
                //result = new UserService().SaveUser(type, user, rebateType);
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