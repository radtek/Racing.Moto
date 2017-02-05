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

namespace Racing.Moto.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class PKController : AdminBaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 盘口设置
        public ActionResult Rate(int id = 0)
        {
            // 0:竞技场, 1: 娱乐场a, 2: 娱乐场b, 3: 娱乐场c
            ViewBag.RateType = id;

            return View();
        }

        public JsonResult GetRates(RateType type)
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new RateService().GetRatesByType(type);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        public JsonResult SaveRates(RateType type, List<Rate> rates)
        {
            var result = new ResponseResult();

            try
            {
                new RateService().UpdateRates(type, rates);
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
        /// 批量修改赔率
        /// </summary>
        /// <param name="type">0:竞技场, 1: 娱乐场a, 2: 娱乐场b, 3: 娱乐场c</param>
        /// <param name="batchType">1:名次，2:大小，3:单双，4:全部</param>
        /// <param name="rate">赔率</param>
        public JsonResult SaveBatch(RateType type, BatchRateType batchType, decimal rate)
        {
            var result = new ResponseResult();

            try
            {
                new RateService().UpdateBatchRates(type, batchType, rate);
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

        #region 注单查询

        public ActionResult Bet()
        {
            return View();
        }

        #endregion

        #region 历史开奖
        public ActionResult History()
        {
            return View();
        }

        #endregion

        #region 站内消息

        #region 公告

        public ActionResult NewsAnnouncement()
        {
            return View();
        }

        public JsonResult GetNewsAnnouncement()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new PostService().GetPostById(AppConfigCache.News_Announcement);
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

        public JsonResult SaveNews(Data.Entities.Post post)
        {
            var result = new ResponseResult();

            try
            {
                new PostService().SavePost(post);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        #region 跑马灯

        public ActionResult NewsMarquee()
        {
            return View();
        }

        public JsonResult GetNewsMarquee()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new PostService().GetPostById(AppConfigCache.News_Marquee);
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

        #endregion
    }
}