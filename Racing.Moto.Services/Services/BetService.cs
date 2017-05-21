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
using Racing.Moto.Data;

namespace Racing.Moto.Services
{
    public class BetService : BaseServcice
    {
        /// <summary>
        /// 同一个PK,可以多次下注
        /// </summary>
        public void SaveBets(int pkId, int userId, List<Bet> bets)
        {
            using (var db = new RacingDbContext())
            {
                int? agentUserId = null;            // 报表使用
                int? generalAgentUserId = null;     // 报表使用

                var user = db.User
                    .Include(nameof(User.ParentUser))
                    .Include(nameof(User.UserRoles))
                    .Where(u => u.UserId == userId).First();

                // 下注用户是会员
                if (user.UserRoles.First().RoleId == RoleConst.Role_Id_Member)
                {
                    agentUserId = user.ParentUserId;
                    generalAgentUserId = db.User.Where(u => u.UserId == user.ParentUser.ParentUserId).First().UserId;
                }
                // 下注用户是代理
                else if (user.UserRoles.First().RoleId == RoleConst.Role_Id_Agent)
                {
                    agentUserId = user.UserId;
                    generalAgentUserId = user.ParentUserId;
                }
                // 下注用户是代理
                else if (user.UserRoles.First().RoleId == RoleConst.Role_Id_General_Agent)
                {
                    agentUserId = user.UserId;
                    generalAgentUserId = user.UserId;
                }

                bets.ForEach(b =>
                {
                    b.PKId = pkId;
                    b.UserId = userId;
                    b.CreateTime = DateTime.Now;
                    b.AgentUserId = agentUserId;
                    b.GeneralAgentUserId = generalAgentUserId;

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
        }

        /// <summary>
        /// 取当前用户的投注
        /// </summary>
        public List<Bet> GetBets(int pkId, int userId)
        {
            using (var db = new RacingDbContext())
            {
                return db.Bet.Include(nameof(Bet.BetItems))
                    .Where(b => b.PKId == pkId && b.UserId == userId).ToList();
            }
        }

        /// <summary>
        /// 取投注, 计算奖金使用
        /// </summary>
        public List<Bet> GetBets(int pkId, int rank, int num)
        {
            using (var db = new RacingDbContext())
            {
                return db.Bet.Where(b => b.PKId == pkId && b.Rank == rank && b.Num == num).ToList();
            }
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

            // 计算下注百分比
            var betRates = CalculateBetRates(betAmounts);
            // 奖池百分比 转换成 矩阵, 用于计算最小中奖名次 [TODO]大于1必不中
            var matrix = GetMatrix(betRates);
            //// 名次下注百分比
            //var rankRates = GetRankRates(betRates);

            /////////////////////test//////////////////////////////
            //var matrixStr = GetMatrixStr(matrix);
            ////////////////////test///////////////////////////////

            // 计算最小中奖名次
            var minCostMatrix = new HungarianAlgorithm(matrix).Run();

            // 验证最小中奖名次的 [奖池百分比] 之和 是否小于 100%, 小于则返回, 否则返回null
            if (IsValidRanks(matrix, minCostMatrix))
            {
                // 计算名次:比赛结果:车号顺序
                var ranks = GetRanks(minCostMatrix, betRates);

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
            using (var db = new RacingDbContext())
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
            //var bonusAmount = totalAmount * (1 - AppConfigCache.Rate_Admin - AppConfigCache.Rate_Return);// 1- 吃二出八 - 退水
            var bonusAmount = totalAmount;

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
            //return model != null ? model.Amount : 0;
            return model != null ? model.RateAmount : 0;
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

        ///// <summary>
        ///// 名次中奖几率
        ///// </summary>
        //private List<RankRateModel> GetRankRates(List<BetRateModel> betRates)
        //{
        //    var models = new List<RankRateModel>();

        //    for (var rank = 1; rank <= 10; rank++)// 10个名次
        //    {
        //        var rate = betRates.Where(r => r.Rank == rank).Sum(r => r.Rate);
        //        models.Add(new RankRateModel { Rank = rank, Rate = rate });
        //    }

        //    return models;
        //}

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
                            rankStr = rankStr.Replace(motoNumsStr, string.Join(",", disruptOrder));
                        }

                        ranks = rankStr.Split(',').Select(r => int.Parse(r)).ToList();
                    }
                }

                //// 如果存在超过2个以上的连续数字, 如: 10,1,2,3,4,5,6,7,8,9 中的1,2,3,4,5,6,7,8,9 , 则将1,2,3,4,5,6,7,8,9打乱
                //ranks = ReOrderRanks(ranks);

                // 调高中奖几率
                ranks = ImproveWinningRate(betRates, ranks);
            }

            return ranks;
        }

