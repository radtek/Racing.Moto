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
    public class RebateJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 退水
        /// 每隔10秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "RebateJob Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(startInfo);
                _logger.Info(startInfo);

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