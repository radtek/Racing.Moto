using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Entities
{
    [Table(nameof(User))]
    public partial class User
    {
        [Key]
        public int UserId { get; set; }

        [StringLength(300)]
        public string UserName { get; set; }

        [StringLength(300)]
        public string Email { get; set; }

        [StringLength(300)]
        public string Password { get; set; }

        public bool Enabled { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public bool IsLocked { get; set; }
        public Nullable<System.DateTime> LockedDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public System.DateTime FailedPasswordAttemptWindowStart { get; set; }
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 账户金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(300)]
        public string Avatar { get; set; }

        /// <summary>
        /// 是否在线, 不映射到数据库, 只用作前台显示
        /// </summary>
        [NotMapped]
        public bool IsOnline { get; set; }
        
        [ForeignKey(nameof(ParentUser))]
        public int? ParentUserId { get; set; }
        public User ParentUser { get; set; }
    }
}