        //如果存在超过2个以上的连续数字, 如: 10,1,2,3,4,5,6,7,8,9 中的1,2,3,4,5,6,7,8,9 , 则将1,2,3,4,5,6,7,8,9打乱
        private List<int> ReOrderRanks(List<int> ranks)
        {
            var ranksStr = string.Join(",", ranks);//10,1,2,3,4,5,6,7,8,9

            var max = ranks.Count;
            var maxRanks = new List<int>();
            for (var i = 1; i <= max; i++)
            {
                maxRanks.Add(i);
            }
            var maxRanksStr = string.Join(",", ranks);   // 1,2,3,4,5,6,7,8,9,10

            for (var len = max; len >= 3; len--)
            {
                var ranksByLen = GetRanksByLen(maxRanks, len);
                var rankSection = ranksByLen.Where(rl => ranksStr.IndexOf(rl) > -1).FirstOrDefault();
                if (rankSection != null)
                {
                    var rList = rankSection.Split(',').Select(r => int.Parse(r)).ToList();
                    var disruptOrder = RandomUtil.DisruptOrder(rList);
                    ranksStr = ranksStr.Replace(rankSection, string.Join(",", disruptOrder));
                    break;
                }
            }

            return ranksStr.Split(',').Select(r => int.Parse(r)).ToList(); ;
        }

        private List<string> GetRanksByLen(List<int> maxRanks, int len)
        {
            var ranks = new List<string>();

            var count = maxRanks.Count - len + 1;   // 如: maxRanks = 1,2,3,4,5,6,7,8,9,10, len =9, count=2, 既: 1,2,3,4,5,6,7,8,9 和2,3,4,5,6,7,8,9,10
            for (var i = 1; i <= count; i++)
            {
                var rankList = maxRanks.Where(r => r >= i && r < len + i).ToList();
                ranks.Add(string.Join(",", rankList));
            }

            return ranks;
        }

        /// <summary>
        /// 提高中奖率
        /// </summary>
        /// <returns></returns>
        private List<int> ImproveWinningRate(List<BetRateModel> betRates, List<int> ranks)
        {
            // 计算中奖比率

            var bonusRate = 1 - AppConfigCache.Rate_Admin - AppConfigCache.Rate_Return; // 吃二出八
            
            // 提高中奖率, 取 中奖比率< 出八 的放在中奖位置
            //betRates = betRates.Where(br => br.Rate > 0).OrderByDescending(br => br.Rate).ToList();//中奖比率倒序
            //var sumBetRate = 0M;
            //foreach (var betRate in betRates)
            //{
            //    sumBetRate += betRate.Rate;
            //    if (sumBetRate <= bonusRate)
            //    {
            //        var tempRanks = ResetRanks(ranks, betRate.Rank, betRate.Num);

            //        // 判断新名次是否超过中奖率
            //        if (IsValidRanks(tempRanks, betRates))
            //        {
            //            ranks = tempRanks;
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            //var startRank = 1;

            //// 取小于0.8(吃二出八) 且最大的比率
            //var maxRate = betRates.Where(br => br.Rate > 0 && br.Rate < bonusRate).OrderByDescending(br => br.Rate).FirstOrDefault();
            //if (maxRate != null)
            //{
            //    // 第一名设置成 此车号
            //    ranks = ResetRanks(ranks, maxRate.Rank, maxRate.Num);

            //    // 如果第一名已经设置, 下边循环从第二名开始
            //    startRank = 2;

            //    // 已经排过序的车号+比率
            //    rankedRates.Add(maxRate);
            //}

            var orderedBetRates = betRates.Where(br => br.Rate > 0 && br.Rate < bonusRate).OrderByDescending(br => br.Rate).ToList();

            var tempRanks = ranks.Select(r => r).ToList();
            var maxSumRankedRate = 0M;
            var tempSumRankedRate = 0M;
            foreach (var betRate in orderedBetRates)
            {
                tempRanks = ranks.Select(r => r).ToList();

                // 已经排过序的车号+比率
                var rankedRates = new List<BetRateModel>();

                // 第一名设置成 此车号
                tempRanks = ResetRanks(tempRanks, betRate.Rank, betRate.Num);
                
                // 已经排过序的车号+比率
                rankedRates.Add(betRate);


                // 如果第一名已经设置, 下边循环从第二名开始
                for (var rank = 2; rank <= 10; rank++)
                {
                    var rankedNums = rankedRates.Select(r => r.Num).ToList();
                    var sumRankedRate = rankedRates.Count > 0 ? rankedRates.Sum(r => r.Rate) : 0;
                    // 第n名 未排过序的 下注车号+比率 中奖比率倒序
                    var rankRates = betRates.Where(br => br.Rate > 0 && br.Rank == rank && !rankedNums.Contains(br.Num)).OrderByDescending(br => br.Rate).ToList();
                    foreach (var rankRate in rankRates)
                    {
                        if (rankRate.Rate + sumRankedRate <= bonusRate)
                        {
                            // 第n名设置成 此车号
                            tempRanks = ResetRanks(tempRanks, rankRate.Rank, rankRate.Num);

                            // 已经排过序的车号+比率
                            rankedRates.Add(rankRate);
                            break;
                        }
                    }
                }
                if(tempSumRankedRate > maxSumRankedRate)
                {
                    maxSumRankedRate = tempSumRankedRate;
                }
                tempSumRankedRate = rankedRates.Count > 0 ? rankedRates.Sum(r => r.Rate) : 0;

                if(tempSumRankedRate > maxSumRankedRate)
                {
                    ranks = tempRanks.Select(r => r).ToList();
                }
            }

            return ranks;
        }
        
