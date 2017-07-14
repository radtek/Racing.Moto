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
        private int _minDummyUserId = 10000000;

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

        #region 生成 虚拟用户
        /// <summary>
        /// 生成 虚拟用户
        /// </summary>
        [HttpPost]
        public ResponseResult GenerateDummyUsers()
        {
            var result = new ResponseResult();

            try
            {
                //var onlineUsers = PKBag.OnlineUserRecorder.GetUserList();

                for (var roomLevel = 1; roomLevel <= 3; roomLevel++)
                {
                    //var roomUsers = PKBag.OnlineUserRecorder.GetUserList().Where(u => u.RoomLevel == roomLevel).ToList();//初中高级场

                    for (var deskNo = 1; deskNo <= 8; deskNo++)
                    {
                        var deskUsers = PKBag.OnlineUserRecorder.GetUserList().Where(r => r.RoomLevel == roomLevel && r.DeskNo == deskNo).ToList();    //桌

                        var num = GetRandom(1, 10);// 随机人数
                        if (num > deskUsers.Count)
                        {
                            // 添加虚拟用户
                            var count = num - deskUsers.Count;
                            if (count > 2)
                            {
                                count = 2;//每次最多2人
                            }
                            for (var c = 0; c < count; c++)
                            {
                                AddDummyOnlineUser(roomLevel, deskNo);
                            }
                        }
                        else
                        {
                            // 随机踢出一个虚拟用户
                            KickoutDummyUser(roomLevel, deskNo);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {

            }
            catch (Exception ex)
            {
                _logger.Info(ex);

                result.Success = false;
                result.Message = MessageConst.System_Error;
            }

            return result;
        }

        private void AddDummyOnlineUser(int roomLevel, int deskNo)
        {
            OnlineUser onlineUser = new OnlineUser();

            onlineUser.UniqueID = OnlineHttpModule.GetDummyUniqueID(_minDummyUserId);
            // 用户名称                                                        
            onlineUser.UserName = Guid.NewGuid().ToString("N");
            // 用户头像  
            onlineUser.Avatar = GetRandomAvatar();

            onlineUser.RoomLevel = roomLevel;
            onlineUser.DeskNo = deskNo;
            onlineUser.Num = OnlineHttpModule.GetMinMotoNum(roomLevel, deskNo);

            // 保存用户信息
            OnlineHttpModule.AddOnlineUser(onlineUser);
        }

        private void KickoutDummyUser(int roomLevel, int deskNo)
        {
            try
            {
                var dummyUsers = PKBag.OnlineUserRecorder.GetUserList().Where(u => u.RoomLevel == roomLevel && u.DeskNo == deskNo && u.UniqueID >= _minDummyUserId).ToList();
                if (dummyUsers.Count > 0)
                {
                    var random = GetRandom(0, dummyUsers.Count - 1);
                    PKBag.OnlineUserRecorder.Delete(dummyUsers[random]);
                }
            }
            catch (InvalidOperationException)
            {

            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        private string GetRandomAvatar()
        {
            var num = GetRandom(1, 17);

            var avatar = string.Format("/img/avatars/user{0}.jpg", num);

            return avatar;
        }

        private int GetRandom(int min, int max)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            var num = r.Next(min, max);
            return num;
        }
        #endregion
    }
}
