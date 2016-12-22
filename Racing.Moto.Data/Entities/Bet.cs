using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    /// <summary>
    /// 下注表: 计算时使用
    /// </summary>
    [Table(nameof(Bet))]
    public partial class Bet
    {
        [Key]
        public int BetId { get; set; }

        /// <summary>
        /// 名次: 行
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 第几号/单/双: 列
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 倍率
        /// </summary>
        public decimal RateVal { get; set; }
    }
}
