using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Game.Data.Entities
{
    /// <summary>
    /// PK: 每10分钟一场PK
    /// 每一场开奖分为四个时期，共持续10分钟（时间均为后台可配置）										
	/// ①开盘时期，持续8m，可以进行下注操作									
	/// ②封盘时期，持续50s，玩家不可以进行下注操作，管理员可以在后台进行注单操作。									
	/// ③比赛时期，持续1m，玩家不可以进行下注操作，管理员也不可以在后台进行注单操作。									
	/// ④开奖时期，持续10s，进入当前时期后，当前开奖期数和开奖结果同时刷新，结束后进入新一起的开盘操作
    /// </summary>
    [Table(nameof(PK))]
    public partial class PK
    {
        [Key]
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
        /// 名次: 临时存数据
        /// 05,08,01,07,10,09,03,02,05,04
        /// </summary>
        [NotMapped]
        [StringLength(100)]
        public string Ranks { get; set; }

        /// <summary>
        /// 初中高级场
        /// </summary>
        public virtual ICollection<PKRoom> PKRooms { get; set; }
    }
}
