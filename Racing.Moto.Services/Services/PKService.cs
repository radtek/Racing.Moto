using Racing.Moto.Services.Caches;
using Racing.Moto.Services.Constants;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Racing.Moto.Core.Extentions;
using System.Data.Entity;
using Racing.Moto.Data;

namespace Racing.Moto.Services
{
    public class PKService : BaseServcice
    {
        /// <summary>
        /// 取当前PK
        /// </summary>
        /// <returns></returns>
        public PK GetCurrentPK()
        {
            using (var db = new RacingDbContext())
            {
                var current = Convert.ToDateTime(DateTime.Now.ToString(DateFormatConst.yMd_Hms));// remove millisecond
                return db.PK.Where(pk => pk.BeginTime <= current && current <= pk.EndTime).FirstOrDefault();
            }
        }

        /// <summary>
        /// 取最后一个PK
        /// </summary>
        /// <returns></returns>
        public PK GetLastPK()
        {
            using (var db = new RacingDbContext())
            {
                return db.PK.OrderByDescending(pk => pk.PKId).FirstOrDefault();
            }
        }

        /// <summary>
        /// 上一期
        /// </summary>
        public PK GetPrevPK()
        {
            using (var db = new RacingDbContext())
            {
                return db.PK.Where(pk => pk.EndTime < DateTime.Now).OrderByDescending(pk => pk.PKId).FirstOrDefault();
            }
        }

        public PK GetPK(int pkId)
        {
            using (var db = new RacingDbContext())
            {
                return db.PK.Where(pk => pk.PKId == pkId).FirstOrDefault();
            }
        }

        public List<PK> GetNotCalculatePKs()
        {
            using (var db = new RacingDbContext())
            {
                return db.PK.Where(pk => pk.Ranks == null).ToList();
            }
        }

        public List<PK> GetNotBonusPKs()
        {
            using (var db = new RacingDbContext())
            {
                return db.PK.Where(pk => !pk.IsBonused && pk.Ranks != null).ToList();
            }
        }

        /// <summary>
        /// 未退水且比赛结束
        /// </summary>
        /// <returns></returns>
        public List<PK> GetNotRebatePKs()
        {
            using (var db = new RacingDbContext())
            {
                return db.PK.Where(pk => !pk.IsRebated && DbFunctions.DiffSeconds(DateTime.Now, pk.EndTime) <= AppConfigCache.Racing_Lottery_Seconds).ToList();
            }
        }

        /// <summary>
        /// 取当前PK
        /// </summary>
        /// <returns></returns>
        public PKModel GetCurrentPKModel()
        {
            using (var db = new RacingDbContext())
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
                CloseBeginTime = currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds),

                GameBeginTime = currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds + currentPK.CloseSeconds),
                GamingSeconds = gamingSeconds,

