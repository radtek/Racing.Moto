using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.Web.Jobs
{
    public class JobScheduler
    {
        public static void Start()
        {
            try
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();

                // 每5秒执行一次
                var interval = 5;
                IJobDetail job = JobBuilder.Create<PkJob>().Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("PkJobTrigger", "PkJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(interval).RepeatForever())
                    .Build();

                scheduler.ScheduleJob(job, trigger);
            }
            catch(Exception ex)
            {
                LogManager.GetCurrentClassLogger().Info(ex);
            }
        }
    }
}