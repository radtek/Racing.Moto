using Newtonsoft.Json;
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

namespace Racing.Moto.Web.Admin.Controllers
{
    public class ReportController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region Report search
        public ActionResult Index()
        {
            ViewBag.LoginUserId = LoginUser.UserId;
            ViewBag.UserType = LoginUser.UserRoles.First().RoleId;

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

        private ReportSearchModel GetReportSearchModelFromUrl(int? parentUserId, int? userId)
        {
            var model = new ReportSearchModel()
            {
                PKId = GetIntQueryString("PKId"),
                BetType = GetIntQueryString("BetType"),
                SearchType = int.Parse(Request.QueryString["SearchType"]),
                ReportType = int.Parse(Request.QueryString["ReportType"]),
                SettlementType = int.Parse(Request.QueryString["SettlementType"]),
                FromDate = GetDateTimeQueryString("FromDate"),
                ToDate = GetDateTimeQueryString("ToDate"),
                PageIndex = int.Parse(Request.QueryString["PageIndex"]),
                PageSize = int.Parse(Request.QueryString["PageSize"]),
                ParentUserId = parentUserId,
                UserId = userId
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
        public ActionResult GeneralAgent(int? id)
        {
            //总代理的父级一定是管理员, 既登录用户
            var model = GetReportSearchModelFromUrl(LoginUser.UserId, null);
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
            var model = GetReportSearchModelFromUrl(id, null);
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
            var model = GetReportSearchModelFromUrl(id, null);
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

                if (model.ReportType == 1)
                {
                    // 交收報表
                    result.Data = new ReportService().GetAgentReports(model);
                }
                else
                {
                    // 分类報表
                    result.Data = new ReportService().GetRankReports(model);
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

        #region 下注明细

        /// <summary>
        /// 下注明细
        /// <param name="id">当前会员UserId</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult Bets(int id)
        {
            var model = GetReportSearchModelFromUrl(null, id);

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);
            ViewBag.QueryString = Request.Url.Query;

            return View();
        }

        [HttpPost]
        public JsonResult GetBets(ReportSearchModel model)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new ReportService().GetUserBetReports(model);
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


        #region 分类报表

        public ActionResult Rank()
        {
            var model = GetReportSearchModelFromUrl(null, null);

            ViewBag.SearchParams = JsonConvert.SerializeObject(model);
            ViewBag.QueryString = Request.Url.Query;

            return View();
        }

        #endregion
    }
}