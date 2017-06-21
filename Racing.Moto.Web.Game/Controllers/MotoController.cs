using NLog;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Models;
using Racing.Moto.Game.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Game.Controllers
{
    public class MotoController : Controller
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        // 房间
        public ActionResult Room(int id)
        {
            return View();
        }

        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="id">roomId</param>
        /// <returns></returns>
        public JsonResult Join(int id)
        {
            var result = new ResponseResult();

            try
            {
                var user = PKBag.OnlineUserRecorder.GetUser(PKBag.LoginUser.UserName);
                user.RoomID = id;
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