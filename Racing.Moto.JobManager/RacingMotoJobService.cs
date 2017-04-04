using NLog;
using Racing.Moto.JobManager.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.JobManager
{
    /// <summary>
    /// RacingMotoJob Service
    /// 创建服务:
    ///     sc create RacingMotoJob binPath= "D:\Work\Racing.Moto\Racing.Moto.JobManager\bin\Debug\Racing.Moto.JobManager.exe" 
    /// 配置服务:
    ///     sc config RacingMotoJob start = AUTO    (自动) 
    ///     sc config RacingMotoJob start = DEMAND(手动)
    ///     sc config RacingMotoJob start = DISABLED(禁用)
    /// 启动服务:
    ///     net start RacingMotoJob
    /// 关闭服务:
    ///     net stop RacingMotoJob
    /// 删除服务:
    ///     sc delete RacingMotoJob
    /// </summary>
    partial class RacingMotoJobService : ServiceBase
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string DateFormatYmdhms = "yyyy/MM/dd HH:mm:ss";

        public RacingMotoJobService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("service: OnStart: " + DateTime.Now.ToString(DateFormatYmdhms));

            JobScheduler.Start();
        }

        protected override void OnStop()
        {
            _logger.Info("service: OnStop: " + DateTime.Now.ToString(DateFormatYmdhms));
        }

        protected override void OnPause()
        {
            base.OnPause();
            _logger.Info("service: OnPause: " + DateTime.Now.ToString(DateFormatYmdhms));
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();

            _logger.Info("service: OnShutdown: " + DateTime.Now.ToString(DateFormatYmdhms));
        }
    }
}
