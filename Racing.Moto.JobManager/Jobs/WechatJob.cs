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
                var wechatWebPath = System.Configuration.ConfigurationManager.AppSettings["WechatWebPath"];

                var pks = pkService.GetRanksNotSyncedPKs();

                foreach (var pk in pks)
                {
                    // 当前时间超过开奖时间
                    if (pk.EndTime.AddSeconds(pk.LotterySeconds) <= DateTime.Now)
                    {
                        RestClient client = new RestClient(wechatWebUrl);

                        var resource = string.Format("/{0}/issuereturn?issue={1}&result={2}", wechatWebPath, pk.PKId, pk.Ranks);
                        var request = new RestRequest(resource, Method.POST);
                        //request.AddJsonBody(new
                        //{
                        //    ID = pk.PKId,
                        //    Ranks = pk.Ranks
                        //});

                        var response = client.Execute(request);

                        if (response != null && !string.IsNullOrEmpty(response.Content))
                        {
                            //var result = JsonConvert.DeserializeObject<ResponseResult>(response.Content);
                            dynamic res = JsonConvert.DeserializeObject(response.Content);

                            /*
                                result: 
                                    0：成功
                                    1：失败
                             */
                            if (res.result.ToString() == "1")
                            {
                                _logger.Info("[SyncRanks Failed] [" + resource + "] " + response.Content);
                            }
                            else
                            {
                                _logger.Info("[SyncRanks Success] [" + resource + "] " + response.Content);

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

        #region 中奖结果
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
                var wechatWebPath = System.Configuration.ConfigurationManager.AppSettings["WechatWebPath"];

                //wechat订单
                var orderNos = betItemService.GetNotSyncedOrderNos();
                
                foreach (var orderNo in orderNos)
                {
                    // 按wechat订单生成奖金+退水
                    var amount = GetBonusAndRebateAmount(orderNo);

                    RestClient client = new RestClient(wechatWebUrl);
                    var resource = string.Format("/{0}/orderreturn?orderId={1}&score={2}", wechatWebPath, orderNo, amount);
                    var request = new RestRequest(resource, Method.POST);
                    //request.AddJsonBody(new
                    //{
                    //    OrderNo = orderNo,
                    //    Amount = amount
                    //});

                    var response = client.Execute(request);

                    if (response != null && !string.IsNullOrEmpty(response.Content))
                    {
                        dynamic res = JsonConvert.DeserializeObject(response.Content);

                        /*
                            result: 
                                0：成功
                                1：失败
                         */
                        if (res.result.ToString() == "1")
                        {
                            _logger.Info("[SyncBonus Failed] [" + resource + "] " + response.Content);
                        }
                        else
                        {
                            _logger.Info("[SyncBonus Success] [" + resource + "] " + response.Content);

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

        /// <summary>
        /// 按wechat单号生成奖金+退水
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public decimal GetBonusAndRebateAmount(long orderNo)
        {
            var rebateAmount = 0M;  //退水
            var bonusAmount = 0M;   //奖金

            var dbBetItems = new BetItemService().GetBetItemsByOrderNo(orderNo);

            // 计算退水
            var userIds = dbBetItems.Select(bi => bi.Bet.UserId).Distinct().ToList();
            var users = new UserService().GetUsers(userIds);
            var userRebates = new UserRebateService().GetUserRebates(userIds);
            foreach (var userId in userIds)
            {
                var user = users.Where(u => u.UserId == userId).First();

                var userBetItems = dbBetItems.Where(bi => bi.Bet.UserId == userId).ToList();
                foreach (var userBetItem in userBetItems)
                {
                    var userRebate = userRebates.Where(e => e.UserId == userId && e.RebateNo == userBetItem.Num).FirstOrDefault();
                    if (userRebate != null)
                    {
                        var rebate = UserRebateService.GetDefaultRebate(userRebate, user.DefaultRebateType);

                        rebateAmount += Math.Round(userBetItem.Amount * rebate, 4);
                    }
                }
            }

            // 计算奖金
            var betService = new BetService();
            var pkIds = dbBetItems.Select(bi => bi.Bet.PKId).Distinct().ToList();
            var pks = new PKService().GetPKs(pkIds);
            var dbPKRates = new PKRateService().GetPKRates(pkIds);
            foreach (var pk in pks)
            {
                var bets = betService.ConvertRanksToBets(pk.Ranks);
                foreach (var bet in bets)
                {
                    var pkRate = dbPKRates.Where(r => r.PKId == pk.PKId && r.Rank == bet.Rank && r.Num == bet.Num).First();

                    var betAmount = dbBetItems.Where(bi => bi.Bet.PKId == pk.PKId && bi.Rank == bet.Rank && bi.Num == bet.Num).Sum(bi => (decimal?)bi.Amount ?? 0);

                    bonusAmount += Math.Round(betAmount * pkRate.Rate, 4);
                }
            }

            return rebateAmount + bonusAmount;
        }

        #endregion

    }
}