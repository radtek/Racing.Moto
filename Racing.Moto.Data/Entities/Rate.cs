using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Entities
{
    /// <summary>
    /// 赔率: 每种[赔率类型]10条数据, 10个名次, 每个名次对应 1-10号 + 大小单双, 共14列
    /// </summary>
    [Table(nameof(Rate))]
    public partial class Rate
    {
        [Key]
        public int RateId { get; set; }

        /// <summary>
        /// 赔率类型
        /// 0:竞技场, 1: 娱乐场a, 2: 娱乐场b, 3: 娱乐场c
        /// </summary>
        public RateType RateType { get; set; }

        /// <summary>
        /// 名次
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 1号
        /// </summary>
        public decimal Rate1 { get; set; }

        /// <summary>
        /// 2号
        /// </summary>
        public decimal Rate2 { get; set; }

        /// <summary>
        /// 3号
        /// </summary>
        public decimal Rate3 { get; set; }

        /// <summary>
        /// 4号
        /// </summary>
        public decimal Rate4 { get; set; }

        /// <summary>
        /// 5号
        /// </summary>
        public decimal Rate5 { get; set; }

        /// <summary>
        /// 6号
        /// </summary>
        public decimal Rate6 { get; set; }

        /// <summary>
        /// 7号
        /// </summary>
        public decimal Rate7 { get; set; }

        /// <summary>
        /// 8号
        /// </summary>
        public decimal Rate8 { get; set; }

        /// <summary>
        /// 9号
        /// </summary>
        public decimal Rate9 { get; set; }

        /// <summary>
        /// 10号
        /// </summary>
        public decimal Rate10 { get; set; }

        /// <summary>
        /// 大
        /// </summary>
        public decimal Big { get; set; }

        /// <summary>
        /// 小
        /// </summary>
        public decimal Small { get; set; }

        /// <summary>
        /// 单
        /// </summary>
        public decimal Odd { get; set; }

        /// <summary>
        /// 双
        /// </summary>
        public decimal Even { get; set; }
    }
}
