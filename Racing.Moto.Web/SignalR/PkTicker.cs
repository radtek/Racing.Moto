using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Racing.Moto.Data.Models;
using Racing.Moto.Data.Services;
using Racing.Moto.Web.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Racing.Moto.Web.SignalR
{
    public class PKTicker
    {
        // Singleton instance
        private readonly static Lazy<PKTicker> _instance = new Lazy<PKTicker>(() => new PKTicker(GlobalHost.ConnectionManager.GetHubContext<PKTickerHub>().Clients));
        //private readonly ConcurrentDictionary<string, Stock> _pkInfo = new ConcurrentDictionary<string, Stock>();
        private PKModel _pkInfo = new PKModel();

        private readonly object _updatePkInfoLock = new object();


        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(5);//每5秒钟推送一次
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

        public PKModel GetPKInfo()
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

        private PKModel GetCurrentPKInfo()
        {
            //var pkService = new PKService();

            //var info = pkService.GetCurrentPKModel();
            
            //return info;
            return new PKModel();
        }

        //private List<int> GetPkInfo(int len)
        //{
        //    var info = new List<int>();

        //    int rep = 1;
        //    int num = 0;
        //    long num2 = DateTime.Now.Ticks + rep;
        //    rep++;
        //    Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
        //    for (int i = 0; i < len; i++)
        //    {
        //        num = Convert.ToInt32((char)(0x30 + ((ushort)(random.Next() % 10))));

        //        info.Add(num);
        //    }

        //    return info;
        //}

        private void BroadcastPkInfo(PKModel pkModel)
        {
            Clients.All.updatePkInfo(pkModel);
        }
    }
}