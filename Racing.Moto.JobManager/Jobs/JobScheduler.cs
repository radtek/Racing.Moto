using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.JobManager.Jobs
{
    public class JobScheduler
    {
        public static void Start()
        {
            try
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();

                // 生成PK: 每1秒执行一次
                var interval = 1;
                IJobDetail job = JobBuilder.Create<PkJob>().Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("PkJobTrigger", "PkJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(interval).RepeatForever())
                    .Build();
                scheduler.ScheduleJob(job, trigger);

                // 计算名次: 每5秒执行一次
                var rankInterval = 5;
                IJobDetail rankJob = JobBuilder.Create<RankJob>().Build();
                ITrigger rankTrigger = TriggerBuilder.Create()
                    .WithIdentity("RankJobTrigger", "RankJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(rankInterval).RepeatForever())
                    .Build();
                scheduler.ScheduleJob(rankJob, rankTrigger);

                // 生成奖金: 每10秒执行一次
                var bonusInterval = 10;
                IJobDetail bonusJob = JobBuilder.Create<BonusJob>().Build();
                ITrigger bonusTrigger = TriggerBuilder.Create()
                    .WithIdentity("BonusJobTrigger", "BonusJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(bonusInterval).RepeatForever())
                    .Build();
                scheduler.ScheduleJob(bonusJob, bonusTrigger);

                // 退水: 每10秒执行一次
                var rebateInterval = 10;
                IJobDetail rebateJob = JobBuilder.Create<RebateJob>().Build();
                ITrigger rebateTrigger = TriggerBuilder.Create()
                    .WithIdentity("RebateJobTrigger", "RebateJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(rebateInterval).RepeatForever())
                    .Build();
                scheduler.ScheduleJob(rebateJob, rebateTrigger);


                #region Game

                // 生成PK: 每1秒执行一次
                var gameInterval = 1;
                IJobDetail gameJob = JobBuilder.Create<PkGameJob>().Build();
                ITrigger gameTrigger = TriggerBuilder.Create()
                    .WithIdentity("PkGameJobTrigger", "PkGameJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(gameInterval).RepeatForever())
                    .Build();
                scheduler.ScheduleJob(gameJob, gameTrigger);

                // 计算名次: 每5秒执行一次
                var gameRankInterval = 5;
                IJobDetail gameRankJob = JobBuilder.Create<PkGameRankJob>().Build();
                ITrigger gameRankTrigger = TriggerBuilder.Create()
                    .WithIdentity("PkGameRankJobTrigger", "PkGameRankJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(gameRankInterval).RepeatForever())
                    .Build();
                scheduler.ScheduleJob(gameRankJob, gameRankTrigger);

                // 生成奖金: 每10秒执行一次
                var gameBonusInterval = 10;
                IJobDetail gameBonusJob = JobBuilder.Create<PkGameBonusJob>().Build();
                ITrigger gameBonusTrigger = TriggerBuilder.Create()
                    .WithIdentity("PkGameBonusJobTrigger", "PkGameBonusJobGroup")
                    .WithSimpleSchedule(t => t.WithIntervalInSeconds(gameBonusInterval).RepeatForever())
                    .Build();
                scheduler.ScheduleJob(gameBonusJob, gameBonusTrigger);

                #endregion

            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Info(ex);
            }
        }
    }
}