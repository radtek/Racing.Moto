using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
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
        
        public virtual UserExtend UserExtend { get; set; }

        [ForeignKey(nameof(ParentUser))]
        public int? ParentUserId { get; set; }
        public User ParentUser { get; set; }

        [InverseProperty(nameof(User))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
