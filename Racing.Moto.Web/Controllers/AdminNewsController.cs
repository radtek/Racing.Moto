using NLog;
using Racing.Moto.Data.Models;
using Racing.Moto.Services.Caches;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using Racing.Moto.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Controllers
{
    [Authorize]
    public class AdminNewsController : AdminBaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 公告

        public ActionResult Announcement()
        {
            return View();
        }

        public JsonResult GetAnnouncement()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = new PostService().GetPostById(AppConfigCache.News_Annocement);
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

        #region 公告

        public ActionResult Marquee()
        {
            return View();
        }

        public JsonResult GetMarquee()
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
    }
}