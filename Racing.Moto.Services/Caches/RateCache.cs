using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services.Caches
{
    public class RateCache
    {
        private static List<Rate> _rates = null;

        public static List<Rate> GetRatesByType(RateType type)
        {
            if (_rates == null)
            {
                _rates = new RateService().GetAll();
            }

            return _rates;
        }

        public static void Update()
        {
            _rates = new RateService().GetAll();
        }
    }
}
