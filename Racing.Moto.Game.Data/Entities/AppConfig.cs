using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Entities
{
    [Table(nameof(AppConfig))]
    public partial class AppConfig
    {
        [Key]
        public int AppConfigId { get; set; }

        [StringLength(300)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Value { get; set; }
    }
}
