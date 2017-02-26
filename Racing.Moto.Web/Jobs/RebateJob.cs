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
    public class RebateJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 退水
        /// 每隔600秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var pkService = new PKService();
                var bonusService = new PKBonusService();

                var pks = pkService.GetNotRebatePKs();

                foreach (var pk in pks)
                {
                    var msg = string.Format("Rebate - PKId : {0} - Time : {1}", pk.PKId, DateTime.Now.ToString(DateFormatConst.yMd_Hms));
                    _logger.Info(msg);

                    // 更新 退水生成标志, 防止多次计算
                    pkService.UpdateIsRebated(pk.PKId, true);
                    // 生成退水
                    bonusService.GenerateRebate(pk);
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }
    }
}