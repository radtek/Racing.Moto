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
    /// 奖金
    /// </summary>
    [Table(nameof(PKBonus))]
    public partial class PKBonus
    {
        [Key]
        public int PKBonusId { get; set; }

        /// <summary>
        /// 名次: 1-10
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 号:1-10号 + 大小单双
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 奖金
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(PK))]
        public int PKId { get; set; }
        public PK PK { get; set; }
    }
}
