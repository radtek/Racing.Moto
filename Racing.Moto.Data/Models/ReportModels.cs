using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// 报表
    /// </summary>
    public class ReportModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 笔数：一场比赛，下注多个盘口，每个盘口算1个笔数
        /// </summary>
        public int BetCount { get; set; }

        /// <summary>
        /// 有效金额：所有下线会员加一起的下注金额总和
        /// </summary>
        public decimal BetAmount { get; set; }

        /// <summary>
        /// 会员输赢：会员输了多少钱，所有的会员组合加一起的输赢（档期都有该数据）（会员输了就是负数，赢了是正数）
        /// </summary>
        public decimal MemberWinOrLoseAmount { get; set; }

        /// <summary>
        /// 应收下线：下级会员输的金额，应该付给上级多少钱？（会员）
        /// </summary>
        public decimal ReceiveAmount { get; set; }

        /// <summary>
        /// 赚取水钱：1%的退水，根据后台管理分配的退水比率分配
        /// </summary>
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// 贡献上级：按照有效金额写
        /// </summary>
        public decimal ContributeHigherLevelAmount { get; set; }

        /// <summary>
        /// 应付上级：应收下线+赚取水钱
        /// </summary>
        public decimal PayHigherLevelAmount { get; set; }
    }

    public class BetReportModel
    {
        public int UserId { get; set; }
        public int BetCount { get; set; }
        public decimal Amount { get; set; }
    }

    public class BonusReportModel
    {
        public int UserId { get; set; }
        public int? AgentUserId { get; set; }
        public int? GeneralAgentUserId { get; set; }

        public BonusType BonusType { get; set; }
        public decimal Amount { get; set; }
    }
}
