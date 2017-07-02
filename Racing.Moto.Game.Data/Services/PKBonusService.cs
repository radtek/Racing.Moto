using Racing.Moto.Core.Extentions;
using Racing.Moto.Game.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Services
{
    public class PKBonusService
    {
        public List<PKBonus> GetPKBonus(int pkId, int userId)
        {
            using (var db = new RacingGameDbContext())
            {
                return db.PKBonus
                    .Where(b => b.PKId == pkId && b.UserId == userId)
                    .OrderBy(b => b.Rank)
                    .ThenBy(b => b.Num)
                    .ToList();
            }
        }

        /// <summary>
        /// 生成奖金
        /// </summary>
        /// <param name="pk"></param>
        public void GenerateBonus(List<PKBonus> bonuses)
        {
            using (var db = new RacingGameDbContext())
            {
                if (bonuses.Count > 0)
                {
                    // 保存奖金
                    db.PKBonus.AddRange(bonuses);
                    db.SaveChanges();

                    // 奖金加到余额
                    var sql = new StringBuilder();

                    foreach (var bonus in bonuses)
                    {
                        sql.AppendLine(string.Format("Update dbo.[User] Set Amount = IsNull(Amount, 0) + {0} Where UserId = {1};", bonus.Amount, bonus.UserId));
                    }
                    db.Database.ExecuteSqlCommand(sql.ToString());
                    db.SaveChanges();
                }
            }
        }

    }
}