using Racing.Moto.Data.Caches;
using Racing.Moto.Data.Constants;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Services
{
    public class PKService : BaseServcice
    {
        /// <summary>
        /// 取最后一个PK
        /// </summary>
        /// <returns></returns>
        public PK GetCurrentPK()
        {
            return db.PK.Where(pk => pk.BeginTime <= DateTime.Now && DateTime.Now <= pk.EndTime).FirstOrDefault();
        }

        /// <summary>
        /// 取当前PK
        /// </summary>
        /// <returns></returns>
        public PKModel GetCurrentPKModel()
        {
            var currentPK = db.PK.Where(pk => pk.BeginTime <= DateTime.Now && DateTime.Now <= pk.EndTime).FirstOrDefault();

            // 不存在PK, 创建新的PK
            if (currentPK == null)
            {
                currentPK = AddPK(DateTime.Now);
            }

            var passedSeconds = (DateTime.Now - currentPK.BeginTime).Seconds;
            var remainSeconds = (currentPK.EndTime - currentPK.BeginTime).Seconds - passedSeconds;
            // 距离比赛开始的秒数, 负:未开始, 正:已开始
            var gamingSeconds = (DateTime.Now - currentPK.BeginTime.AddSeconds(currentPK.OpeningSeconds + currentPK.CloseSeconds)).Seconds;
            // 比赛已经开始n秒
            var gamePassedSeconds = gamingSeconds > 0 ? gamingSeconds : 0;
            // 比赛剩余n秒
            var gameRemainSeconds = currentPK.GameSeconds - gamePassedSeconds;
            gameRemainSeconds = gameRemainSeconds > 0 ? gameRemainSeconds : 0;

            return new PKModel
            {
                PK = currentPK,
                PassedSeconds = passedSeconds,
                RemainSeconds = remainSeconds,
                GamingSeconds = gamingSeconds,
                GamePassedSeconds = gamePassedSeconds,
                GameRemainSeconds = gameRemainSeconds
            };
        }

        public PK AddPK(DateTime beginTime)
        {
            var pkRates = new List<PKRate>();

            foreach (var rate in RateCache.GetAllRates())
            {
                for (var num = 1; num <= 14; num++)
                {
                    pkRates.Add(new PKRate { Rank = rate.Rank, Num = num, Rate = RateService.GetRate(rate, num) });
                }
            }

            var pk = new PK
            {
                CreateTime = DateTime.Now,
                BeginTime = beginTime,
                EndTime = DateTime.Now.AddMinutes(AppConfigCache.Racing_Total_Seconds),
                OpeningSeconds = AppConfigCache.Racing_Opening_Seconds,
                CloseSeconds = AppConfigCache.Racing_Close_Seconds,
                GameSeconds = AppConfigCache.Racing_Game_Seconds,
                LotterySeconds = AppConfigCache.Racing_Lottery_Seconds,
                PKRates = pkRates
            };

            return pk;
        }


        /// <summary>
        /// 判断是否处于封盘期
        /// </summary>
        public bool IsClosedTime(PK pk)
        {
            return pk.BeginTime.AddSeconds(pk.OpeningSeconds) <= DateTime.Now && DateTime.Now <= pk.BeginTime.AddSeconds(pk.CloseSeconds);
        }

        /// <summary>
        /// 更新名次
        /// </summary>
        public void UpdateRanks(int pkId, string ranks)
        {
            var dbPK = db.PK.Where(pk => pk.PKId == pkId).FirstOrDefault();
            if (dbPK != null)
            {
                dbPK.Ranks = ranks;

                db.SaveChanges();
            }
        }
    }
}
