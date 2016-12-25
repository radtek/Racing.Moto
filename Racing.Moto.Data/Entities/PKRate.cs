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
    /// 每一期的倍率: 一共10条数据, 10个名次, 每个名次对应 1-10号 + 大小单双, 共14列
    /// 由于倍率表(Rate), 可以修改, 所以需要记录下当前期的倍率
    /// </summary>
    [Table(nameof(PKRate))]
    public partial class PKRate
    {
        [Key]
        public int PKRateId { get; set; }

        /// <summary>
        /// 名次
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 1号
        /// </summary>
        public decimal Number1 { get; set; }

        /// <summary>
        /// 2号
        /// </summary>
        public decimal Number2 { get; set; }

        /// <summary>
        /// 3号
        /// </summary>
        public decimal Number3 { get; set; }

        /// <summary>
        /// 4号
        /// </summary>
        public decimal Number4 { get; set; }

        /// <summary>
        /// 5号
        /// </summary>
        public decimal Number5 { get; set; }

        /// <summary>
        /// 6号
        /// </summary>
        public decimal Number6 { get; set; }

        /// <summary>
        /// 7号
        /// </summary>
        public decimal Number7 { get; set; }

        /// <summary>
        /// 8号
        /// </summary>
        public decimal Number8 { get; set; }

        /// <summary>
        /// 9号
        /// </summary>
        public decimal Number9 { get; set; }

        /// <summary>
        /// 10号
        /// </summary>
        public decimal Number10 { get; set; }

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

        [ForeignKey(nameof(PK))]
        public int PKId { get; set; }
        public PK PK { get; set; }
    }
}
