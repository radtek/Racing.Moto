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

#if DEBUG
                if (!pkService.ExistPK(now))
                {
                    var pk = new PKService().AddPK();

                    if (pk != null)
                    {
                        var msg = string.Format("Add new PK - PKId : {0} - Time : {1}", pk.PKId, now.ToString(DateFormatConst.yMd_Hms));
                        _logger.Info(msg);
                    }
                    else
                    {
                        var msg = string.Format("Add new PK - PKId : {0} - Time : {1}", "重复数据, 未进行插入", now.ToString(DateFormatConst.yMd_Hms));
                    }
                }
#else

                //_logger.Info("release");
                var start = new DateTime(now.Year, now.Month, now.Day, 9, 2, 0);
                var end = new DateTime(now.Year, now.Month, now.Day, 23, 52, 5);

                if (start <= now && now <= end)
                {
                    if (!pkService.ExistPK(now))
                    {
                        var pk = new PKService().AddPK();

                        if (pk != null)
                        {
                            var msg = string.Format("Add new PK - PKId : {0} - Time : {1}", pk.PKId, now.ToString(DateFormatConst.yMd_Hms));
                            _logger.Info(msg);
                        }
                        else
                        {
                            var msg = string.Format("Add new PK - PKId : {0} - Time : {1}", "重复数据, 未进行插入", now.ToString(DateFormatConst.yMd_Hms));
                        }
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }
    }
}