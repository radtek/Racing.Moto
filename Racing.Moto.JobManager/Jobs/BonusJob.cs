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
    /// <summary>
    /// 生成奖金
    /// </summary>
    public class BonusJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 生成奖金
        /// 每隔10秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "BonusJob Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
            var pkService = new PKService();
            var bonusService = new PKBonusService();
            var betService = new BetService();

            var pks = pkService.GetNotBonusPKs();

            foreach (var pk in pks)
            {
                if (pk != null && !string.IsNullOrEmpty(pk.Ranks))
                {
                    // 生成奖金
                    if (!pk.IsBonused)
                    {
                        // 更新 奖金生成标志, 防止多次计算
                        pkService.UpdateIsBonused(pk.PKId, true);
                        // 生成奖金
                        bonusService.GenerateBonus(pk);

                        var msg = string.Format("Generate Bonus - PKId : {0} - Time : {2}", pk.PKId, DateTime.Now.ToString(DateFormatConst.yMd_Hms));
                        _logger.Info(msg);
                    }
                }
            }

            // 更新Bet表已结算标志
            betService.UpdateSettlementDone();
        }
    }
}