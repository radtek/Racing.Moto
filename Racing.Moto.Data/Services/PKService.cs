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
        public PK GetLastPK()
        {
            return db.PK.OrderByDescending(pk => pk.PKId).FirstOrDefault();
        }

        /// <summary>
        /// 取当前PK
        /// </summary>
        /// <returns></returns>
        public PKModel GetCurrentPKModel()
        {
            var currentPk = db.PK.Where(pk => pk.BeginTime <= DateTime.Now && DateTime.Now <= pk.EndTime).FirstOrDefault();

            if (currentPk != null)
            {
                return new PKModel
                {
                    PK = currentPk,
                    PassedSeconds = (DateTime.Now - currentPk.BeginTime).Seconds
                };
            }
            else
            {
                return null;
            }
        }

        public PK AddPK(DateTime beginTime)
        {
            var pk = new PK
            {
                CreateTime = DateTime.Now,
                BeginTime = beginTime,
                EndTime = DateTime.Now.AddMinutes(AppConfigCache.Racing_Total_Seconds),
                OpeningSeconds = AppConfigCache.Racing_Opening_Seconds,
                CloseSeconds = AppConfigCache.Racing_Close_Seconds,
                GameSeconds = AppConfigCache.Racing_Game_Seconds,
                LotterySeconds = AppConfigCache.Racing_Lottery_Seconds,
                PKRates = RateCache.GetAllRates().Select(r => new PKRate
                {
                    Rank = r.Rank,
                    Number1 = r.Number1,
                    Number2 = r.Number2,
                    Number3 = r.Number3,
                    Number4 = r.Number4,
                    Number5 = r.Number5,
                    Number6 = r.Number6,
                    Number7 = r.Number7,
                    Number8 = r.Number8,
                    Number9 = r.Number9,
                    Number10 = r.Number10,
                    Big = r.Big,
                    Small = r.Small,
                    Odd = r.Odd,
                    Even = r.Even
                }).ToList()
            };

            return pk;
        }
    }
}
