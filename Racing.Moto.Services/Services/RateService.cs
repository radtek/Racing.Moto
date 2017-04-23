using Racing.Moto.Data;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class RateService : BaseServcice
    {
        public List<Rate> GetAll()
        {
            using (var db = new RacingDbContext())
            {
                return db.Rate.OrderBy(r => r.RateType).ThenBy(r => r.Rank).ToList();
            }
        }

        public List<Rate> GetRatesByType(RateType type)
        {
            using (var db = new RacingDbContext())
            {
                return db.Rate.Where(r => r.RateType == type).OrderBy(r => r.Rank).ToList();
            }
        }

        public void UpdateRates(RateType type, List<Rate> rates)
        {
            using (var db = new RacingDbContext())
            {
                var dbRates = db.Rate.Where(r => r.RateType == type).OrderBy(r => r.Rank).ToList();
                foreach (var dbRate in dbRates)
                {
                    var rate = rates.Where(r => r.RateId == dbRate.RateId).FirstOrDefault();
                    if (rate != null)
                    {
                        dbRate.Rate1 = rate.Rate1;
                        dbRate.Rate2 = rate.Rate2;
                        dbRate.Rate3 = rate.Rate3;
                        dbRate.Rate4 = rate.Rate4;
                        dbRate.Rate5 = rate.Rate5;
                        dbRate.Rate6 = rate.Rate6;
                        dbRate.Rate7 = rate.Rate7;
                        dbRate.Rate8 = rate.Rate8;
                        dbRate.Rate9 = rate.Rate9;
                        dbRate.Rate10 = rate.Rate10;
                        dbRate.Big = rate.Big;
                        dbRate.Small = rate.Small;
                        dbRate.Odd = rate.Odd;
                        dbRate.Even = rate.Even;
                    }
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 批量修改赔率
        /// </summary>
        /// <param name="type">0:竞技场, 1: 娱乐场a, 2: 娱乐场b, 3: 娱乐场c</param>
        /// <param name="batchType">1:名次，2:大小，3:单双，4:全部</param>
        /// <param name="rate">赔率</param>
        public void UpdateBatchRates(RateType type, BatchRateType batchType, decimal rate)
        {
            using (var db = new RacingDbContext())
            {
                var dbRates = db.Rate.Where(r => r.RateType == type).OrderBy(r => r.Rank).ToList();

                foreach (var dbRate in dbRates)
                {
                    #region 名次 or 全部
                    if (batchType == BatchRateType.Rank || batchType == BatchRateType.All)
                    {
                        dbRate.Rate1 = rate;
                        dbRate.Rate2 = rate;
                        dbRate.Rate3 = rate;
                        dbRate.Rate4 = rate;
                        dbRate.Rate5 = rate;
                        dbRate.Rate6 = rate;
                        dbRate.Rate7 = rate;
                        dbRate.Rate8 = rate;
                        dbRate.Rate9 = rate;
                        dbRate.Rate10 = rate;
                    }
                    #endregion

                    #region 大小 or 全部
                    if (batchType == BatchRateType.BigSmall || batchType == BatchRateType.All)
                    {
                        dbRate.Big = rate;
                        dbRate.Small = rate;
                    }
                    #endregion

                    #region 大小 or 全部
                    if (batchType == BatchRateType.OddEven || batchType == BatchRateType.All)
                    {
                        dbRate.Odd = rate;
                        dbRate.Even = rate;
                    }
                    #endregion
                }

                db.SaveChanges();
            }
        }

        #region static

        public static decimal GetRate(Rate rate, int num)
        {
            var rateVal = 0M;

            switch (num)
            {
                case 1: rateVal = rate.Rate1; break;
                case 2: rateVal = rate.Rate2; break;
                case 3: rateVal = rate.Rate3; break;
                case 4: rateVal = rate.Rate4; break;
                case 5: rateVal = rate.Rate5; break;
                case 6: rateVal = rate.Rate6; break;
                case 7: rateVal = rate.Rate7; break;
                case 8: rateVal = rate.Rate8; break;
                case 9: rateVal = rate.Rate9; break;
                case 10: rateVal = rate.Rate10; break;
                case 11: rateVal = rate.Big; break;
                case 12: rateVal = rate.Small; break;
                case 13: rateVal = rate.Odd; break;
                case 14: rateVal = rate.Even; break;
            }

            return rateVal;
        }

        #endregion
    }
}
