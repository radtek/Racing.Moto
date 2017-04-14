using Racing.Moto.Core.GraphAlgorithms;
using Racing.Moto.Services.Caches;
using Racing.Moto.Services.Constants;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Racing.Moto.Core.Utils;
using Racing.Moto.Core.Extentions;
using System.Data.Entity;

namespace Racing.Moto.Services
{
    public class BetService : BaseServcice
    {
        /// <summary>
        /// 同一个PK,可以多次下注
        /// </summary>
        public void SaveBets(int pkId, int userId, List<Bet> bets)
        {
            //int? agentUserId = null;            // 报表使用
            //int? genearlAgentUserId = null;     // 报表使用

            var user = db.User
                .Include(nameof(User.ParentUser))
                .Include(nameof(User.UserRoles))
                .Where(u => u.UserId == userId).First();

            //// 下注用户是会员
            //if (user.UserRoles.First().RoleId == RoleConst.Role_Id_Member)
            //{
            //    agentUserId = user.ParentUserId;
            //    genearlAgentUserId = db.User.Where(u => u.UserId == user.ParentUser.ParentUserId).First().UserId;
            //}
            //// 下注用户是代理
            //else if (user.UserRoles.First().RoleId == RoleConst.Role_Id_Agent)
            //{
            //    agentUserId = user.UserId;
            //    genearlAgentUserId = user.ParentUserId;
            //}
            //// 下注用户是代理
            //else if (user.UserRoles.First().RoleId == RoleConst.Role_Id_General_Agent)
            //{
            //    agentUserId = user.UserId;
            //    genearlAgentUserId = user.UserId;
            //}

            bets.ForEach(b =>
            {
                b.PKId = pkId;
                b.UserId = userId;
                b.CreateTime = DateTime.Now;
                b.BetItems = new List<BetItem>
                {
                    new BetItem
                    {
                        Rank = b.Rank,
                        Num = b.Num,
                        Amount = b.Amount,
                        CreateTime = DateTime.Now
                    }
                };
            });

            var dbBets = db.Bet.Where(b => b.PKId == pkId && b.UserId == userId).ToList();
            if (dbBets.Count == 0)
            {
                // 第一次添加
                db.Bet.AddRange(bets);
            }
            else
            {
                // 追加投注
                foreach (var dbBet in dbBets)
                {
                    var newBet = bets.Where(b => b.Rank == dbBet.Rank && b.Num == dbBet.Num).FirstOrDefault();
                    if (newBet != null)
                    {
                        // 追加投注
                        dbBet.Amount += newBet.Amount;

                        // 追加投注条目
                        var betItem = new BetItem
                        {
                            BetId = dbBet.BetId,
                            Rank = newBet.Rank,
                            Num = newBet.Num,
                            Amount = newBet.Amount,
                            CreateTime = DateTime.Now
                        };
                        db.BetItem.Add(betItem);
                    }
                }

                // 新投注
                foreach (var bet in bets)
                {
                    var dbBet = dbBets.Where(b => b.Rank == bet.Rank && b.Num == bet.Num).FirstOrDefault();
                    if (dbBet == null)
                    {
                        db.Bet.Add(bet);// 新投注
                    }
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// 取当前用户的投注
        /// </summary>
        public List<Bet> GetBets(int pkId, int userId)
        {
            return db.Bet.Include(nameof(Bet.BetItems))
                .Where(b => b.PKId == pkId && b.UserId == userId).ToList();
        }

        /// <summary>
        /// 取投注, 计算奖金使用
        /// </summary>
        public List<Bet> GetBets(int pkId, int rank, int num)
        {
            return db.Bet.Where(b => b.PKId == pkId && b.Rank == rank && b.Num == num).ToList();
        }

        #region 转换PK表中Ranks

        /// <summary>
        /// 转换PK表中Ranks(字符串名次)为Bet, 用于计算奖金
        /// </summary>
        public List<Bet> ConvertRanksToBets(string ranks)
        {
            var bets = new List<Bet>();

            var rankList = ranks.Split(',');
            for (var i = 0; i < rankList.Length; i++)
            {
                var rank = i + 1;
                var num = Convert.ToInt32(rankList[i]);

                bets.Add(new Bet
                {
                    Rank = rank,
                    Num = num
                });

                bets.AddRange(GetBSOEBets(rank, num));
            }

            return bets;
        }

        /// <summary>
        /// 大小单双
        /// 名次+车号 计算大小单双
        /// 如: 第一名是5号车, 则第一名是小和单
        /// </summary>
        /// <param name="rank">名次</param>
        /// <param name="num">车号 1-10</param>
        /// <returns></returns>
        private List<Bet> GetBSOEBets(int rank, int num)
        {
            var bets = new List<Bet>();

            // 大小
            var bSNum = (num > 5) ? BetNumConst.Big : BetNumConst.Small;
            bets.Add(new Bet
            {
                Rank = rank,
                Num = bSNum
            });

            // 单双
            var oENum = (num % 2 != 0) ? BetNumConst.Odd : BetNumConst.Even;
            bets.Add(new Bet
            {
                Rank = rank,
                Num = oENum
            });

            return bets;
        }

        #endregion

        #region 计算名次
        /// <summary>
        /// 计算名次车号
        /// 既第一名是几号车, 第二名是几号车...
        /// </summary>
        /// <param name="pkId">期号</param>
        public List<int> CalculateRanks(int pkId)
        {
            // 下注金额按(名次+车号)求和
            var betAmounts = GetBetAmounts(pkId);
            // 没有人下注, 返回随机名次
            if (betAmounts.Sum(a => a.Amount) == 0)
            {
                var ranks = RandomUtil.GetRandomList(1, 10);

                return ranks;
            }

            // 计算奖池百分比
            var betRates = CalculateBetRates(betAmounts);
            // 奖池百分比 转换成 矩阵, 用于计算最小中奖名次 [TODO]大于1必不中
            var matrix = GetMatrix(betRates);

            /////////////////////test//////////////////////////////
            //var matrixStr = GetMatrixStr(matrix);
            ////////////////////test///////////////////////////////

            // 计算最小中奖名次
            var minCostMatrix = new HungarianAlgorithm(matrix).Run();

            // 验证最小中奖名次的 [奖池百分比] 之和 是否小于 100%, 小于则返回, 否则返回null
            if (IsValidRanks(matrix, minCostMatrix))
            {
                // 计算名次:比赛结果:车号顺序
                var ranks = GetRanks(minCostMatrix);

                return ranks;
            }
            else
            {
                return null;
            }
        }

        #region Private Methods
        /// <summary>
        /// 下注金额求和
        /// 10行14列(10个车号+大小单双)数据: 10行名次, 14列(10个车号+大小单双)下注, 用于计算 奖池百分比（奖池占有率）
        /// </summary>
        private List<BetAmountModel> GetBetAmounts(int pkId)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT [Rank], [Num], SUM(Amount) Amount, SUM(RateAmount) RateAmount");
            sql.AppendLine("FROM (");
            sql.AppendLine("	SELECT [B].[Rank], [B].[Num], [B].[Amount], [B].[Amount] * [PR].[Rate] AS RateAmount, [PR].[Rate]");
            sql.AppendLine("	FROM [dbo].[Bet] AS [B]");
            sql.AppendLine("	INNER JOIN [dbo].[PKRate] AS [PR] ON [B].[PKId] = [PR].[PKId] AND [B].[Rank] = [PR].[Rank] AND [B].[Num] = [PR].[Num]");
            sql.AppendLine("	WHERE [B].[PKId] = " + pkId);
            sql.AppendLine(") AS TEMP");
            sql.AppendLine("GROUP BY [Rank],[Num]");

            var amounts = db.Database.SqlQuery<BetAmountModel>(sql.ToString()).ToList();

            return amounts;
        }

        /// <summary>
        /// 计算奖池百分比
        /// 大于1的为必不中
        /// </summary>
        /// <returns></returns>
        private List<BetRateModel> CalculateBetRates(List<BetAmountModel> betAmounts)
        {
            var betRates = new List<BetRateModel>();

            var totalAmount = betAmounts.Sum(ba => ba.Amount);//押注总额
            var bonusAmount = totalAmount * (1 - AppConfigCache.Rate_Admin - AppConfigCache.Rate_Return);// 1- 吃二出八 - 退水

            for (var rank = 1; rank <= 10; rank++)// 10个名次
            {
                for (var num = 1; num <= 10; num++)// 10个车号
                {
                    var bet = betAmounts.Where(ba => ba.Rank == rank && ba.Num == num).FirstOrDefault();// 第n名选m号车的                    
                    var big = num > 5 ? betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Big).FirstOrDefault() : null;//第n名选大
                    var small = num <= 5 ? betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Small).FirstOrDefault() : null;//第n名选小
                    var odd = num % 2 != 0 ? betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Odd).FirstOrDefault() : null;//第n名选单
                    var even = num % 2 == 0 ? betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Even).FirstOrDefault() : null;//第n名选双

                    var amount = GetAmount(bet) + GetAmount(big) + GetAmount(small) + GetAmount(odd) + GetAmount(even);

                    var rate = Math.Round(amount / bonusAmount, 4);//保留四位小数
                    betRates.Add(new BetRateModel
                    {
                        Rank = rank,
                        Num = num,
                        Rate = rate,
                        IsValid = rate < 1  //大于1的为必不中
                    });
                }
            }

