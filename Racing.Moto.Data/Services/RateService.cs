using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Services
{
    public class RateService : BaseServcice
    {
        public List<Rate> GetAll()
        {
            return db.Rate.OrderBy(r => r.Rank).ToList();
        }

        public static decimal GetRate(Rate rate, int num)
        {
            var rateVal = 0M;

            switch (num)
            {
                case 1: rateVal = rate.Number1; break;
                case 2: rateVal = rate.Number2; break;
                case 3: rateVal = rate.Number3; break;
                case 4: rateVal = rate.Number4; break;
                case 5: rateVal = rate.Number5; break;
                case 6: rateVal = rate.Number6; break;
                case 7: rateVal = rate.Number7; break;
                case 8: rateVal = rate.Number8; break;
                case 9: rateVal = rate.Number9; break;
                case 10: rateVal = rate.Number10; break;
                case 11: rateVal = rate.Big; break;
                case 12: rateVal = rate.Small; break;
                case 13: rateVal = rate.Odd; break;
                case 14: rateVal = rate.Even; break;
            }

            return rateVal;
        }
    }
}
