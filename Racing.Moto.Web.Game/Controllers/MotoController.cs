using NLog;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Models;
using Racing.Moto.Game.Data.Services;
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

        #region 房间
        public ActionResult Room(int id)
        {
            ViewBag.RoomId = id;
            return View();
        }

        /// <summary>
        /// 取用户余额
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBalance()
        {
            var result = new ResponseResult();

            try
            {
                var balance = new UserService().GetBalance(PKBag.LoginUser.UserName);

                result.Data = balance;
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
        /// 进入房间
        /// </summary>
        /// <param name="model">room model</param>
        /// <returns></returns>
        public JsonResult Join(RoomUserModel model)
        {
            var result = new ResponseResult();

            try
            {
                var user = PKBag.OnlineUserRecorder.GetUser(PKBag.LoginUser.UserName);

                if (user.RoomLevel == model.RoomLevel && user.DeskNo == model.DeskNo)
                {
                    //已经进入的房间
                }
                else
                {
                    var maxMembers = 10;    //最多人数
                    var deskUsers = PKBag.OnlineUserRecorder.GetUsers(model.RoomLevel, model.DeskNo);
                    var memberCount = deskUsers.Count();
                    if (memberCount == maxMembers)
                    {
                        result.Success = false;
                        result.Message = "房间已满";
                    }
                    else
                    {
                        user.RoomLevel = model.RoomLevel;
                        user.DeskNo = model.DeskNo;

                        // 取最小的 还未在房间中 使用的车号
                        var motoNums = OnlineHttpModule.GetMinMotoNum(model.RoomLevel, model.DeskNo);
                        user.Num = motoNums;
                    }
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


        /// <summary>
        /// 退出房间
        /// </summary>
        /// <returns></returns>
        public JsonResult Exit()
        {
            var result = new ResponseResult();

            try
            {
                var user = PKBag.OnlineUserRecorder.GetUser(PKBag.LoginUser.UserName);
                user.RoomLevel = 0;
                user.DeskNo = 0;
                user.Num = 0;
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
        /// 竞技场
        /// </summary>
        /// <param name="id">room id: 初中高级场</param>
        /// <param name="cid">desk no: 桌号</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Arena(int id, int cid)
        {
            ViewBag.RoomId = id;
            ViewBag.DeskNo = cid;

            if (PKBag.LoginUser != null)
            {
                var user = PKBag.OnlineUserRecorder.GetUser(PKBag.LoginUser.UserName);
                if (user.Num > 0)
                {
                    ViewBag.MyMotoNum = user.Num;
                }
            }

            return View();
        }
    }
}