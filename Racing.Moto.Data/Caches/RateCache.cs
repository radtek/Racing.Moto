using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Caches
{
    public class RateCache
    {
        private static List<Rate> _rates = null;

        public static List<Rate> GetAllAppConfigs()
        {
            if (_rates == null)
            {
                _rates = new RateService().GetAll();
            }

            return _rates;
        }

        public static Rate GetRate(int rank)
        {
            var rate = GetAllAppConfigs().Where(a => a.Rank == rank).FirstOrDefault();

            return rate;
        }

        public static void Update()
        {
            _rates = new RateService().GetAll();
        }
    }
}
