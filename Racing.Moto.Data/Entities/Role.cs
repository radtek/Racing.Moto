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
    /// 角色
    /// 1. 管理员(admin), 可添加总代理
    /// 2. 总代理(main agent), 可添加代理
    /// 3. 代理(agent), 可添加会员
    /// 4. 会员(member)
    /// </summary>
    [Table(nameof(Role))]
    public partial class Role
    {
        [Key]
        public int RoleId { get; set; }

        [StringLength(300)]
        public string RoleName { get; set; }
        
        [InverseProperty(nameof(Role))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