            return betRates;
        }

        private decimal GetAmount(BetAmountModel model)
        {
            return model != null ? model.Amount : 0;
        }

        /// <summary>
        /// 奖池百分比 转换成 矩阵, 用于计算最小中奖名次
        /// decimal * 100 取整
        /// </summary>
        private int[,] GetMatrix(List<BetRateModel> betRates)
        {
            var matrix = new int[10, 10];

            for (var rank = 1; rank <= 10; rank++)// 10个名次
            {
                for (var num = 1; num <= 10; num++)// 10个车号
                {
                    var rate = betRates.Where(r => r.Rank == rank && r.Num == num).FirstOrDefault();
                    matrix[rank - 1, num - 1] = rate != null ? Decimal.ToInt32(rate.Rate * 100) : 0;
                }
            }

            return matrix;
        }

        private string GetMatrixStr(int[,] matrix)
        {
            StringBuilder sb = new StringBuilder();

            for (var rank = 1; rank <= 10; rank++)// 10个名次
            {
                var list = new List<string>();
                for (var num = 1; num <= 10; num++)// 10个车号
                {
                    list.Add(matrix[rank - 1, num - 1].ToString().PadLeft(5, ' '));
                }
                sb.AppendLine(string.Join(",", list));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 验证最小中奖名次的 [奖池百分比] 之和 是否小于 100%
        /// </summary>
        private bool IsValidRanks(int[,] matrix, int[] minCostMatrix)
        {
            /*
             Matrix:
                0     40    0     10    30
                10    0     30    20    30
                0     40    20    10    30
                40    10    40    30    0
                30    40    30    40    10
             Array:
                0    1    3    4    2 

                第0行第0列, 第1行第1列, 第2行第3列, 第3行第4列, 第4行第2列
                0 + 0 + 10 + 0 + 40 = 50 
             */

            var rate = 0;
            for (var i = 0; i < minCostMatrix.Length; i++)
            {
                rate += matrix[i, minCostMatrix[i]];
            }

            return rate < 100;
        }

        /// <summary>
        /// 计算名次
        /// minCostMatrix 的计数从0开始, 加1返回
        /// </summary>
        /// <returns></returns>
        private List<int> GetRanks(int[] minCostMatrix, List<BetRateModel> betRates)
        {
            var ranks = minCostMatrix.Select(m => m + 1).ToList();

            // 自然数升序的情况, 通常是因为无论什么顺序, 奖金都相同, 如只有一个用户下注1号车的10个名次都是大,且金额相同
            // 此时生成个随机数返回
            var rankStr = string.Join(",", ranks);
            if (rankStr == "1,2,3,4,5,6,7,8,9,10")
            {
                ranks = RandomUtil.GetRandomList(1, 10);
            }
            else
            {
                // 用户下注1-9号车, 10没下注, 结果为 10,1,2,3,4,5,6,7,8,9, 后9位如果奖金相同, 后9位生成随机顺序
                var motoRates = betRates.GroupBy(r => r.Num).Select(g => new { Num = g.Key, Val = g.Sum(r => r.Rate) }).ToList();
                // 按奖金分组, 如果每组长度有超过2的, 这组如果在计算结果中是按升序存在的, 则打乱顺序
                var valRates = motoRates.GroupBy(r => r.Val).Select(g => new { Val = g.Key, Count = g.Count() }).ToList();
                foreach (var valRate in valRates)
                {
                    if (valRate.Count > 2)
                    {
                        var motoNums = motoRates.Where(r => r.Val == valRate.Val).Select(r => r.Num).OrderBy(num => num).ToList();
                        var motoNumsStr = string.Join(",", motoNums);
                        if (rankStr.IndexOf(motoNumsStr) > -1)
                        {
                            var disruptOrder = RandomUtil.DisruptOrder(motoNums);
                            rankStr.Replace(rankStr, string.Join(",", disruptOrder));
                        }
                    }
                }
            }

            return ranks;
        }
        #endregion

        #endregion

        /// <summary>
        /// 取用户某期已下注金额
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="pkId">期Id</param>
        /// <returns>已下注金额</returns>
        public List<BetAmountModel> GetSumAmounts(int userId, int pkId)
        {
            return db.Bet
                .Where(b => b.UserId == userId && b.PKId == pkId)
                .GroupBy(b => new { b.Rank, b.Num })
                .Select(g => new BetAmountModel
                {
                    Rank = g.Key.Rank,
                    Num = g.Key.Num,
                    Amount = g.Sum(b => b.Amount)
                }).ToList();
        }

        /// <summary>
        /// 每局结束后, 更新已结算标志 IsSettlementDone
        /// </summary>
        public void UpdateSettlementDone()
        {
            var dbBets = db.Bet.Where(b => !b.IsSettlementDone && DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) > 0).ToList();
            dbBets.ForEach(b => b.IsSettlementDone = true);
            db.SaveChanges();
        }


        #region User Report

        #region User Report Bet
        /// <summary>
        /// 用户.今日已结/未结明细
        /// 今日已结在封盘时job生成, 包括奖金+退水
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PagerResult<Bet> GetUserBetReport(UserReportSearchModel model)
        {
            var query = db.Bet.Where(b => b.UserId == model.UserId
                && b.IsSettlementDone == model.IsSettlementDone
                && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0);//今日


            if (model.IsSettlementDone)
            {
                //已结
                query = query.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) > 0);
            }
            else
            {
                //未结
                query = query.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) < 0);
            }

            var result = query
                .OrderByDescending(b => b.BetId)
                .Pager(model.PageIndex, model.PageSize);

            // 奖金
            if (model.IsSettlementDone)
            {
                //已结
                var betIds = result.Items.Select(b => b.BetId).ToList();
                var pkBonus = db.PKBonus.Where(b => betIds.Contains(b.BetId)).ToList();
                foreach (var bet in result.Items)
                {
                    var bonus = pkBonus.Where(b => b.BetId == bet.BetId).ToList();
                    var amount = bonus.Count() > 0 ? bonus.Sum(b => b.Amount) : 0;
                    bet.BonusAmount = amount - bet.Amount;  // 退水后奖金 = 中奖金额+退水-本金, 奖金+退水 job生成
                }
            }
            else
            {
                //未结
                var pkIds = result.Items.Select(b => b.PKId).ToList();
                var pkRates = db.PKRate.Where(r => pkIds.Contains(r.PKId)).ToList();
                var userRebates = db.UserRebate.Include(nameof(UserRebate.User)).Where(u => u.UserId == model.UserId).ToList();
                foreach (var bet in result.Items)
                {
                    var pkRate = pkRates.Where(r => r.PKId == bet.PKId && r.Num == bet.Num && r.Rank == bet.Rank).First();
                    var bonus = bet.Amount * pkRate.Rate;
                    var userRebate = userRebates.Where(u => u.UserId == bet.UserId && u.RebateNo == bet.Num).First();
                    var rebate = bet.Amount * UserRebateService.GetDefaultRebate(userRebate, userRebate.User.DefaultRebateType);
                    bet.BonusAmount = bonus + rebate - bet.Amount;  // 退水后奖金 = 中奖金额+退水-本金, 奖金+退水 job生成
                }
            }

            return result;
        }

        /// <summary>
        /// 用户.今日已结/未结明细 统计
        /// 今日已结在封盘时job生成, 包括奖金+退水
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserBonusReportStatistics GetUserBetReportStatistics(UserReportSearchModel model)
        {
            var statistics = new UserBonusReportStatistics();

            var queryBet = db.Bet.Where(b => b.UserId == model.UserId
                && b.IsSettlementDone == model.IsSettlementDone     //今日已结/未结明细 
                && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0);  //今日


            #region Bonus 今日已结在封盘时job生成, 包括奖金+退水
            //var queryBonus = db.PKBonus
            //    .Where(b => b.UserId == model.UserId && b.IsSettlementDone == model.IsSettlementDone && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0);

            //if (model.IsSettlementDone)
            //{
            //    //已结
            //    queryBonus = queryBonus.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) > 0);
            //}
            //else
            //{
            //    //未结
            //    queryBonus = queryBonus.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) < 0);
            //}
            #endregion

            // 注单数量
            statistics.BetCount = queryBet.Count();

            // 下注金额
            statistics.BetAmount = statistics.BetCount > 0 ? queryBet.Sum(b => b.Amount) : 0;

            // 中奖金额+退水-本金, 奖金+退水 job生成
            if (model.IsSettlementDone)
            {
                //今日已结在封盘时job生成, 包括奖金+退水
                var queryBonus = db.PKBonus
                    .Where(b => b.UserId == model.UserId && b.IsSettlementDone == model.IsSettlementDone && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0);

                //已结
                queryBonus = queryBonus.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) > 0);

                statistics.BonusAmount = queryBonus.Any() ? queryBonus.Sum(b => b.Amount) - statistics.BetAmount : 0 - statistics.BetAmount;
            }
            else
            {
                //未结(可赢金额),按一定中奖计算 
                var bets = queryBet.ToList();
                var pkIds = bets.Select(b => b.PKId).ToList();
                var pkRates = db.PKRate.Where(r => pkIds.Contains(r.PKId)).ToList();
                var userRebates = db.UserRebate.Include(nameof(UserRebate.User)).Where(u => u.UserId == model.UserId).ToList();
                foreach (var bet in bets)
                {
                    var pkRate = pkRates.Where(r => r.PKId == bet.PKId && r.Num == bet.Num && r.Rank == bet.Rank).First();
                    var bonus = bet.Amount * pkRate.Rate;
                    var userRebate = userRebates.Where(u => u.UserId == bet.UserId && u.RebateNo == bet.Num).First();
                    var rebate = bet.Amount * UserRebateService.GetDefaultRebate(userRebate, userRebate.User.DefaultRebateType);
                    bet.BonusAmount = bonus + rebate - bet.Amount;  // 退水后奖金 = 中奖金额+退水-本金, 奖金+退水 job生成
                }

                statistics.BonusAmount = bets.Any() ? bets.Sum(b => b.BonusAmount) : 0;
            }


            return statistics;
        }

        #endregion


        #region User Report BetItem
        /// <summary>
        /// 用户.今日已结/未结明细
        /// 今日已结在封盘时job生成, 包括奖金+退水
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public PagerResult<BetItem> GetUserBetItemReport(UserReportSearchModel model)
        {
            var query = db.BetItem
                .Include(nameof(BetItem.Bet))
                .Where(b => b.Bet.UserId == model.UserId
                    && b.Bet.IsSettlementDone == model.IsSettlementDone
                    && DbFunctions.DiffDays(b.Bet.PK.EndTime, DateTime.Now) == 0);//今日


            if (model.IsSettlementDone)
            {
                //已结
                query = query.Where(b => DbFunctions.DiffSeconds(b.Bet.PK.EndTime, DateTime.Now) > 0);
            }
            else
            {
                //未结
                query = query.Where(b => DbFunctions.DiffSeconds(b.Bet.PK.EndTime, DateTime.Now) < 0);
            }

            var result = query
                .OrderByDescending(b => b.BetId).ThenBy(b => b.BetItemId)
                .Pager(model.PageIndex, model.PageSize);


            var pkIds = result.Items.Select(b => b.Bet.PKId).ToList();
            var pkRates = db.PKRate.Where(r => pkIds.Contains(r.PKId)).ToList();
            var userRebates = db.UserRebate.Include(nameof(UserRebate.User)).Where(u => u.UserId == model.UserId).ToList();
            // 奖金
            if (model.IsSettlementDone)
            {
                //已结
                var betIds = result.Items.Select(b => b.BetId).ToList();
                var pkBonus = db.PKBonus.Where(b => betIds.Contains(b.BetId)).ToList();// 多条 BetItem 生成的退水+奖金
                foreach (var betItem in result.Items)
                {
                    var pkRate = pkRates.Where(r => r.PKId == betItem.Bet.PKId && r.Num == betItem.Num && r.Rank == betItem.Rank).First();
                    var hasBonus = pkBonus.Where(b => b.BetId == betItem.BetId && b.Rank == betItem.Rank && b.Num == betItem.Num && b.BonusType == Data.Enums.BonusType.Bonus).Any();
                    var bonusAmount = hasBonus ? betItem.Amount * pkRate.Rate : 0;

                    var userRebate = userRebates.Where(u => u.UserId == betItem.Bet.UserId && u.RebateNo == betItem.Num).First();
                    var rebateAmount = betItem.Amount * UserRebateService.GetDefaultRebate(userRebate, userRebate.User.DefaultRebateType);

                    betItem.BonusAmount = bonusAmount + rebateAmount - betItem.Amount;  // 退水后奖金 = 中奖金额+退水-本金, 奖金+退水 job生成
                }
            }
            else
            {
                //未结
                foreach (var betItem in result.Items)
                {
                    var pkRate = pkRates.Where(r => r.PKId == betItem.Bet.PKId && r.Num == betItem.Num && r.Rank == betItem.Rank).First();
                    var bonusAmount = betItem.Amount * pkRate.Rate;
                    var userRebate = userRebates.Where(u => u.UserId == betItem.Bet.UserId && u.RebateNo == betItem.Num).First();
                    var rebateAmount = betItem.Amount * UserRebateService.GetDefaultRebate(userRebate, userRebate.User.DefaultRebateType);
                    betItem.BonusAmount = bonusAmount + rebateAmount - betItem.Amount;  // 退水后奖金 = 中奖金额+退水-本金
                }
            }

            return result;
        }
        /// <summary>
        /// 用户.今日已结/未结明细 统计
        /// 今日已结在封盘时job生成, 包括奖金+退水
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserBonusReportStatistics GetUserBetItemReportStatistics(UserReportSearchModel model)
        {
            var statistics = new UserBonusReportStatistics();

            var queryBet = db.BetItem.Include(nameof(BetItem.Bet)).Where(b => b.Bet.UserId == model.UserId
                && b.Bet.IsSettlementDone == model.IsSettlementDone     //今日已结/未结明细 
                && DbFunctions.DiffDays(b.Bet.PK.EndTime, DateTime.Now) == 0);  //今日


            #region Bonus 今日已结在封盘时job生成, 包括奖金+退水
            //var queryBonus = db.PKBonus
            //    .Where(b => b.UserId == model.UserId && b.IsSettlementDone == model.IsSettlementDone && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0);

            //if (model.IsSettlementDone)
            //{
            //    //已结
            //    queryBonus = queryBonus.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) > 0);
            //}
            //else
            //{
            //    //未结
            //    queryBonus = queryBonus.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) < 0);
            //}
            #endregion

            // 注单数量
            statistics.BetCount = queryBet.Count();

            // 下注金额
            statistics.BetAmount = statistics.BetCount > 0 ? queryBet.Sum(b => b.Amount) : 0;

            // 中奖金额+退水-本金, 奖金+退水 job生成
            if (model.IsSettlementDone)
            {
                //今日已结在封盘时job生成, 包括奖金+退水
                var queryBonus = db.PKBonus
                    .Where(b => b.UserId == model.UserId && b.IsSettlementDone == model.IsSettlementDone && DbFunctions.DiffDays(b.PK.EndTime, DateTime.Now) == 0);

                //已结
                queryBonus = queryBonus.Where(b => DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) > 0);

                statistics.BonusAmount = queryBonus.Any() ? queryBonus.Sum(b => b.Amount) - statistics.BetAmount : 0 - statistics.BetAmount;
            }
            else
            {
                //未结(可赢金额),按一定中奖计算 
                var bets = queryBet.ToList();
                var pkIds = bets.Select(b => b.Bet.PKId).ToList();
                var pkRates = db.PKRate.Where(r => pkIds.Contains(r.PKId)).ToList();
                var userRebates = db.UserRebate.Include(nameof(UserRebate.User)).Where(u => u.UserId == model.UserId).ToList();
                foreach (var betItem in bets)
                {
                    var pkRate = pkRates.Where(r => r.PKId == betItem.Bet.PKId && r.Num == betItem.Num && r.Rank == betItem.Rank).First();
                    var bonus = betItem.Amount * pkRate.Rate;
                    var userRebate = userRebates.Where(u => u.UserId == betItem.Bet.UserId && u.RebateNo == betItem.Num).First();
                    var rebate = betItem.Amount * UserRebateService.GetDefaultRebate(userRebate, userRebate.User.DefaultRebateType);
                    betItem.BonusAmount = bonus + rebate - betItem.Amount;  // 退水后奖金 = 中奖金额+退水-本金, 奖金+退水 job生成
                }

                statistics.BonusAmount = bets.Any() ? bets.Sum(b => b.BonusAmount) : 0;
            }


            return statistics;
        }
        #endregion


        #endregion
    }
}
