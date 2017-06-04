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

                        // 奖金加到余额
                        var userExtensionService = new UserExtensionService();
                        foreach (var bonus in bonuses)
                        {
                            userExtensionService.AddAmount(bonus.UserId, bonus.Amount);
                        }
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
            var userExtensionService = new UserExtensionService();

            using (var db = new RacingDbContext())
            {
                // 按下注用户生成
                var userIds = db.Bet.Where(b => b.PKId == pk.PKId).Select(b => b.UserId).Distinct().ToList();
                foreach (var userId in userIds)
                {
                    var user = db.User.Where(u => u.UserId == userId).First();
                    var userRebates = db.UserRebate.Where(r => r.UserId == userId).ToList();

                    if (userRebates.Count > 0)
                    {
                        var bonuses = new List<PKBonus>();// 会员退水奖金
                        var agentBonuses = new List<PKBonus>();// 代理退水奖金
                        var generalAgentBonuses = new List<PKBonus>();// 总代理退水奖金

                        var dbBets = db.Bet.Where(bi => bi.PKId == pk.PKId && bi.UserId == userId).ToList();
                        foreach (var dbBet in dbBets)
                        {
                            #region 会员退水
                            var userRebate = userRebates.Where(e => e.RebateNo == dbBet.Num).FirstOrDefault();
                            var rebate = UserRebateService.GetDefaultRebate(userRebate, user.DefaultRebateType);
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

                            #endregion

                            if (user.ParentUserId.HasValue)
                            {
                                #region 代理退水
                                var agentUser = db.User.Where(u => u.UserId == user.ParentUserId).First();
                                var agentUserRebate = db.UserRebate
                                    .Where(r => r.UserId == user.ParentUserId && r.RebateNo == dbBet.Num).FirstOrDefault();
                                var agentRebate = UserRebateService.GetDefaultRebate(agentUserRebate, user.DefaultRebateType);  // 使用下注用户的默认盘
                                if (agentRebate - rebate > 0)
                                {
                                    agentBonuses.Add(new PKBonus
                                    {
                                        BetId = dbBet.BetId,
                                        PKId = pk.PKId,
                                        UserId = userRebate.User.ParentUserId.Value,
                                        ChildUserId = user.UserId,
                                        Rank = dbBet.Rank,
                                        Num = dbBet.Num,
                                        BonusType = Data.Enums.BonusType.Rebate,
                                        Amount = Math.Round(dbBet.Amount * (agentRebate - rebate), 4),//代理退水 - 给会员的退水
                                        IsSettlementDone = true // 直接设置成已结算
                                    });
                                }
                                #endregion


                                if (agentUser.ParentUserId.HasValue)
                                {
                                    #region 总代理退水

                                    var generalAgentUserRebate = db.UserRebate
                                        .Where(r => r.UserId == agentUser.ParentUserId && r.RebateNo == dbBet.Num).FirstOrDefault();
                                    var generalAgentRebate = UserRebateService.GetDefaultRebate(generalAgentUserRebate, user.DefaultRebateType); // 使用下注用户的默认盘
                                    if (generalAgentRebate - agentRebate > 0)
                                    {
                                        generalAgentBonuses.Add(new PKBonus
                                        {
                                            BetId = dbBet.BetId,
                                            PKId = pk.PKId,
                                            UserId = agentUser.ParentUserId.Value,
                                            ChildUserId = agentUser.UserId,
                                            Rank = dbBet.Rank,
                                            Num = dbBet.Num,
                                            BonusType = Data.Enums.BonusType.Rebate,
                                            Amount = Math.Round(dbBet.Amount * (generalAgentRebate - agentRebate), 4),//总代理退水 - 给代理的退水
                                            IsSettlementDone = true // 直接设置成已结算
                                        });
                                    }

                                    #endregion
                                }
                            }
                        }
                        if (bonuses.Count > 0)
                        {
                            // 保存会员退水奖金
                            db.PKBonus.AddRange(bonuses);
                            db.SaveChanges();

                            // 奖金加到余额
                            foreach (var bonus in bonuses)
                            {
                                userExtensionService.AddAmount(bonus.UserId, bonus.Amount);
                            }
                        }
                        if (agentBonuses.Count > 0)
                        {
                            // 保存会员退水奖金
                            db.PKBonus.AddRange(agentBonuses);
                            db.SaveChanges();

                            // 奖金加到余额
                            foreach (var bonus in agentBonuses)
                            {
                                userExtensionService.AddAmount(bonus.UserId, bonus.Amount);
                            }
                        }
                        if (generalAgentBonuses.Count > 0)
                        {
                            // 保存会员退水奖金
                            db.PKBonus.AddRange(generalAgentBonuses);
                            db.SaveChanges();

                            // 奖金加到余额
                            foreach (var bonus in generalAgentBonuses)
                            {
                                userExtensionService.AddAmount(bonus.UserId, bonus.Amount);
                            }
                        }
                    }
                }
            }
        }

    }
}