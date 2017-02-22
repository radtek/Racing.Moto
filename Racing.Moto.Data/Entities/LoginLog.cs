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
    /// 登录日志
    /// </summary>
    [Table(nameof(LoginLog))]
    public partial class LoginLog
    {
        [Key]
        public int LoginLogId { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        [StringLength(20)]
        public string IP { get; set; }

        /// <summary>
        /// 归属地
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
