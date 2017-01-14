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
        #region old
        /// <summary>
        /// 计算名次车号
        /// 既第一名是几号车, 第二名是几号车...
        /// </summary>
        /// <param name="pkId">期号</param>
        public List<int> CalculateRanksOld(int pkId)
        {
            // 比赛结果:车号顺序
            var motoOrders = new List<int>();

            // 期
            var pk = db.PK.Include(nameof(PK.PKRates)).First();

            //押注的金额
            var betAmount = db.Bet.Where(b => b.PKId == pkId).Sum(b => b.Amount);

            // 10个车号押注的奖金
            var motoAmounts = GetMotoAmounts(pkId);

            /*
                百分比划分：
                    【公式：100%总金额 = 20%吃二出八 + 12%退水 +  68%奖池】 
                    20%为管理员所获得的利润
                    12%为各级的代理的提成
                    68%为奖池金额，按照公式返还给玩家
             */
            var maxBonus = betAmount * 0.68M;// 总押注金额的68%

            /*
                计算车号排序(名次)
                    1. 按车号押注金额升序排列既最小的奖金支出(不包括大小单双), 计算奖金, 结果 <= 总押注金额的68%  完成, 否则执行2
                    2. 第一名向下冒泡,  计算奖金, 结果 <= 总押注金额的68%  完成, 否则执行2
             */
            // 按押注金额升序排列既最小的奖金支出(不包括大小单双)
            motoOrders = motoAmounts.OrderBy(m => m.Amount).Select(m => m.MotoNo).ToList();
            var bonus = betAmount;
            var position = 0;
            while (bonus > betAmount * 0.68M)
            {
                bonus = CalculateBonus(pk, motoOrders);

                // 重新排序
                motoOrders = ReOrder(motoOrders, position);

                position = (position + 1) % 9;//移动9次之后到底, 从最顶端重新开始
            }

            return motoOrders;
        }

        /// <summary>
        /// 取车号 押注金额
        /// 10个车号的 押注金额
        /// </summary>
        /// <param name="pkId">期号</param>
        /// <returns></returns>
        private List<MotoAmountModel> GetMotoAmounts(int pkId)
        {
            // 10个车号押注的奖金
            var motoAmounts = new List<MotoAmountModel>();
            for (var i = 1; i <= 10; i++)
            {
                motoAmounts.Add(new MotoAmountModel
                {
                    MotoNo = i,
                    Amount = db.Bet.Where(b => b.PKId == pkId && b.Num == i).Sum(b => b.Amount)
                });
            }
            return motoAmounts;
        }

        /// <summary>
        /// 计算奖金
        /// </summary>
        /// <param name="pkRate">倍率</param>
        /// <param name="motoOrders">车号顺序</param>
        /// <returns></returns>
        private decimal CalculateBonus(PK pk, List<int> motoOrders)
        {
            var bonus = 0M;

            for (int i = 0; i < 10; i++)
            {
                var rank = i + 1;//名次
                //var motoAmount = motoAmounts.Where(m => m.MotoNo == motoOrders[i]).FirstOrDefault().Amount;
                var pkRate = pk.PKRates.Where(r => r.Rank == rank).First(); //本期倍率 [TODO]

                // 按车号计算
                for (var j = 0; j < motoOrders.Count; j++)
                {
                    var rate = PKRateService.GetRate(pkRate, motoOrders[j]);//名次的倍率
                    decimal numAmount = db.Bet.Where(b => b.Num == motoOrders[j] && b.Rank == rank).Sum(b => b.Amount);
                    bonus += numAmount * rate;
                }

                // 按大小单双计算
                var bigRate = PKRateService.GetRate(pkRate, BetNumConst.Big);
                decimal bigAmount = db.Bet.Where(b => b.Num == BetNumConst.Big && b.Rank == rank).Sum(b => b.Amount);
                bonus += bigAmount * bigRate;

                var smallRate = PKRateService.GetRate(pkRate, BetNumConst.Small);
                decimal smallAmount = db.Bet.Where(b => b.Num == BetNumConst.Small && b.Rank == rank).Sum(b => b.Amount);
                bonus += smallAmount * smallRate;

                var oddRate = PKRateService.GetRate(pkRate, BetNumConst.Odd);
                decimal oddAmount = db.Bet.Where(b => b.Num == BetNumConst.Odd && b.Rank == rank).Sum(b => b.Amount);
                bonus += oddAmount * oddRate;

                var evenRate = PKRateService.GetRate(pkRate, BetNumConst.Even);
                decimal evenAmount = db.Bet.Where(b => b.Num == BetNumConst.Even && b.Rank == rank).Sum(b => b.Amount);
                bonus += evenAmount * evenRate;
            }

            return bonus;
        }

        /// <summary>
        /// 重新排序
        /// 第一名向下冒泡
        /// </summary>
        /// <param name="orders">[2,5,1,3,4,7,8,6,10,9]</param>
        /// <param name="position">当前位置</param>
        /// <returns></returns>
        private List<int> ReOrder(List<int> orders, int position)
        {
            position = position % (orders.Count - 1);

            var temp = orders[position];
            orders[position] = orders[position + 1];
            orders[position + 1] = temp;

            return orders;
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
