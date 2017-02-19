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

        /// <summary>
        /// 生成奖金
        /// </summary>
        /// <param name="pk"></param>
        public void GenerateBonus(PK pk)
        {
            var betService = new BetService();
            var bets = betService.ConvertRanksToBets(pk.Ranks);
            var pkRates = new PKRateService().GetPKRates(pk.PKId);

            // 按 名次/大小单双+车号 循环
            foreach (var bet in bets)
            {
                // 奖金
                var bonuses = new List<PKBonus>();

                // 名次+车号 的下注数据
                var dbBets = betService.GetBets(pk.PKId, bet.Rank, bet.Num);
                foreach (var dbBet in dbBets)
                {
                    var pkRate = pkRates.Where(r => r.Rank == dbBet.Rank && r.Num == dbBet.Num).First();

                    bonuses.Add(new PKBonus
                    {
                        PKId = pk.PKId,
                        UserId = dbBet.UserId,
                        Rank = dbBet.Rank,
                        Num = dbBet.Num,
                        Amount = 0//[TODO]
                    });
                }

                if (bonuses.Count > 0)
                {
                    // 保存奖金
                    db.PKBonus.AddRange(bonuses);
                    db.SaveChanges();
                }
            }
        }
    }
}
