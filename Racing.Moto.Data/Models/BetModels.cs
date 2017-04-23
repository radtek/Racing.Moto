using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// 下注金额
    /// 按名次+车号(1-14)求和
    /// </summary>
    public class BetAmountModel
    {
        /// <summary>
        /// 名次
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 车号+大小单双
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 倍率
        /// </summary>
        public decimal PKRate { get; set; }

        /// <summary>
        /// 金额*赔率
        /// </summary>
        public decimal RateAmount { get; set; }
    }
    /// <summary>
    /// 奖池百分比（奖池占有率）
    /// 名次+车号(1-10)的 奖池百分比
    /// </summary>
    public class BetRateModel
    {
        /// <summary>
        /// 名次
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 车号: 10个车号
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 奖池百分比: 保留四位小数
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }
    }

    /// <summary>
    /// 名次中奖百分比
    /// </summary>
    public class RankRateModel
    {
        /// <summary>
        /// 名次
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 百分比
        /// </summary>
        public decimal Rate { get; set; }
    }

    public class BetStatisticModel
    {
        public PKModel PKModel { get; set; }

        public List<PKRate> PKRates { get; set; }

        public List<BetAmountModel> BetAmountsAll { get; set; }

        public List<BetAmountModel> BetAmounts1 { get; set; }
        public List<BetAmountModel> BetAmounts2 { get; set; }
        public List<BetAmountModel> BetAmounts3 { get; set; }
        public List<BetAmountModel> BetAmounts4 { get; set; }
        public List<BetAmountModel> BetAmounts5 { get; set; }
        public List<BetAmountModel> BetAmounts6 { get; set; }
        public List<BetAmountModel> BetAmounts7 { get; set; }
        public List<BetAmountModel> BetAmounts8 { get; set; }
        public List<BetAmountModel> BetAmounts9 { get; set; }
        public List<BetAmountModel> BetAmounts10 { get; set; }

        //總投注額
        public decimal TotalAmount { get; set; }
        //public decimal BetAmount1 { get; set; }
        //public decimal BetAmount2 { get; set; }
        //public decimal BetAmount3 { get; set; }
        //public decimal BetAmount4 { get; set; }
        //public decimal BetAmount5 { get; set; }
        //public decimal BetAmount6 { get; set; }
        //public decimal BetAmount7 { get; set; }
        //public decimal BetAmount8 { get; set; }
        //public decimal BetAmount9 { get; set; }
        //public decimal BetAmount10 { get; set; }

        //最高盈利
        public decimal MaxProfit { get; set; }
        //public decimal MaxProfit1 { get; set; }
        //public decimal MaxProfit2 { get; set; }
        //public decimal MaxProfit3 { get; set; }
        //public decimal MaxProfit4 { get; set; }
        //public decimal MaxProfit5 { get; set; }
        //public decimal MaxProfit6 { get; set; }
        //public decimal MaxProfit7 { get; set; }
        //public decimal MaxProfit8 { get; set; }
        //public decimal MaxProfit9 { get; set; }
        //public decimal MaxProfit10 { get; set; }

        //最高虧損
        public decimal MaxLoss { get; set; }
        //public decimal MaxLoss1 { get; set; }
        //public decimal MaxLoss2 { get; set; }
        //public decimal MaxLoss3 { get; set; }
        //public decimal MaxLoss4 { get; set; }
        //public decimal MaxLoss5 { get; set; }
        //public decimal MaxLoss6 { get; set; }
        //public decimal MaxLoss7 { get; set; }
        //public decimal MaxLoss8 { get; set; }
        //public decimal MaxLoss9 { get; set; }
        //public decimal MaxLoss10 { get; set; }

        public List<RankAmountModel> RankAmounts { get; set; }
    }

    public class RankAmountModel
    {
        public int Rank { get; set; }

        public decimal Amount { get; set; }
    }

    public class BetAmountRankModel
    {
        public int Rank { get; set; }

        public List<BetAmountModel> BetAmounts { get; set; }
    }
}
