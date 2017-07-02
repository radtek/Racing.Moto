using NLog;
using Quartz;
using Racing.Moto.Core.Utils;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Entities;
using Racing.Moto.Game.Data.Services;
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
    public class PkGameRankJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 计算名次, 生成奖金
        /// 每隔3秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "[PkGameRankJob] Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(startInfo);
                _logger.Info(startInfo);

                Run();
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        public void Run()
        {
            var pkService = new PKService();
            
            var pks = pkService.GetNotCalculatePKs();

            foreach (var pk in pks)
            {
                if (pk != null && !pk.IsRanked)
                {
                    try
                    {
                        var now = DateTime.Now;
                        if (now >= pk.BeginTime.AddSeconds(pk.OpeningSeconds))// 封盘
                        {
                            var desks = new List<PKRoomDesk>();

                            // 计算名次
                            foreach (var room in pk.PKRooms)
                            {
                                foreach (var desk in room.PKRoomDesks)
                                {
                                    var ranks = RandomUtil.GetRandomList(1, 10);
                                    desk.Ranks = string.Join(",", ranks);
                                }

                                desks.AddRange(room.PKRoomDesks);
                            }

                            // 保存名次
                            pkService.UpdateRanks(desks);


                            var msg = string.Format("[PkGameRankJob] Calculate Ranks - PKId : {0} - Time : {1}", pk.PKId, now.ToString(DateFormatConst.yMd_Hms));
                            _logger.Info(msg);
                        }

                    }
                    catch (Exception ex)
                    {
                        var msg = string.Format("[PkGameRankJob] Calculate Ranks Failed - PKId : {0} - Time : {1}", pk.PKId, DateTime.Now.ToString(DateFormatConst.yMd_Hms));
                        _logger.Info(msg);
                        _logger.Info(ex);
                    }
                }
            }
        }
    }
}