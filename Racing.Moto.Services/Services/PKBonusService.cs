using Racing.Moto.Core.Extentions;
using Racing.Moto.Data;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    public class PKBonusService : BaseServcice
    {
        public List<PKBonus> GetPKBonus(int pkId, int userId)
        {
            using (var db = new RacingDbContext())
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
        public void GenerateBonus(PK pk)
        {
            using (var db = new RacingDbContext())
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
                            BetId = dbBet.BetId,
                            PKId = pk.PKId,
                            UserId = dbBet.UserId,
                            Rank = dbBet.Rank,
                            Num = dbBet.Num,
                            BonusType = Data.Enums.BonusType.Bonus,
                            Amount = Math.Round(dbBet.Amount * pkRate.Rate, 4),
                            IsSettlementDone = true // 直接设置成已结算
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

        /// <summary>
        /// 生成退水
        /// </summary>
        /// <param name="pk"></param>
        public void GenerateRebate(PK pk)
        {
            using (var db = new RacingDbContext())
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

                        // 会员退水奖金
                        var bonuses = new List<PKBonus>();

                        var dbBets = db.Bet.Where(bi => bi.PKId == pk.PKId && bi.UserId == userId).ToList();
                        foreach (var dbBet in dbBets)
                        {
                            bonuses.Add(new PKBonus
                            {
                                BetId = dbBet.BetId,
                                PKId = pk.PKId,
                                UserId = dbBet.UserId,
                                Rank = dbBet.Rank,
                                Num = dbBet.Num,
                                BonusType = Data.Enums.BonusType.Rebate,
                                Amount = Math.Round(dbBet.Amount * rebate, 4),
                                IsSettlementDone = true // 直接设置成已结算
                            });
                        }

                        if (bonuses.Count > 0)
                        {
                            // 保存会员退水奖金
                            db.PKBonus.AddRange(bonuses);
                            db.SaveChanges();

                            // 代理退水奖金
                            if (userRebate.User.ParentUserId.HasValue)
                            {
                                var agentUserRebate = db.UserRebate.Include(nameof(UserRebate.User)).Where(r => r.UserId == userRebate.User.ParentUserId).FirstOrDefault();
                                if (agentUserRebate != null)
                                {
                                    var agentUser = agentUserRebate.User;
                                    var agentRebate = UserRebateService.GetDefaultRebate(agentUserRebate, agentUserRebate.User.DefaultRebateType);
                                    // 代理退水奖金
                                    var agentBonuses = bonuses.Select(b => new PKBonus
                                    {
                                        BetId = b.BetId,
                                        PKId = pk.PKId,
                                        UserId = agentUser.UserId,
                                        Rank = b.Rank,
                                        Num = b.Num,
                                        BonusType = Data.Enums.BonusType.Rebate,
                                        Amount = Math.Round(b.Amount * agentRebate, 4),
                                        IsSettlementDone = true // 直接设置成已结算
                                    }).ToList();
                                    // 保存代理退水奖金
                                    db.PKBonus.AddRange(agentBonuses);
                                    db.SaveChanges();
                                }

                                // 总代理退水奖金
                                if (agentUserRebate.User.ParentUserId.HasValue)
                                {
                                    var generalAgentUserRebate = db.UserRebate.Include(nameof(UserRebate.User)).Where(r => r.UserId == agentUserRebate.User.ParentUserId).FirstOrDefault();
                                    if (generalAgentUserRebate != null)
                                    {
                                        var generalAgentUser = generalAgentUserRebate.User;
                                        var generalAgentRebate = UserRebateService.GetDefaultRebate(generalAgentUserRebate, generalAgentUserRebate.User.DefaultRebateType);
                                        // 总代理退水奖金
                                        var generalAgentBonuses = bonuses.Select(b => new PKBonus
                                        {
                                            BetId = b.BetId,
                                            PKId = pk.PKId,
                                            UserId = generalAgentUser.UserId,
                                            Rank = b.Rank,
                                            Num = b.Num,
                                            BonusType = Data.Enums.BonusType.Rebate,
                                            Amount = Math.Round(b.Amount * generalAgentRebate, 4),
                                            IsSettlementDone = true // 直接设置成已结算
                                        }).ToList();
                                        // 保存总代理退水奖金
                                        db.PKBonus.AddRange(generalAgentBonuses);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }
}
