using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// 比赛状况, 每n秒推送一次
    /// </summary>
    public class PKModel
    {
        /// <summary>
        /// 比赛
        /// </summary>
        public PK PK { get; set; }
        
        /// <summary>
        /// 比赛已经开始的秒数: 当前时间 - 比赛开始时间 的秒数
        /// 新用户打开页面时, 计算赛车的位置
        /// </summary>
        public int PassedSeconds { get; set; }
    }
}
