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
    /// 用户下注表: 绑定时使用
    /// </summary>
    [Table(nameof(PKUser))]
    public partial class PKUser
    {
        [Key]
        public int PKUserId { get; set; }

        /// <summary>
        /// 用户下注信息Json: 10行 14列
        /// </summary>
        [StringLength(2000)]
        public string RawJson { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        /// <summary>
        /// PK
        /// </summary>
        [ForeignKey(nameof(PK))]
        public int PKId { get; set; }
        public virtual PK PK { get; set; }
    }
}
