using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Racing.Moto.Data.Models;
using Racing.Moto.Data.Entities;

namespace Racing.Moto.Web.SignalR.Hubs
{
    //[HubName("PrevPKTickerHub")]
    public class PrevPKTickerHub : Hub
    {
        //public void Send(string name, string message, string extention)
        //{
        //    // Call the addNewMessageToPage method to update clients.
        //    Clients.All.addNewMessageToPage(name, message, extention);
        //}
        private readonly PrevPKTicker _pkicker;

        public PrevPKTickerHub() : this(PrevPKTicker.Instance) { }

        public PrevPKTickerHub(PrevPKTicker prevPkTicker)
        {
            _pkicker = prevPkTicker;
        }

        public PK GetPK()
        {
            return _pkicker.GetPK();
        }
    }
}