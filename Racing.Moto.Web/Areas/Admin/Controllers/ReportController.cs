using Newtonsoft.Json;
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

        #region Report search
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

        #region 交收报表

        private ReportSearchModel GetReportSearchModelFromUrl()
        {
            var model = new ReportSearchModel()
            {
                BetType = GetIntQueryString("BetType"),
                SearchType = int.Parse(Request.QueryString["SearchType"]),
                ReportType = int.Parse(Request.QueryString["ReportType"]),
                SettlementType = int.Parse(Request.QueryString["SettlementType"]),
                FromDate = GetDateTimeQueryString("FromDate"),
                ToDate = GetDateTimeQueryString("ToDate"),
            };

            return model;
        }

        private int? GetIntQueryString(string paramName)
        {
            return !string.IsNullOrEmpty(Request.QueryString[paramName]) ? (int?)int.Parse(Request.QueryString[paramName]) : null;
        }

        private DateTime? GetDateTimeQueryString(string paramName)
        {
            return !string.IsNullOrEmpty(Request.QueryString[paramName]) ? (DateTime?)DateTime.Parse(Request.QueryString[paramName]) : null;
        }

        #region 总代理

        public ActionResult GeneralAgent(int id)
        {
            var model = GetReportSearchModelFromUrl();
            model.UserType = UserType.Admin;

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);

            return View();
        }

        #endregion


        #region 代理

        public ActionResult Agent(int id)
        {
            var model = GetReportSearchModelFromUrl();
            model.UserType = UserType.GeneralAgent;

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);

            return View();
        }

        #endregion


        #region 会员

        public ActionResult Member(int id)
        {
            var model = GetReportSearchModelFromUrl();
            model.UserType = UserType.Agent;

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);

            return View();
        }

        #endregion



        [HttpPost]
        public JsonResult SearchReport(ReportSearchModel model)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new PKBonusService().GetBonusReport(model);
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