using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Racing.Moto.Web.SignalR.Hubs
{
    //[HubName("PkTickerHub")]
    public class PkTickerHub : Hub
    {
        //public void Send(string name, string message, string extention)
        //{
        //    // Call the addNewMessageToPage method to update clients.
        //    Clients.All.addNewMessageToPage(name, message, extention);
        //}
        private readonly PkTicker _pkicker;

        public PkTickerHub() : this(PkTicker.Instance) { }

        public PkTickerHub(PkTicker pkTicker)
        {
            _pkicker = pkTicker;
        }

        public List<int> GetPkInfo()
        {
            return _pkicker.GetPkInfo();
        }
    }
}