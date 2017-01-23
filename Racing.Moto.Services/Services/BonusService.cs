using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class BonusService : BaseServcice
    {
        public List<PKBonus> GetPKBonus(BonusSearchModel model)
        {
            return db.PKBonus
                .Where(b => b.PKId == model.PKId && b.UserId == model.UserId)
                .OrderBy(b => b.Rank)
                .ThenBy(b => b.Num)
                .ToList();
        }
    }
}
