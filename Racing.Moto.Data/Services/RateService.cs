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
            return db.Rate.ToList();
        }
    }
}
