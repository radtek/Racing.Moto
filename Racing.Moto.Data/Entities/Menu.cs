using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    [Table(nameof(Menu))]
    public partial class Menu
    {
        [Key]
        public int MenuId { get; set; }
        
        [StringLength(300)]
        public string MenuName { get; set; }

        [StringLength(300)]
        public string MenuUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool Visible { get; set; }


        [ForeignKey(nameof(ParentMenu))]
        public int? ParentMenuId { get; set; }
        public Menu ParentMenu { get; set; }

        [InverseProperty(nameof(Menu))]
        public virtual ICollection<MenuRole> MenuRoles { get; set; }
    }
}
