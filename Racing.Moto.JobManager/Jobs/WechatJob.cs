using Newtonsoft.Json;
using NLog;
using Quartz;
using Racing.Moto.Core.Utils;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.JobManager.Jobs
{
    /*
     *   DisallowConcurrentExecution
         禁止并发执行多个相同定义的JobDetail, 
         这个注解是加在Job类上的, 但意思并不是不能同时执行多个Job, 而是不能并发执行同一个Job Definition(由JobDetail定义), 但是可以同时执行多个不同的JobDetail
    */
    [DisallowConcurrentExecution]
    public class WechatJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 1. 开奖结果: 回调微信端接口, 返回开奖结果
        /// 2. 中奖结果: 回调微信端接口, 返回下单中奖结果
        /// 每隔5秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "[WechatJob] Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(startInfo);
                _logger.Info(startInfo);

                Run();
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        public void Run()
        {
            try
            {
                // 1. 开奖结果: 回调微信端接口, 返回开奖结果
                SyncRanks();

                // 2. 中奖结果: 回调微信端接口, 返回下单中奖结果
                SyncBonus();
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        /// <summary>
        /// 开奖结果: 回调微信端接口, 返回开奖结果
        /// </summary>
        private void SyncRanks()
        {
            try
            {
                var pkService = new PKService();

                var wechatWebUrl = System.Configuration.ConfigurationManager.AppSettings["WechatWebUrl"];

                var pks = pkService.GetRanksNotSyncedPKs();

                foreach (var pk in pks)
                {
                    // 当前时间超过开奖时间
                    if (pk.EndTime.AddSeconds(pk.LotterySeconds) <= DateTime.Now)
                    {
                        RestClient client = new RestClient(wechatWebUrl);
                        var request = new RestRequest("/api/Wechat/SyncRanks", Method.POST);
                        request.AddJsonBody(new
                        {
                            ID = pk.PKId,
                            Ranks = pk.Ranks
                        });

                        var response = client.Execute(request);

                        if (response != null && !string.IsNullOrEmpty(response.Content))
                        {
                            var result = JsonConvert.DeserializeObject<ResponseResult>(response.Content);

                            if (!result.Success)
                            {
                                _logger.Info(response.Content);
                            }
                            else
                            {
                                pkService.UpdateIsRanksSynced(pk.PKId, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        /// <summary>
        /// 2. 中奖结果: 回调微信端接口, 返回下单中奖结果
        /// </summary>
        private void SyncBonus()
        {
            try
            {
                var betItemService = new BetItemService();
                var pkBonusService = new PKBonusService();

                var wechatWebUrl = System.Configuration.ConfigurationManager.AppSettings["WechatWebUrl"];

                var dbBetItems = betItemService.GetNotSyncedBetItems();

                var orderNos = dbBetItems.GroupBy(b => b.OrderNo).Select(g => g.Key).ToList();


                foreach (var orderNo in orderNos)
                {
                    // 当前世界超过开奖时间
                    var betIds = dbBetItems.Where(b => b.OrderNo == orderNo).Select(b => b.BetId).ToList();
                    var amount = pkBonusService.GetAmountByBetIds(betIds);

                    RestClient client = new RestClient(wechatWebUrl);
                    var request = new RestRequest("/api/Wechat/SyncBonus", Method.POST);
                    request.AddJsonBody(new
                    {
                        OrderNo = orderNo,
                        Amount = amount
                    });

                    var response = client.Execute(request);

                    if (response != null && !string.IsNullOrEmpty(response.Content))
                    {
                        var result = JsonConvert.DeserializeObject<ResponseResult>(response.Content);

                        if (!result.Success)
                        {
                            _logger.Info(response.Content);
                        }
                        else
                        {
                            betItemService.UpdateIsSynced(orderNo, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

    }
}