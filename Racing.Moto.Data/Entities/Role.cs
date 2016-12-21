using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
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
