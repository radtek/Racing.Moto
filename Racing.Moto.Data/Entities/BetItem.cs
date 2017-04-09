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
    /// 下注表条目: 每次PK同一个人可以多次下注, Bet.Amount = Sum(BetItem.Amount)
    /// </summary>
    [Table(nameof(BetItem))]
    public partial class BetItem
    {
        [Key]
        public int BetItemId { get; set; }

        /// <summary>
        /// 名次: 行(1-10名)
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 押注的车号/大小/单双: 列(1-14, 1-10为第几号, 11-12为大小,13-14为单双)
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 奖金金额
        /// </summary>
        [NotMapped]
        public decimal BonusAmount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Bet
        /// </summary>
        [ForeignKey(nameof(Bet))]
        public int BetId { get; set; }
        public virtual Bet Bet { get; set; }
    }
}
