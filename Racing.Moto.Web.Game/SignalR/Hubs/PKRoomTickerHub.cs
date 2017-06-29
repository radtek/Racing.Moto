using Microsoft.AspNet.SignalR;
using Racing.Moto.Game.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.Web.Game.SignalR.Hubs
{
    public class PKRoomTickerHub : Hub
    {
        private readonly PKRoomTicker _pkicker;

        public PKRoomTickerHub() : this(PKRoomTicker.Instance) { }

        public PKRoomTickerHub(PKRoomTicker pkTicker)
        {
            _pkicker = pkTicker;
        }

        public List<RoomModel> GetPKInfo()
        {
            return _pkicker.GetPKInfo();
        }
    }
}