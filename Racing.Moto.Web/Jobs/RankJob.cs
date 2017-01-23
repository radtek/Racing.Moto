using NLog;
using Quartz;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.Web.Jobs
{
    public class RankJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 计算名次, 奖金
        /// 每隔3秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var pkService = new PKService();

                var pk = pkService.GetCurrentPK();
                if (pk != null && string.IsNullOrEmpty(pk.Ranks))
                {
                    var now = DateTime.Now;
                    if (now > pk.BeginTime.AddSeconds(pk.OpeningSeconds))// 已封盘
                    {
                        // 计算名次
                        var rankList = new BetService().CalculateRanks(pk.PKId);
                        var ranks = string.Join(",", rankList);
                        pkService.UpdateRanks(pk.PKId, ranks);

                        var msg = string.Format("Calculate Rankd - PKId : {0} - Ranks : {1} - Time : {2}", pk.PKId, ranks, now.ToString(DateFormatConst.yMd_Hms));
                        _logger.Info(msg);

                        // 计算奖金

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