using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// 检索
    /// </summary>
    public class SearchModel
    {
        /// <summary>
        /// 页号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 期数: PKID
        /// </summary>
        public int Key { get; set; }
    }

    /// <summary>
    /// 用户报表检索
    /// </summary>
    public class UserReportSearchModel
    {
        /// <summary>
        /// 页号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettlementDone { get; set; }
    }

    /// <summary>
    /// 报表检索
    /// </summary>
    public class ReportSearchModel
    {
        /// <summary>
        /// 页号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// 用户Id
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// 父级用户Id
        /// </summary>
        public int? ParentUserId { get; set; }

        /// <summary>
        /// PKId
        /// </summary>
        public int? PKId { get; set; }

        /// <summary>
        /// 用户类型
        /// RoleId	RoleName
        /// 1	管理员
        /// 2	总代理
        /// 3	代理
        /// 4	会员
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 下註類型: 1-10名, 11,12 大小, 13,14 单双
        /// </summary>
        public int? BetType { get; set; }

        /// <summary>
        /// 1:按期數, 2:按日期
        /// </summary>
        public int SearchType { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// 截至日期
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// 1:交收報錶, 2:分類報錶
        /// </summary>
        public int ReportType { get; set; }

        /// <summary>
        /// 1:已 結 算, 2:未 結 算
        /// </summary>
        public int SettlementType { get; set; }
    }
}
