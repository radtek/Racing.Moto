using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    [Table(nameof(UserExtend))]
    public partial class UserExtend
    {
        [Key, ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// 账户金额
        /// </summary>
        public decimal Amount { get; set; }

        public virtual User User { get; set; }
    }
}
