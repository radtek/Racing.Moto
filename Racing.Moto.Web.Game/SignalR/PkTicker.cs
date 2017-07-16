using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Racing.Moto.Game.Data.Models;
using Racing.Moto.Game.Data.Services;
using Racing.Moto.Game.Web.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Racing.Moto.Game.Web.SignalR
{
    public class PKTicker
    {
        // Singleton instance
        private readonly static Lazy<PKTicker> _instance = new Lazy<PKTicker>(() => new PKTicker(GlobalHost.ConnectionManager.GetHubContext<PKTickerHub>().Clients));
        //private readonly ConcurrentDictionary<string, Stock> _pkInfo = new ConcurrentDictionary<string, Stock>();
        private PKInfoModel _pkInfo = new PKInfoModel();

        private readonly object _updatePkInfoLock = new object();


        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);//每2秒钟推送一次
        private readonly Timer _timer;
        private volatile bool _updatingPkInfo = false;

        #region property
        public static PKTicker Instance
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

        private PKTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _pkInfo = GetCurrentPKInfo();
            
            _timer = new Timer(UpdatePKInfo, null, _updateInterval, _updateInterval);
        }

        public PKInfoModel GetPKInfo()
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
                    _pkInfo = GetCurrentPKInfo();

                    BroadcastPkInfo(_pkInfo);

                    _updatingPkInfo = false;
                }
            }
        }

        private PKInfoModel GetCurrentPKInfo()
        {
            var pkService = new PKService();

            var info = pkService.GetCurrentPKModel();

            return info;
        }

        private void BroadcastPkInfo(PKInfoModel pkModel)
        {
            Clients.All.updatePkInfo(pkModel);
        }
    }
}