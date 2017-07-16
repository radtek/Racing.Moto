using App.Core.OnlineStat;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Models;
using Racing.Moto.Game.Web.Mvc;
using Racing.Moto.Game.Web.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Racing.Moto.Game.Web.SignalR
{
    public class PKRoomTicker
    {
        // Singleton instance
        private readonly static Lazy<PKRoomTicker> _instance = new Lazy<PKRoomTicker>(() => new PKRoomTicker(GlobalHost.ConnectionManager.GetHubContext<PKRoomTickerHub>().Clients));
        //private readonly ConcurrentDictionary<string, Stock> _pkInfo = new ConcurrentDictionary<string, Stock>();
        private HttpContext _contextCurrent;
        private List<RoomModel> _pkRoomInfo = new List<RoomModel>();

        private readonly object _updatePKRoomInfoLock = new object();


        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);//每2秒钟推送一次
        private readonly Timer _timer;
        private volatile bool _updatingPKRoomInfo = false;

        #region property
        public static PKRoomTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }
        #endregion

        private PKRoomTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _pkRoomInfo = GetPKRoomInfoFromSession();

            _timer = new Timer(UpdatePKRoomInfo, null, _updateInterval, _updateInterval);
        }

        public List<RoomModel> GetPKRoomInfo(HttpContext context)
        {
            _contextCurrent = context;

            return _pkRoomInfo;
        }

        public void UpdatePKRoomInfo(object state)
        {
            lock (_updatePKRoomInfoLock)
            {
                if (!_updatingPKRoomInfo)
                {
                    _updatingPKRoomInfo = true;

                    // 获取最新数据
                    _pkRoomInfo = GetPKRoomInfoFromSession();

                    BroadcastPKRoomInfo(_pkRoomInfo);

                    _updatingPKRoomInfo = false;
                }
            }
        }

        private List<RoomModel> GetPKRoomInfoFromSession()
        {
            var rooms = new List<RoomModel>();

            var onlineUserRecorder = PKBag.OnlineUserRecorder != null
                ? PKBag.OnlineUserRecorder
                : _contextCurrent != null ? _contextCurrent.Cache[SessionConst.OnlineUserRecorderCacheKey] as OnlineUserRecorder : null;

            if (onlineUserRecorder == null)
            {
                return rooms;
            }

            //var users = onlineUserRecorder.GetUserList();

            var roomCount = 3;//初中高三个级别
            var deskCount = 8;  //每个级别(房间) 有 8 个桌子
            for (var i = 1; i <= roomCount; i++)
            {
                var room = new RoomModel { RoomLevel = i, RoomDesks = new List<RoomDeskModel>() };

                for (var j = 1; j <= deskCount; j++)
                {
                    var desk = new RoomDeskModel
                    {
                        RoomLevel = i,
                        RoomDeskId = j,
                        Users = onlineUserRecorder.GetUsers(i, j).Select(u => new RoomUserModel
                        {
                            UserId = u.UniqueID,
                            UserName = u.UserName,
                            Avatar = u.Avatar,
                            Num = u.Num
                        }).ToList()
                    };
                    room.RoomDesks.Add(desk);
                }

                rooms.Add(room);
            }

            return rooms;
        }

        private void BroadcastPKRoomInfo(List<RoomModel> roomModels)
        {
            Clients.All.updatePkRoomInfo(roomModels);
        }
    }
}