using App.Core.OnlineStat;
using NLog;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Models;
using Racing.Moto.Game.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Racing.Moto.Game.Web.ApiControllers
{
    public class OnlineUserController : ApiController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 取盘口登录用户
        /// </summary>
        [HttpPost]
        public ResponseResult GetOnlineUsers()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = PKBag.OnlineUserRecorder.GetUserList();
            }
            catch (Exception ex)
            {
                _logger.Info(ex);

                result.Success = false;
                result.Message = MessageConst.System_Error;
            }

            return result;
        }

        /// <summary>
        /// 踢出口登录用户
        /// </summary>
        [HttpPost]
        public ResponseResult KickOutUser(OnlineUser user)
        {
            var result = new ResponseResult();

            try
            {
                var onlineUser = PKBag.OnlineUserRecorder.GetUser(user.UserName);
                PKBag.OnlineUserRecorder.Delete(onlineUser);
            }
            catch (Exception ex)
            {
                _logger.Info(ex);

                result.Success = false;
                result.Message = MessageConst.System_Error;
            }

            return result;
        }
    }
}