        /// <summary>
        /// 设置第rank名为第num号车
        /// </summary>
        /// <param name="ranks"></param>
        /// <param name="rank"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private List<int> ResetRanks(List<int> ranks, int rank, int num)
        {
            var index = 0;    // 当前车(num)所在位置
            for (var i = 0; i < ranks.Count; i++)
            {
                if (ranks[i] == num)
                {
                    index = i;
                    break;
                }
            }

            // 两辆车互换位置
            var motoNum = ranks[rank - 1];
            ranks[rank - 1] = num;
            ranks[index] = motoNum;

            return ranks;
        }

        private bool IsValidRanks(List<int> ranks, List<BetRateModel> betRates)
        {
            var sumRate = 0M;
            for (int i = 0; i < ranks.Count; i++)
            {
                sumRate += betRates.Where(r => r.Rank == (i + 1) && r.Num == ranks[i]).Sum(r => r.Rate);
            }

            return (sumRate <= 1 - AppConfigCache.Rate_Admin - AppConfigCache.Rate_Return);
        }

        /// <summary>
        /// 取 名次比率中 求和 小于 拨出比率(出八)的
        /// </summary>
        /// <param name="bonusRate">拨出比率(出八)</param>
        /// <param name="rankRates">名次比率</param>
        /// <returns></returns>
        private List<RankRateModel> GetImproveWinningRankRates(decimal bonusRate, List<RankRateModel> rankRates)
        {
            var newRankRates = new List<RankRateModel>();

            var rates = rankRates.Select(r => r.Rate).ToList();
            var minRates = GetClosestSum(bonusRate, rates, 0);
            foreach (var minRate in minRates)
            {
                var rankRate = rankRates.Where(r => r.Rate == minRate).First();
                newRankRates.Add(rankRate);
            }

            return newRankRates;
        }
        #region GetClosestSum
        /*
             List<int> input = new List<int>() { 3, 9, 8, 4, 5, 7, 10 };
             int targetSum = 15;
             SumUp(input, targetSum);             
        */
        private bool InRange(decimal num, decimal value, decimal range)
        {
            //return ((num >= value - range) && (num < value + range));
            return ((num >= range) && (num < value));
        }

        private List<decimal> GetClosestSum(decimal value, List<decimal> elements, decimal range)
        {
            elements.Sort();
            var possibleResults = new List<decimal>();
            for (int x = elements.Count - 1; x > 0; x--)
            {
                if (InRange(elements[x], value, range)) possibleResults.Add(elements[x]);
                decimal possibleResult = elements[x];
                for (int i = x - 1; i > -1; i--)
                {
                    possibleResult += elements[i];
                    if (possibleResult > (value + range - 1)) possibleResult -= elements[i];
                    if (InRange(possibleResult, value, range)) possibleResults.Add(possibleResult);
                }
            }
            decimal bestResult = -1;
            for (int x = 0; x < possibleResults.Count; x++)
            {
                if (bestResult == -1)
                    bestResult = possibleResults[x];
                if (Math.Abs(value - possibleResults[x]) < Math.Abs(value - bestResult))
                    bestResult = possibleResults[x];
            }
            return possibleResults;
        }

