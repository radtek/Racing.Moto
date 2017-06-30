using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Entities
{
    [Table(nameof(PKRoom))]
    public class PKRoom
    {
        [Key]
        public int PKRoomId { get; set; }

        /// <summary>
        /// 房间基本: 1-初级, 2-中级, 3-高级
        /// </summary>
        public int PKRoomLevel { get; set; }


        [ForeignKey(nameof(PK))]
        public int PKId { get; set; }
        public PK PK { get; set; }

        /// <summary>
        /// 初中高级场
        /// </summary>
        public virtual ICollection<PKRoomDesk> PKRoomDesks { get; set; }
    }
}
