using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    [Table(nameof(MenuRole))]
    public partial class MenuRole
    {
        [Key]
        public int MenuRoleId { get; set; }

        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        [ForeignKey(nameof(Menu))]
        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }
    }
}
