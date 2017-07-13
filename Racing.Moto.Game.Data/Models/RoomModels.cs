using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Models
{
    public class RoomModel
    {
        /// <summary>
        /// 级别: 1-初级, 2-中级, 3-高级
        /// </summary>
        public int RoomLevel { get; set; }

        public List<RoomDeskModel> RoomDesks { get; set; }
    }

    public class RoomDeskModel
    {
        /// <summary>
        /// 级别: 1-初级, 2-中级, 3-高级
        /// </summary>
        public int RoomLevel { get; set; }

        /// <summary>
        /// 房间桌子ID 1-8
        /// </summary>
        public int RoomDeskId { get; set; }

        public List<RoomUserModel> Users { get; set; }
    }

    public class RoomUserModel
    {
        /// <summary>
        /// 级别: 1-初级, 2-中级, 3-高级
        /// </summary>
        public int RoomLevel { get; set; }

        /// <summary>
        /// 桌号
        /// </summary>
        public int DeskNo { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// 用户的车号
        /// </summary>
        public int Num { get; set; }
    }
}
