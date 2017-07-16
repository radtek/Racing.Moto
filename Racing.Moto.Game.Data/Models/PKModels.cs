using Racing.Moto.Game.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Models
{
    /// <summary>
    /// 比赛状况, 每n秒推送一次
    /// </summary>
    public class PKInfoModel
    {
        /// <summary>
        /// 盘
        /// </summary>
        public PKModel PK { get; set; }

        /// <summary>
        /// 当前时间: 服务器时间可能与用户机器时间不同, 返回服务器当前时间 用于计算倒计时
        /// </summary>
        public DateTime Now { get; set; }

        /// <summary>
        /// 开盘的秒数: 当前时间 - 开盘开始时间 的秒数
        /// </summary>
        public int PassedSeconds { get; set; }

        /// <summary>
        /// 距离封盘的秒数, 负:已封盘, 正:距离封盘的秒数
        /// </summary>
        public int OpeningRemainSeconds { get; set; }

        /// <summary>
        /// 距离开奖的秒数
        /// </summary>
        public int ToLotterySeconds { get; set; }

        /// <summary>
        /// 封盘开始时间
        /// </summary>
        public DateTime CloseBeginTime { get; set; }

        /// <summary>
        /// 游戏开始时间
        /// </summary>
        public DateTime GameBeginTime { get; set; }

        /// <summary>
        /// 盘剩余的秒数: 当前时间 总开盘秒数 - PassedSeconds 的秒数
        /// </summary>
        public int RemainSeconds { get; set; }

        /// <summary>
        /// 比赛已经开始的秒数: 当前时间 - 比赛开始时间 的秒数
        /// 新用户打开页面时, 计算赛车的位置
        /// </summary>
        public int GamePassedSeconds { get; set; }

        /// <summary>
        /// 比赛剩余的秒数: 当前时间 GameSeconds - PassedSeconds 的秒数
        /// 新用户打开页面时, 计算赛车的位置
        /// </summary>
        public int GameRemainSeconds { get; set; }

        /// <summary>
        /// 距离比赛开始的秒数, 负:未开始, 正:已开始
        /// </summary>
        public int GamingSeconds { get; set; }


        public List<PKRoomModel> PKRooms { get; set; }
    }

    public class PKModel
    {
        public int PKId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 开盘时长, 按秒记录
        /// </summary>
        public int OpeningSeconds { get; set; }

        /// <summary>
        /// 封盘时长, 按秒记录
        /// </summary>
        public int CloseSeconds { get; set; }

        /// <summary>
        /// 比赛时长, 按秒记录
        /// </summary>
        public int GameSeconds { get; set; }

        /// <summary>
        /// 开奖时长, 按秒记录
        /// </summary>
        public int LotterySeconds { get; set; }

        /// <summary>
        /// 是否已生成名次
        /// </summary>
        public bool IsRanked { get; set; }

        /// <summary>
        /// 奖金已生成标志
        /// </summary>
        public bool IsBonused { get; set; }

        /// <summary>
        /// 名次: 临时存数据
        /// 05,08,01,07,10,09,03,02,05,04
        /// </summary>
        public string Ranks { get; set; }

        /// <summary>
        /// 初中高级场
        /// </summary>
        public List<PKRoomModel> PKRooms { get; set; }
    }

    public class PKRoomModel
    {
        public int PKRoomId { get; set; }

        /// <summary>
        /// 房间基本: 1-初级, 2-中级, 3-高级
        /// </summary>
        public int PKRoomLevel { get; set; }

        
        public int PKId { get; set; }


        public List<PKRoomDeskModel> PKRoomDesks { get; set; }
    }

    public class PKRoomDeskModel
    {
        public int PKRoomDeskId { get; set; }

        /// <summary>
        /// 桌号: 1-8桌
        /// </summary>
        public int DeskNo { get; set; }


        /// <summary>
        /// 名次
        /// 05,08,01,07,10,09,03,02,05,04
        /// </summary>
        public string Ranks { get; set; }
        
        public int PKRoomId { get; set; }
    }

}
