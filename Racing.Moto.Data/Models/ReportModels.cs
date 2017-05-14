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

        public int Num { get; set; }

        /// <summary>
        /// 笔数：一场比赛，下注多个盘口，每个盘口算1个笔数
        /// </summary>
        public int BetCount { get; set; }

        /// <summary>
        /// 有效金额：所有下线会员加一起的下注 金额总和
        /// </summary>
        public decimal BetAmount { get; set; }

        /// <summary>
        /// 会员输赢：会员 输+赢总和, 正+负, 求和后可正可负
        /// </summary>
        public decimal MemberWinOrLoseAmount { get; set; }

        /// <summary>
        /// 应收下线：下级会员输的金额 - 给下级退水  
        /// job生成会员赢的金额+退水, 故 应收下线 = 有效金额 - 下级赢的金额 - 下级退水
        /// </summary>
        public decimal ReceiveAmount { get; set; }

        /// <summary>
        /// 赚取水钱：根据后台管理分配的退水比率分配
        /// </summary>
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// 贡献上级= 有效金额
        /// </summary>
        public decimal ContributeHigherLevelAmount { get; set; }

        /// <summary>
        /// 应付上级：应收下线-赚取水钱
        /// </summary>
        public decimal PayHigherLevelAmount { get; set; }
    }

    public class BetReportModel
    {
        public int UserId { get; set; }
        public int BetCount { get; set; }
        public decimal Amount { get; set; }

        public int Num { get; set; }
    }

    public class BonusReportModel
    {
        public int UserId { get; set; }
        public int? AgentUserId { get; set; }
        public int? GeneralAgentUserId { get; set; }
        public int RoleId { get; set; }

        public BonusType BonusType { get; set; }
        public decimal Amount { get; set; }

        public int Num { get; set; }
    }
}
