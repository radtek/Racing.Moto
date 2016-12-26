using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Racing.Moto.Web.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Racing.Moto.Web.SignalR
{
    public class PkTicker
    {
        // Singleton instance
        private readonly static Lazy<PkTicker> _instance = new Lazy<PkTicker>(() => new PkTicker(GlobalHost.ConnectionManager.GetHubContext<PkTickerHub>().Clients));
        //private readonly ConcurrentDictionary<string, Stock> _pkInfo = new ConcurrentDictionary<string, Stock>();
        private List<int> _pkInfo = new List<int>();

        private readonly object _updatePkInfoLock = new object();


        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(5);
        private readonly Timer _timer;
        private volatile bool _updatingPkInfo = false;

        #region property
        public static PkTicker Instance
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

        private PkTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _pkInfo.Clear();

            _pkInfo = GetRandomPkInfo(10);

            //_stocks.Clear();
            //var stocks = new List<Stock>
            //{
            //    new Stock { Symbol = "MSFT", Price = 30.31m },
            //    new Stock { Symbol = "APPL", Price = 578.18m },
            //    new Stock { Symbol = "GOOG", Price = 570.30m }
            //};
            //stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));

            _timer = new Timer(UpdatePkInfo, null, _updateInterval, _updateInterval);

        }

        public List<int> GetPkInfo()
        {
            return _pkInfo;
        }

        public void UpdatePkInfo(object state)
        {
            lock (_updatePkInfoLock)
            {
                if (!_updatingPkInfo)
                {
                    _updatingPkInfo = true;

                    // 获取最新数据
                    _pkInfo = GetRandomPkInfo(10);

                    BroadcastPkInfo(_pkInfo);

                    _updatingPkInfo = false;
                }
            }
        }

        private List<int> GetRandomPkInfo(int len)
        {
            var info = new List<int>();

            int rep = 1;
            int num = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < len; i++)
            {
                num = Convert.ToInt32((char)(0x30 + ((ushort)(random.Next() % 10))));

                info.Add(num);
            }

            return info;
        }

        private void BroadcastPkInfo(List<int> order)
        {
            Clients.All.updatePkInfo(order);
        }
    }
}