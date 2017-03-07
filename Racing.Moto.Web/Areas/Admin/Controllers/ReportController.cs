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

        private ReportSearchModel GetReportSearchModelFromUrl(int? parentUserId)
        {
            var model = new ReportSearchModel()
            {
                BetType = GetIntQueryString("BetType"),
                SearchType = int.Parse(Request.QueryString["SearchType"]),
                ReportType = int.Parse(Request.QueryString["ReportType"]),
                SettlementType = int.Parse(Request.QueryString["SettlementType"]),
                FromDate = GetDateTimeQueryString("FromDate"),
                ToDate = GetDateTimeQueryString("ToDate"),
                PageIndex = int.Parse(Request.QueryString["PageIndex"]),
                PageSize = int.Parse(Request.QueryString["PageSize"]),
                ParentUserId = parentUserId
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

        #region 总代理列表

        /// <summary>
        /// 总代理列表
        /// </summary>
        public ActionResult GeneralAgent()
        {
            var model = GetReportSearchModelFromUrl(null);
            model.UserType = UserType.Admin;//当前用户是管理员

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);
            ViewBag.QueryString = Request.Url.Query;

            return View();
        }

        #endregion

        #region 代理列表

        /// <summary>
        /// 代理列表
        /// </summary>
        /// <param name="id">上级总代理UserId</param>
        /// <returns></returns>
        public ActionResult Agent(int id)
        {
            var model = GetReportSearchModelFromUrl(id);
            model.UserType = UserType.GeneralAgent;//当前用户是总代理

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);
            ViewBag.QueryString = Request.Url.Query;
            ViewBag.ParentUser = new UserService().GetUser(id);

            return View();
        }

        #endregion

        #region 会员列表

        /// <summary>
        /// 会员列表
        /// </summary>
        /// <param name="id">上级代理UserId</param>
        /// <returns></returns>
        public ActionResult Member(int id)
        {
            var model = GetReportSearchModelFromUrl(id);
            model.UserType = UserType.Agent;//当前用户是代理

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);
            ViewBag.QueryString = Request.Url.Query;
            ViewBag.ParentUser = new UserService().GetUser(id);

            return View();
        }

        #endregion


        [HttpPost]
        public JsonResult SearchReport(ReportSearchModel model)
        {
            var result = new ResponseResult();

            try
            {
                //if (model.UserType == RoleConst.Role_Id_Admin || model.UserType == RoleConst.Role_Id_General_Agent)
                //{
                //    result.Data = new ReportService().GetAgentReports(model);
                //}
                //else if (model.UserType == RoleConst.Role_Id_Agent)
                //{
                //    result.Data = new ReportService().GetAgentReports(model);
                //}
                result.Data = new ReportService().GetAgentReports(model);
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

        /// <summary>
        /// 下注明细
        /// </summary>
        /// <returns></returns>
        public ActionResult Bets()
        {
            return View();
        }

    }
}