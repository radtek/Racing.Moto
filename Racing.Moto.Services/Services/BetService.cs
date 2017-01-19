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

namespace Racing.Moto.Services
{
    public class BetService : BaseServcice
    {
        public void AddBets(List<Bet> bets)
        {
            db.Bet.AddRange(bets);
            db.SaveChanges();
        }

        #region 计算名次
        /// <summary>
        /// 计算名次车号
        /// 既第一名是几号车, 第二名是几号车...
        /// </summary>
        /// <param name="pkId">期号</param>
        public List<int> CalculateRanks(int pkId)
        {
            // 下注金额求和
            var betAmounts = GetBetAmounts(pkId);
            // 计算奖池百分比
            var betRates = CalculateBetRates(betAmounts);
            // 奖池百分比 转换成 矩阵, 用于计算最小中奖名次 [TODO]大于1必不中
            var matrix = GetMatrix(betRates);
            // 计算最小中奖名次
            var minCostMatrix = new HungarianAlgorithm(matrix).Run();

            // 验证最小中奖名次的 [奖池百分比] 之和 是否小于 100%, 小于则返回, 否则返回null
            if (IsValidRanks(matrix, minCostMatrix))
            {
                // 计算名次:比赛结果:车号顺序
                var motoOrders = GetRanks(minCostMatrix);

                return motoOrders;
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
            sql.AppendLine("SELECT [Rank],[Num], SUM(Amount) Amount");
            sql.AppendLine("FROM (");
            sql.AppendLine("	SELECT [B].*, [PR].[Rate]");
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
                    var big = betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Big).FirstOrDefault();//第n名选大
                    var small = betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Small).FirstOrDefault();//第n名选小
                    var odd = betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Odd).FirstOrDefault();//第n名选单
                    var even = betAmounts.Where(ba => ba.Rank == rank && ba.Num == BetNumConst.Even).FirstOrDefault();//第n名选双

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
        private List<int> GetRanks(int[] minCostMatrix)
        {
            return minCostMatrix.Select(m => m + 1).ToList();
        }
        #endregion

        #endregion
    }
}
