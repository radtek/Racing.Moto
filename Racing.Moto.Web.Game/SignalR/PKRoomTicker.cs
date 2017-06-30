using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
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
        private List<RoomModel> _pkInfo = new List<RoomModel>();

        private readonly object _updatePkInfoLock = new object();


        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);//每2秒钟推送一次
        private readonly Timer _timer;
        private volatile bool _updatingPkInfo = false;

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

            _pkInfo = GetPKRoomInfo();

            _timer = new Timer(UpdatePKInfo, null, _updateInterval, _updateInterval);
        }

        public List<RoomModel> GetPKInfo()
        {
            return _pkInfo;
        }

        public void UpdatePKInfo(object state)
        {
            lock (_updatePkInfoLock)
            {
                if (!_updatingPkInfo)
                {
                    _updatingPkInfo = true;

                    // 获取最新数据
                    _pkInfo = GetPKRoomInfo();

                    BroadcastPkInfo(_pkInfo);

                    _updatingPkInfo = false;
                }
            }
        }

        private List<RoomModel> GetPKRoomInfo()
        {
            var rooms = new List<RoomModel>();

            var users = PKBag.OnlineUserRecorder.GetUserList();

            var roomCount = 3;//初中高三个级别
            var deskCount = 8;  //每个级别(房间) 有 8 个桌子
            for (var i = 1; i <= roomCount; i++)
            {
                var room = new RoomModel { RoomLevel = i, RoomDesks = new List<RoomDeskModel>() };

                for (var j = 1; j <= deskCount; j++)
                {
                    var desk = new RoomDeskModel
                    {
                        RoomDeskId = j,
                        Users = users.Where(u => u.RoomLevel == i && u.DeskID == j).Select(u => new RoomUserModel
                        {
                            UserId = u.UniqueID,
                            UserName = u.UserName
                        }).ToList()
                    };
                    room.RoomDesks.Add(desk);
                }
            }

            return rooms;
        }

        private void BroadcastPkInfo(List<RoomModel> roomModels)
        {
            Clients.All.updatePkInfo(roomModels);
        }
    }
}