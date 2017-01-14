using NLog;
using Quartz;
using Racing.Moto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.Web.Jobs
{
    public class PkJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();


        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                // 每隔5秒查看一下是否已经封盘, 封盘后计算名次
                var pkService = new PKService();
                var currentPK = pkService.GetCurrentPK();
                if (currentPK != null)
                {
                    // 封盘期 && 名次未更新过
                    if (pkService.IsClosedTime(currentPK) && string.IsNullOrEmpty(currentPK.Ranks))
                    {
                        // 计算名次
                        var ranks = new BetService().CalculateRanks(currentPK.PKId);
                        if (ranks != null)
                        {
                            pkService.UpdateRanks(currentPK.PKId, string.Join(",", ranks));
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