        #endregion

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
            using (var db = new RacingDbContext())
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
        }

        /// <summary>
        /// 每局结束后, 更新已结算标志 IsSettlementDone
        /// </summary>
        public void UpdateSettlementDone()
        {
            using (var db = new RacingDbContext())
            {
                var dbBets = db.Bet.Where(b => !b.IsSettlementDone && DbFunctions.DiffSeconds(b.PK.EndTime, DateTime.Now) > 0).ToList();
                dbBets.ForEach(b => b.IsSettlementDone = true);
                db.SaveChanges();
            }
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
            using (var db = new RacingDbContext())
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
        }

        /// <summary>
        /// 用户.今日已结/未结明细 统计
        /// 今日已结在封盘时job生成, 包括奖金+退水
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserBonusReportStatistics GetUserBetReportStatistics(UserReportSearchModel model)
        {
            using (var db = new RacingDbContext())
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
            using (var db = new RacingDbContext())
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
        }
        /// <summary>
        /// 用户.今日已结/未结明细 统计
        /// 今日已结在封盘时job生成, 包括奖金+退水
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserBonusReportStatistics GetUserBetItemReportStatistics(UserReportSearchModel model)
        {
            using (var db = new RacingDbContext())
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
        }
        #endregion


        #endregion

        #region 即時注單信息

