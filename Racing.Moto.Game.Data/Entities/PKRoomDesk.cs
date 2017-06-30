using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Entities
{
    [Table(nameof(PKRoomDesk))]
    public class PKRoomDesk
    {
        [Key]
        public int PKRoomDeskId { get; set; }

        /// <summary>
        /// 桌号: 1-8桌
        /// </summary>
        public int DeskNo { get; set; }


        /// <summary>
        /// 名次
        /// 05,08,01,07,10,09,03,02,05,04
        /// </summary>
        [StringLength(100)]
        public string Ranks { get; set; }


        [ForeignKey(nameof(PKRoom))]
        public int PKRoomId { get; set; }
        public PKRoom PKRoom { get; set; }
    }
}
