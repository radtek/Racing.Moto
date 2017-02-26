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
        /// 计算名次, 生成奖金
        /// 每隔3秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var pkService = new PKService();
                var bonusService = new PKBonusService();

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
                            var rankList = new BetService().CalculateRanks(pk.PKId);
                            var ranks = string.Join(",", rankList);
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
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }
    }
}