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
}
