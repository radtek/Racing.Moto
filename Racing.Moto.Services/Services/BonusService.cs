using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class PKBonusService : BaseServcice
    {
        public List<PKBonus> GetPKBonus(int pkId, int userId)
        {
            return db.PKBonus
                .Where(b => b.PKId == pkId && b.UserId == userId)
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
                        BonusType = Data.Enums.BonusType.Bonus,
                        Amount = Math.Round(dbBet.Amount * pkRate.Rate, 4)
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

        /// <summary>
        /// 生成退水
        /// </summary>
        /// <param name="pk"></param>
        public void GenerateRebate(PK pk)
        {
            // 按下注用户生成
            var userIds = db.Bet.Where(b => b.PKId == pk.PKId).Select(b => b.UserId).Distinct().ToList();
            var userRebates = db.UserRebate.Include(nameof(UserRebate.User)).Where(r => userIds.Contains(r.UserId)).ToList();
            foreach (var userId in userIds)
            {
                var userRebate = userRebates.Where(e => e.UserId == userId).FirstOrDefault();

                if (userRebate != null)
                {
                    var rebate = UserRebateService.GetDefaultRebate(userRebate, userRebate.User.DefaultRebateType);

                    // 退水奖金
                    var bonuses = new List<PKBonus>();

                    var dbBets = db.Bet.Where(bi => bi.PKId == pk.PKId && bi.UserId == userId).ToList();
                    foreach (var dbBet in dbBets)
                    {
                        bonuses.Add(new PKBonus
                        {
                            PKId = pk.PKId,
                            UserId = dbBet.UserId,
                            Rank = dbBet.Rank,
                            Num = dbBet.Num,
                            BonusType = Data.Enums.BonusType.Rebate,
                            Amount = Math.Round(dbBet.Amount * rebate, 4)
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

        #region Report

        public List<PKBonus> GetBonusReport(BonusSearchModel model)
        {
            return db.PKBonus
                .Where(b => b.PKId == model.PKId && b.UserId == model.UserId)
                .OrderBy(b => b.Rank)
                .ThenBy(b => b.Num)
                .ToList();
        }

        #endregion
    }
}
