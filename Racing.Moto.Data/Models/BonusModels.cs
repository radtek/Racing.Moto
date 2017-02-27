using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// PK结束后奖金检索
    /// </summary>
    public class BonusSearchModel
    {
        public int PKId { get; set; }
        public int UserId { get; set; }
    }

    /// <summary>
    /// 奖金检索结果
    /// </summary>
    public class BonusResultModel
    {
        public int PKId { get; set; }
        public string UserName { get; set; }
        public int Count { get; set; }//笔数
        public decimal TotalAmount { get; set; }//金额
        public decimal MemberAmount { get; set; }//会员输赢
        public decimal ReceiveAmount { get; set; }//应收下线
        public decimal RebateAmount { get; set; }//赚取水钱
        public decimal PayAmount { get; set; }//应付上级
    }
}
