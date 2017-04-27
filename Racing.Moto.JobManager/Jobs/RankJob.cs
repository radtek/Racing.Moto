using NLog;
using Quartz;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
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
    public class RankJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 计算名次, 生成奖金
        /// 每隔3秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "RankJob Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(startInfo);
                _logger.Info(startInfo);

                //var pkService = new PKService();
                //var bonusService = new PKBonusService();

                ////var pk = pkService.GetCurrentPK();

                //var pks = pkService.GetNotCalculatePKs();

                //foreach (var pk in pks)
                //{
                //    if (pk != null && string.IsNullOrEmpty(pk.Ranks))
                //    {
                //        var now = DateTime.Now;
                //        if (now >= pk.BeginTime.AddSeconds(pk.OpeningSeconds))// 封盘
                //        {
                //            // 计算名次
                //            var rankList = new BetService().CalculateRanks(pk.PKId);
                //            var ranks = string.Join(",", rankList);
                //            pkService.UpdateRanks(pk.PKId, ranks);

                //            var msg = string.Format("Calculate Ranks - PKId : {0} - Ranks : {1} - Time : {2}", pk.PKId, ranks, now.ToString(DateFormatConst.yMd_Hms));
                //            _logger.Info(msg);

                //            // 生成奖金
                //            if (!pk.IsBonused)
                //            {
                //                // 更新 奖金生成标志, 防止多次计算
                //                pkService.UpdateIsBonused(pk.PKId, true);
                //                // 生成奖金
                //                bonusService.GenerateBonus(pk);
                //            }
                //        }
                //    }
                //}

                Run();
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        public void Run()
        {
            var pkService = new PKService();
            var bonusService = new PKBonusService();
            var betService = new BetService();

            //var pk = pkService.GetCurrentPK();

            var pks = pkService.GetNotCalculatePKs();

            foreach (var pk in pks)
            {
                if (pk != null && string.IsNullOrEmpty(pk.Ranks))
                {
                    var now = DateTime.Now;
                    if (now >= pk.BeginTime.AddSeconds(pk.OpeningSeconds))// 封盘
                    {
                        // 计算名次
                        var rankList = betService.CalculateRanks(pk.PKId);
                        var ranks = string.Join(",", rankList);
                        pk.Ranks = ranks;
                        pkService.UpdateRanks(pk.PKId, ranks);

                        var msg = string.Format("Calculate Ranks - PKId : {0} - Ranks : {1} - Time : {2}", pk.PKId, ranks, now.ToString(DateFormatConst.yMd_Hms));
                        _logger.Info(msg);

                        // 生成奖金
                        if (!pk.IsBonused)
                        {
                            // 更新 奖金生成标志, 防止多次计算
                            pkService.UpdateIsBonused(pk.PKId, true);
                            // 生成奖金
                            bonusService.GenerateBonus(pk);
                        }
                    }
                }
            }

            // 更新Bet表已结算标志
            betService.UpdateSettlementDone();
        }
    }
}