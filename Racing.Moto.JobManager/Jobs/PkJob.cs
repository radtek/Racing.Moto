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
    public class PkJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// PK
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "PkJob Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(startInfo);
                _logger.Info(startInfo);

                var pkService = new PKService();

                // 每隔5秒查看一下, 生成PK
                var now = DateTime.Now;
                var start = new DateTime(now.Year, now.Month, now.Day, 9, 2, 0);
                var end = new DateTime(now.Year, now.Month, now.Day, 23, 52, 5);
                //var start = new DateTime(now.Year, now.Month, now.Day - 1, 9, 2, 0);
                //var end = new DateTime(now.Year, now.Month, now.Day + 1, 23, 52, 5);

                //if (start <= now && now <= end)
                //{
                //    if (!pkService.ExistPK(now))
                //    {
                //        var pk = new PKService().AddPK();

                //        var msg = string.Format("Add new PK - PKId : {0} - Time : {1}", pk.PKId, now.ToString(DateFormatConst.yMd_Hms));
                //        _logger.Info(msg);
                //    }
                //}
                if (!pkService.ExistPK(now))
                {
                    var pk = new PKService().AddPK();

                    var msg = string.Format("Add new PK - PKId : {0} - Time : {1}", pk.PKId, now.ToString(DateFormatConst.yMd_Hms));
                    _logger.Info(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }
    }
}