                GamePassedSeconds = gamePassedSeconds,
                GameRemainSeconds = gameRemainSeconds
            };
        }

        public PK AddPK()
        {
            using (var db = new RacingDbContext())
            {
                // 生成PK
                PK pk = db.Database.SqlQuery<PK>(string.Format("EXEC {0}", DBConst.SP_PK_GeneratePK)).FirstOrDefault();

                if (pk != null)
                {
                    // 生成赔率
                    db.Database.ExecuteSqlCommand(string.Format("EXEC {0} {1}", DBConst.SP_PK_GeneratePKRate, pk.PKId));
                }

                return pk;
            }
        }
        #region 添加到数据库 使用 存储过程 SP_PK_GeneratePK 代替
        [Obsolete]
        public PK AddPK(DateTime beginTime)
        {
            using (var db = new RacingDbContext())
            {
                var pk = new PK
                {
                    CreateTime = DateTime.Now,
                    BeginTime = beginTime,
                    EndTime = DateTime.Now.AddSeconds(AppConfigCache.Racing_Total_Seconds),
                    OpeningSeconds = AppConfigCache.Racing_Opening_Seconds,
                    CloseSeconds = AppConfigCache.Racing_Close_Seconds,
                    GameSeconds = AppConfigCache.Racing_Game_Seconds,
                    LotterySeconds = AppConfigCache.Racing_Lottery_Seconds,
                    //PKRates = pkRates //pkRates 过多, 插入时间很长, 必须和PK新增分开
                };

                db.PK.Add(pk);
                db.SaveChanges();

                // pkRates 过多, 插入时间很长, 必须和PK新增分开
                var pkRates = new List<PKRate>();
                foreach (var rate in RateCache.GetRatesByType(Data.Enums.RateType.Arena))
                {
                    for (var num = 1; num <= 14; num++)
                    {
                        pkRates.Add(new PKRate { PKId = pk.PKId, Rank = rate.Rank, Num = num, Rate = RateService.GetRate(rate, num) });
                    }
                }
                db.PKRate.AddRange(pkRates);
                db.SaveChanges();

                return pk;
            }
        }

        [Obsolete]
        public PK SavePK(DateTime dt)
        {
            using (var db = new RacingDbContext())
            {
                var currentPK = db.PK.Where(pk => pk.BeginTime <= dt && dt <= pk.EndTime).FirstOrDefault();

                // 不存在PK, 创建新的PK
                if (currentPK == null)
                {
                    currentPK = AddPK(dt);
                }

                return currentPK;
            }
        }
        #endregion

        public bool ExistPK(DateTime dt)
        {
            using (var db = new RacingDbContext())
            {
                return db.PK.Where(pk => pk.BeginTime <= dt && dt <= pk.EndTime).Any();
            }
        }

        /// <summary>
        /// 更新名次
        /// </summary>
        public bool UpdateRanks(int pkId, string ranks)
        {
            using (var db = new RacingDbContext())
            {
                var dbPK = db.PK.Where(pk => pk.PKId == pkId).FirstOrDefault();
                if (dbPK != null && string.IsNullOrEmpty(dbPK.Ranks))
                {
                    dbPK.Ranks = ranks;

                    db.SaveChanges();

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 更新 奖金生成标志, 防止多次计算
        /// </summary>
        public void UpdateIsBonused(int pkId, bool isBonused)
        {
            using (var db = new RacingDbContext())
            {
                var pk = db.PK.Where(p => p.PKId == pkId).FirstOrDefault();
                if (pk != null)
                {
                    pk.IsBonused = isBonused;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 更新 奖金生成标志, 防止多次计算
        /// </summary>
        public void UpdateIsRebated(int pkId, bool isRebated)
        {
            using (var db = new RacingDbContext())
            {
                var pk = db.PK.Where(p => p.PKId == pkId).FirstOrDefault();
                if (pk != null)
                {
                    pk.IsRebated = isRebated;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 判断是否处于封盘期
        /// </summary>
        public bool IsClosed(PK pk)
        {
            using (var db = new RacingDbContext())
            {
                return pk.BeginTime.AddSeconds(pk.OpeningSeconds) <= DateTime.Now && DateTime.Now <= pk.BeginTime.AddSeconds(pk.OpeningSeconds + pk.CloseSeconds);
            }
        }

        /// <summary>
        /// 验证PK是否仍在开盘期
        /// </summary>
        public bool IsOpening(int pkId)
        {
            using (var db = new RacingDbContext())
            {
                var dbPK = db.PK.Where(pk => pk.PKId == pkId).FirstOrDefault();

                if (dbPK != null && dbPK.BeginTime.AddSeconds(dbPK.OpeningSeconds) > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 取PK
        /// </summary>
        public PagerResult<PK> GetPKs(SearchModel searchModel)
        {
            using (var db = new RacingDbContext())
            {
                var query = db.PK.Where(pk => pk.Ranks != null);
                if (searchModel.Key > 0)
                {
                    query = query.Where(q => q.PKId == searchModel.Key);
                }

                var pks = query.OrderByDescending(pk => pk.PKId).Pager<PK>(searchModel.PageIndex, searchModel.PageSize);
                return pks;
            }
        }

        /// <summary>
        /// 取已结算的PK数据, 报表查询中按期数查询使用
        /// </summary>
        /// <param name="count">期数</param>
        public List<PK> GetSettlementPKs(int count)
        {
            using (var db = new RacingDbContext())
            {
                return db.PK
                    .Where(pk => pk.EndTime < DateTime.Now)
                    .OrderByDescending(pk => pk.PKId)
                    .Take(count)
                    .ToList();
            }
        }
    }
}
