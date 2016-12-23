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
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            // 每10秒执行一次
            IJobDetail job = JobBuilder.Create<PkJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("PkJobTrigger", "PkJobGroup")
                .WithSimpleSchedule(t => t.WithIntervalInSeconds(10).RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}