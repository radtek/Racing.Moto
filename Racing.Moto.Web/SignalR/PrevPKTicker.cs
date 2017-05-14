using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Web.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Racing.Moto.Web.SignalR
{
    public class PrevPKTicker
    {
        // Singleton instance
        private readonly static Lazy<PrevPKTicker> _instance = new Lazy<PrevPKTicker>(() => new PrevPKTicker(GlobalHost.ConnectionManager.GetHubContext<PrevPKTickerHub>().Clients));
        //private readonly ConcurrentDictionary<string, Stock> _pkInfo = new ConcurrentDictionary<string, Stock>();
        private PK _pk = new PK();

        private readonly object _updatePKLock = new object();


        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(5);//每5秒钟推送一次
        private readonly Timer _timer;
        private volatile bool _updatingPK = false;

        #region property
        public static PrevPKTicker Instance
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

        private PrevPKTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _pk = GetPrevPK();
            
            _timer = new Timer(UpdatePK, null, _updateInterval, _updateInterval);
        }

        public PK GetPK()
        {
            return _pk;
        }

        public void UpdatePK(object state)
        {
            lock (_updatePKLock)
            {
                if (!_updatingPK)
                {
                    _updatingPK = true;

                    // 获取最新数据
                    _pk = GetPrevPK();

                    BroadcastPK(_pk);

                    _updatingPK = false;
                }
            }
        }

        private PK GetPrevPK()
        {
            var pkService = new PKService();

            var pk = pkService.GetPrevPKResult();

            return pk;
        }

        private void BroadcastPK(PK pk)
        {
            Clients.All.updatePK(pk);
        }
    }
}