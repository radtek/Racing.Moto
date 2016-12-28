using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Racing.Moto.Data.Models;

namespace Racing.Moto.Web.SignalR.Hubs
{
    //[HubName("PkTickerHub")]
    public class PKTickerHub : Hub
    {
        //public void Send(string name, string message, string extention)
        //{
        //    // Call the addNewMessageToPage method to update clients.
        //    Clients.All.addNewMessageToPage(name, message, extention);
        //}
        private readonly PKTicker _pkicker;

        public PKTickerHub() : this(PKTicker.Instance) { }

        public PKTickerHub(PKTicker pkTicker)
        {
            _pkicker = pkTicker;
        }

        public PKModel GetPKInfo()
        {
            return _pkicker.GetPKInfo();
        }
    }
}