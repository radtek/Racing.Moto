using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Entities
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
        /// 号:1-10号
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 奖金
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 奖金类型
        /// 1: 奖金, 2: 退水
        /// </summary>
        //public BonusType BonusType { get; set; }

        ///// <summary>
        ///// 是否已结算
        ///// </summary>
        //public bool IsSettlementDone { get; set; }

        ///// <summary>
        ///// 下级给上级退水时记录下级的UserId
        ///// </summary>
        //public int? ChildUserId { get; set; }

        ///// <summary>
        ///// 下注
        ///// </summary>
        //[ForeignKey(nameof(Bet))]
        //public int BetId { get; set; }
        //public virtual Bet Bet { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// 桌
        /// </summary>
        [ForeignKey(nameof(PKRoomDesk))]
        public int PKRoomDeskId { get; set; }
        public PKRoomDesk PKRoomDesk { get; set; }

        /// <summary>
        /// 初中高级
        /// </summary>
        public int PKRoomId { get; set; }

        /// <summary>
        /// PK
        /// </summary>
        public int PKId { get; set; }
    }
}
