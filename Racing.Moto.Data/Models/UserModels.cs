using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// 用户检索
    /// </summary>
    public class UserSearchModel
    {
        /// <summary>
        /// 父UserId
        /// </summary>
        public int? FatherUserId { get; set; }

        /// <summary>
        /// 祖父UserId
        /// </summary>
        public int? GrandFatherUserId { get; set; }

        /// <summary>
        /// 用户类型 = 角色RoleId
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 冻结
        /// </summary>
        public bool? IsLocked { get; set; }

        /// <summary>
        /// 页号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 信用资料
    /// </summary>
    public class UserCreditModel
    {
        /// <summary>
        /// 今日消费
        /// </summary>
        public decimal TodayBetAmount { get; set; }

        /// <summary>
        /// 未开奖消费
        /// </summary>
        public decimal NotBonusAmount { get; set; }

        /// <summary>
        /// 今日返点
        /// </summary>
        public decimal TodayRebateAmount { get; set; }

        /// <summary>
        /// 今日利润
        /// </summary>
        public decimal TodayProfitAmount { get; set; }

        /// <summary>
        /// 今日盈亏
        /// </summary>
        public decimal TodayProfitAndLossAmount { get; set; }

        //public User User { get; set; }

        /// <summary>
        /// 退水, 限额
        /// </summary>
        public List<UserRebate> UserRebates { get; set; }
    }

    public class UserBonusReportStatistics
    {
        /// <summary>
        /// 注单数量
        /// </summary>
        public int BetCount { get; set; }

        /// <summary>
        /// 下注金额
        /// </summary>
        public decimal BetAmount { get; set; }

        /// <summary>
        /// 奖金+退水金额
        /// </summary>
        public decimal BonusAmount { get; set; }
    }
}
