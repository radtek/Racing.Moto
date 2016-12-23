using NLog;
using Quartz;
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
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }
    }
}