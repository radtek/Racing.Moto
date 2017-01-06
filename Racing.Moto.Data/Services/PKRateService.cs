using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Services
{
    public class PKRateService : BaseServcice
    {
        public static decimal GetRate(PKRate pkRate, int num)
        {
            var rate = 0M;

            //switch (num)
            //{
            //    case 1: rate = pkRate.Number1; break;
            //    case 2: rate = pkRate.Number2; break;
            //    case 3: rate = pkRate.Number3; break;
            //    case 4: rate = pkRate.Number4; break;
            //    case 5: rate = pkRate.Number5; break;
            //    case 6: rate = pkRate.Number6; break;
            //    case 7: rate = pkRate.Number7; break;
            //    case 8: rate = pkRate.Number8; break;
            //    case 9: rate = pkRate.Number9; break;
            //    case 10: rate = pkRate.Number10; break;
            //    case 11: rate = pkRate.Big; break;
            //    case 12: rate = pkRate.Small; break;
            //    case 13: rate = pkRate.Odd; break;
            //    case 14: rate = pkRate.Even; break;
            //}

            return rate;
        }
    }
}
