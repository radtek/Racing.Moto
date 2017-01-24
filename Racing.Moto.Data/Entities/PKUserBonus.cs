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
    /// 弃用, 用Bet + BetItem + Bonus代替
    /// 用户中奖信息表
    /// </summary>
    [Table(nameof(PKUserBonus))]
    public partial class PKUserBonus
    {
        [Key]
        public int PKUserBonusId { get; set; }

        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// PK
        /// </summary>
        [ForeignKey(nameof(PK))]
        public int PKId { get; set; }
        public virtual PK PK { get; set; }

        /// <summary>
        /// 用户下注表
        /// </summary>
        [ForeignKey(nameof(PKUser))]
        public int PKUserId { get; set; }
        public virtual PKUser PKUser { get; set; }        
    }
}
