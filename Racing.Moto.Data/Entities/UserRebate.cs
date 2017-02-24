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
    /// <summary>
    /// 退水
    /// </summary>
    [Table(nameof(UserRebate))]
    public partial class UserRebate
    {
        [Key]
        public int UserRebateId { get; set; }

        /// <summary>
        /// 1 - 14
        /// </summary>
        public int RebateNo { get; set; }

        /*
         A盘、B盘、C盘

            1、开通总代理，总代理的退水分3个档，分别为A盘、B盘、C盘
            2、用户可在A盘、B盘、C盘调水的额度
            3、退水的额度也可以随时进行手动调整，不能超出上级所设置的最大额度
            4、水点调节额度，只能越来越少，不能超出上级所设置的最大额度
            5、退水可能精确到0.01单位

            *超级管理员：
               他给【总代理】设置A\B\C的最大水额度，默认：A盘4个水、B盘3个水，C盘2个水
            *总代理：
               他给【代理】设置A\B\C的最大水额度，默认：A盘4个水、B盘3个水，C盘2个水
            *代理：
               他给【会员】只能设置1个盘，最早会员在购买、下注、结算的时候，按照该盘的水点进行返点
            *会员：
               只允许存在A盘、B盘、C盘其中一个盘，作为自己的退水返点            
             
        */
        /// <summary>
        /// 盘类型
        /// 1: A盘
        /// </summary>
        public decimal RebateTypeA { get; set; }

        /// <summary>
        /// 盘类型
        /// 2: B盘
        /// </summary>
        public decimal RebateTypeB { get; set; }

        /// <summary>
        /// 盘类型
        /// 3: C盘
        /// </summary>
        public decimal RebateTypeC { get; set; }


        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
