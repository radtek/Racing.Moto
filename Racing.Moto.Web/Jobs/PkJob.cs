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
                var pkService = new PKService();

                // 每隔5秒查看一下, 生成PK
                var now = DateTime.Now;

                if (!pkService.ExistPK(now))
                {
                    var pk = new PKService().AddPK(now);

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