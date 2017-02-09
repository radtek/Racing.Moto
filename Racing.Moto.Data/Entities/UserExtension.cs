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
        /// 退水比例
        /// </summary>
        public decimal Rebate { get; set; }

        public virtual User User { get; set; }
    }
}