        public BetStatisticModel GetBetStatistic()
        {
            using (var db = new RacingDbContext())
            {
                var model = new BetStatisticModel();

                var pk = new PKService().GetCurrentPK();
                //var pk = new PKService().GetPK(2129);
                if (pk == null)
                {
                    return null;
                }
                var pkModel = new PKService().ConvertToPKModel(pk);
                var pkRates = new PKRateService().GetPKRates(pk.PKId);

                var sql = "SELECT [Rank], Num, sum(Amount) Amount \n"
                    + "    FROM [Racing.Moto].[dbo].[Bet] WHERE PKId = {0} \n"
                    + "    GROUP BY Num, [Rank]";

                var dbBetAmounts = db.Database.SqlQuery<BetAmountModel>(string.Format(sql, pk.PKId)).ToList();
                model.BetAmountsAll = dbBetAmounts;

                //pk
                model.PKModel = pkModel;
                //pkRates
                model.PKRates = pkRates;
                model.TotalAmount = dbBetAmounts.Count > 0 ? dbBetAmounts.Sum(a => a.Amount) : 0;

                // betAmountRankModels
                //var betAmountRankModels = new List<BetAmountRankModel>();
                //for (var rank = 1; rank <= 10; rank++)
                //{
                //    betAmountRankModels.Add(new BetAmountRankModel
                //    {
                //        Rank = rank,
                //        BetAmounts = GetBetAmounts(dbBetAmounts, pkRates, rank)
                //    });
                //}
                //model.BetAmountRankModels = betAmountRankModels;

                model.BetAmounts1 = GetBetAmounts(dbBetAmounts, pkRates, 1);
                model.BetAmounts2 = GetBetAmounts(dbBetAmounts, pkRates, 2);
                model.BetAmounts3 = GetBetAmounts(dbBetAmounts, pkRates, 3);
                model.BetAmounts4 = GetBetAmounts(dbBetAmounts, pkRates, 4);
                model.BetAmounts5 = GetBetAmounts(dbBetAmounts, pkRates, 5);
                model.BetAmounts6 = GetBetAmounts(dbBetAmounts, pkRates, 6);
                model.BetAmounts7 = GetBetAmounts(dbBetAmounts, pkRates, 7);
                model.BetAmounts8 = GetBetAmounts(dbBetAmounts, pkRates, 8);
                model.BetAmounts9 = GetBetAmounts(dbBetAmounts, pkRates, 9);
                model.BetAmounts10 = GetBetAmounts(dbBetAmounts, pkRates, 10);

                //總投注額
                //model.BetAmount1 = model.BetAmounts1.Sum(a => a.Amount);
                //model.BetAmount2 = model.BetAmounts2.Sum(a => a.Amount);
                //model.BetAmount3 = model.BetAmounts3.Sum(a => a.Amount);
                //model.BetAmount4 = model.BetAmounts4.Sum(a => a.Amount);
                //model.BetAmount5 = model.BetAmounts5.Sum(a => a.Amount);
                //model.BetAmount6 = model.BetAmounts6.Sum(a => a.Amount);
                //model.BetAmount7 = model.BetAmounts7.Sum(a => a.Amount);
                //model.BetAmount8 = model.BetAmounts8.Sum(a => a.Amount);
                //model.BetAmount9 = model.BetAmounts9.Sum(a => a.Amount);
                //model.BetAmount10 = model.BetAmounts10.Sum(a => a.Amount);

                //最高盈利
                model.MaxProfit = model.TotalAmount * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit1 = model.BetAmount1 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit2 = model.BetAmount2 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit3 = model.BetAmount3 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit4 = model.BetAmount4 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit5 = model.BetAmount5 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit6 = model.BetAmount6 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit7 = model.BetAmount7 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit8 = model.BetAmount8 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit9 = model.BetAmount9 * (1 - AppConfigCache.Rate_Rebate_A);
                //model.MaxProfit10 = model.BetAmount10 * (1 - AppConfigCache.Rate_Rebate_A);

                //最高虧損
                model.MaxLoss = dbBetAmounts.Sum(a => a.Amount * a.PKRate) - model.TotalAmount;
                //model.MaxLoss1 = model.BetAmounts1.Sum(m => m.Amount * m.PKRate) - model.BetAmount1;
                //model.MaxLoss2 = model.BetAmounts2.Sum(m => m.Amount * m.PKRate) - model.BetAmount2;
                //model.MaxLoss3 = model.BetAmounts3.Sum(m => m.Amount * m.PKRate) - model.BetAmount3;
                //model.MaxLoss4 = model.BetAmounts4.Sum(m => m.Amount * m.PKRate) - model.BetAmount4;
                //model.MaxLoss5 = model.BetAmounts5.Sum(m => m.Amount * m.PKRate) - model.BetAmount5;
                //model.MaxLoss6 = model.BetAmounts6.Sum(m => m.Amount * m.PKRate) - model.BetAmount6;
                //model.MaxLoss7 = model.BetAmounts7.Sum(m => m.Amount * m.PKRate) - model.BetAmount7;
                //model.MaxLoss8 = model.BetAmounts8.Sum(m => m.Amount * m.PKRate) - model.BetAmount8;
                //model.MaxLoss9 = model.BetAmounts9.Sum(m => m.Amount * m.PKRate) - model.BetAmount9;
                //model.MaxLoss10 = model.BetAmounts10.Sum(m => m.Amount * m.PKRate) - model.BetAmount10;

                //RankAmountModels
                model.RankAmounts = GetRankAmountModels(dbBetAmounts);

                return model;
            }
        }

        private List<BetAmountModel> GetBetAmounts(List<BetAmountModel> dbBetAmounts, List<PKRate> pkRates, int rank)
        {
            var betAmounts = new List<BetAmountModel>();
            for (var num = 1; num <= 14; num++)
            {
                var pkRate = pkRates.Where(r => r.Rank == rank && r.Num == num).FirstOrDefault();
                var betAmount = dbBetAmounts.Where(bA => bA.Rank == rank && bA.Num == num).FirstOrDefault();
                if (betAmount == null)
                {
                    betAmount = new BetAmountModel
                    {
                        Rank = rank,
                        Num = num,
                        Amount = 0
                    };
                }
                betAmount.PKRate = pkRate.Rate;

                betAmounts.Add(betAmount);
            }

            return betAmounts;
        }

        private List<RankAmountModel> GetRankAmountModels(List<BetAmountModel> dbBetAmounts)
        {
            var models = new List<RankAmountModel>();
            for (var i = 1; i <= 10; i++)
            {
                var amountList = dbBetAmounts.Where(a => a.Rank == i).ToList();

                models.Add(new RankAmountModel
                {
                    Rank = i,
                    Amount = amountList.Count > 0 ? amountList.Sum(a => a.Amount) : 0
                });
            }
            return models;
        }

        private decimal GetLoss(List<BetAmountModel> models)
        {
            return models.Sum(m => m.Amount * m.PKRate);
        }

        #endregion
    }
}
