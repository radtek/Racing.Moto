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
        public ActionResult Rate()
        {
            return View();
        }

        public JsonResult GetRates()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new RateService().GetAll();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        public JsonResult SaveRates(List<Rate> rates)
        {
            var result = new ResponseResult();

            try
            {
                new RateService().UpdateRates(rates);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        public JsonResult SaveBatch(BatchRateType type, decimal rate)
        {
            var result = new ResponseResult();

            try
            {
                //new RateService().UpdateRates(rates);
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