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
        /// 倍率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 退水后结果
        /// </summary>
        [NotMapped]
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// 退水后结果
        /// </summary>
        [NotMapped]
        public decimal BonusAmount { get; set; }

        /// <summary>
        /// 是否已结算, 前台查询使用
        /// </summary>
        public bool IsSettlementDone { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// 代理 UserId 报表使用
        /// </summary>
        public int? AgentUserId { get; set; }

        /// <summary>
        /// 总代理 UserId 报表使用
        /// </summary>
        public int? GeneralAgentUserId { get; set; }

        #region Wechat
        /// <summary>
        /// 微信端订单号, 存在子表[BetItem]中
        /// </summary>
        [NotMapped]
        public long OrderNo { get; set; }
        #endregion

        /// <summary>
        /// PK
        /// </summary>
        [ForeignKey(nameof(PK))]
        public int PKId { get; set; }
        public virtual PK PK { get; set; }

        /// <summary>
        /// 下注条目
        /// </summary>
        [InverseProperty(nameof(Bet))]
        public virtual ICollection<BetItem> BetItems { get; set; }
    }
}
