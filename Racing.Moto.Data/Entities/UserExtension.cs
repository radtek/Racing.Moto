using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    [Table(nameof(UserExtension))]
    public partial class UserExtension
    {
        [Key, ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// 账户金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 如果当前用户是总代理: 总代理下代理的数量
        /// </summary>
        public int AgentCount { get; set; }

        /// <summary>
        /// 如果当前用户是代理: 代理下会员的数量
        /// </summary>
        public int MemberCount { get; set; }
        
        public virtual User User { get; set; }

        /// <summary>
        /// 代理 UserId 报表使用
        /// </summary>
        public int? AgentUserId { get; set; }

        /// <summary>
        /// 总代理 UserId 报表使用
        /// </summary>
        public int? GeneralAgentUserId { get; set; }
    }
}
