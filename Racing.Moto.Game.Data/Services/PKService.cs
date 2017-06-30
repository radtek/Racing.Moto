﻿using Racing.Moto.Game.Data.Caches;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Entities;
using Racing.Moto.Game.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Services
{
    public class PKService
    {
        /// <summary>
        /// 取当前PK
        /// </summary>
        /// <returns></returns>
        public PK GetCurrentPK()
        {
            using (var db = new RacingGameDbContext())
            {
                var current = Convert.ToDateTime(DateTime.Now.ToString(DateFormatConst.yMd_Hms));// remove millisecond
                return db.PK.Where(pk => pk.BeginTime <= current && current <= pk.EndTime).FirstOrDefault();
            }
        }

        public PK GetPK(int pkId)
        {
            using (var db = new RacingGameDbContext())
            {
                return db.PK.Where(pk => pk.PKId == pkId).FirstOrDefault();
            }
        }
        /// <summary>
        /// 取当前PK
        /// </summary>
        /// <returns></returns>
        public PKModel GetCurrentPKModel()
        {
            using (var db = new RacingGameDbContext())
            {
                var currentPK = GetCurrentPK();
                //var currentPK = GetLastPK();

                // job 设置5秒启动一次, 两次PK之间会有时间差, 故取不到PK数据
                if (currentPK == null)
                {
                    return null;
                }

                return ConvertToPKModel(currentPK);
            }
        }
        public PKModel ConvertToPKModel(PK currentPK)
        {
            var now = DateTime.Now;

            var passedSeconds = (int)(now - currentPK.BeginTime).TotalSeconds;
            var remainSeconds = (int)(currentPK.EndTime - currentPK.BeginTime).TotalSeconds - passedSeconds;
            // 距离封盘的秒数, 负:已封盘, 正:距离封盘的秒数
            var openingRemainSeconds = (int)(currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds) - now).TotalSeconds;
            //// 距离开奖的秒数, 负:已封盘, 正:距离封盘的秒数
            var toLotterySeconds = (int)(currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds + currentPK.CloseSeconds + currentPK.GameSeconds) - now).TotalSeconds;
            //var toLotterySeconds = (int)(currentPK.BeginTime.AddSeconds(AppConfigCache.Racing_Total_Seconds) - now).TotalSeconds;

            // 距离比赛开始的秒数, 负:未开始, 正:已开始
            var gamingSeconds = (int)(now - currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds + currentPK.CloseSeconds)).TotalSeconds;

            // 比赛已经开始n秒
            var gamePassedSeconds = gamingSeconds > 0 ? gamingSeconds : 0;
            // 比赛剩余n秒
            var gameRemainSeconds = currentPK.GameSeconds - gamePassedSeconds;
            gameRemainSeconds = gameRemainSeconds > 0 ? gameRemainSeconds : 0;

            return new PKModel
            {
                PK = currentPK,
                Now = now,
                PassedSeconds = passedSeconds,
                RemainSeconds = remainSeconds,
                OpeningRemainSeconds = openingRemainSeconds,
                ToLotterySeconds = Math.Abs(toLotterySeconds) + AppConfigCache.Racing_Lottery_Seconds,  //延后10s
                CloseBeginTime = currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds),

                GameBeginTime = currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds + currentPK.CloseSeconds),
                GamingSeconds = gamingSeconds,

                GamePassedSeconds = gamePassedSeconds,
                GameRemainSeconds = gameRemainSeconds
            };
        }

        /// <summary>
        /// 生成 1条PK, 3条PKRoom (初中高级), 24条PKRoomDesk(桌)
        /// </summary>
        public PK AddPK()
        {
            using (var db = new RacingGameDbContext())
            {
                // 生成PK
                var pk = db.Database.SqlQuery<PK>(string.Format("EXEC {0}", DBConst.SP_PK_GeneratePK)).FirstOrDefault();
                return pk;
            }
        }

        public bool ExistPK(DateTime dt)
        {
            using (var db = new RacingGameDbContext())
            {
                return db.PK.Where(pk => pk.BeginTime <= dt && dt <= pk.EndTime).Any();
            }
        }


        public List<PK> GetNotCalculatePKs()
        {
            using (var db = new RacingGameDbContext())
            {
                return db.PK
                    .Include(nameof(PK.PKRooms)).Include("PKRooms.PKRoomDesks")
                    .Where(pk => !pk.IsRanked).ToList();
            }
        }

        /// <summary>
        /// 更新名次
        /// </summary>
        public void UpdateRanks(List<PKRoomDesk> desks)
        {
            using (var db = new RacingGameDbContext())
            {
                var deskIds = desks.Select(d => d.PKRoomDeskId).ToList();
                var dbDesks = db.PKRoomDesk.Where(d => deskIds.Contains(d.PKRoomDeskId)).ToList();

                foreach(var dbDesk in dbDesks)
                {
                    var desk = desks.Where(d => d.PKRoomDeskId == dbDesk.PKRoomDeskId).First();

                    dbDesk.Ranks = desk.Ranks;
                }

                db.SaveChanges();
            }
        }
    }
}
