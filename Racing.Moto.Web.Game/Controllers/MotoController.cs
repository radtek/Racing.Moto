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
    public class MotoController : BaseController
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
        /// <param name="model">room model</param>
        /// <returns></returns>
        public JsonResult Join(RoomUserModel model)
        {
            var result = new ResponseResult();

            try
            {
                var maxMembers = 10;    //最多人数

                var memberCount = PKBag.OnlineUserRecorder.GetUserList().Where(u => u.RoomLevel == model.RoomLevel && u.DeskNo == model.DeskNo).Count();
                if (memberCount == maxMembers)
                {
                    result.Success = false;
                    result.Message = "房间已满";
                }
                else
                {
                    var user = PKBag.OnlineUserRecorder.GetUser(PKBag.LoginUser.UserName);
                    user.RoomLevel = model.RoomLevel;
                    user.DeskNo = model.DeskNo;
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
    }
}