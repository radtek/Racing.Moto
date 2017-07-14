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
    public class PKModel
    {
        /// <summary>
        /// 盘
        /// </summary>
        public PK PK { get; set; }

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


        public List<PKRoom> PKRooms { get; set; }
    }
}
