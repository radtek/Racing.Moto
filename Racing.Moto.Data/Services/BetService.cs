using Racing.Moto.Data.Constants;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Services
{
    public class BetService : BaseServcice
    {
        /// <summary>
        /// 计算名次车号
        /// 既第一名是几号车, 第二名是几号车...
        /// </summary>
        /// <param name="pkId">期号</param>
        public List<int> CalculateRanks(int pkId)
        {
            // 比赛结果:车号顺序
            var motoOrders = new List<int>();

            // 期
            var pk = db.PK.Include(nameof(PK.PKRates)).First();

            //押注的金额
            var betAmount = db.Bet.Where(b => b.PKUser.PKId == pkId).Sum(b => b.Amount);

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
            var position = -1;
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
                    Amount = db.Bet.Where(b => b.PKUser.PKId == pkId && b.Num == i).Sum(b => b.Amount)
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
                var pkRate = pk.PKRates.Where(r => r.Rank == rank).First(); //本期倍率

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
        /// <param name="orders">[2,5,1,3,4,7,8,6,,10,9]</param>
        /// <param name="position">当前位置</param>
        /// <returns></returns>
        private List<int> ReOrder(List<int> orders, int position)
        {
            var temp = orders[position];
            orders[position] = orders[position + 1];
            orders[position + 1] = temp;

            return orders;
        }
    }
}
