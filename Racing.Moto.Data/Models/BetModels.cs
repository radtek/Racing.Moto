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
    }
}
