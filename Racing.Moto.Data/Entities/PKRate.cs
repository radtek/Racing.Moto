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
    /// 每一期的倍率: 每次PK 10 * 14 条数据, 10个名次, 每个名次对应 1-10号 + 大小单双
    /// 由于倍率表(Rate), 可以修改, 所以需要记录下当前期的倍率
    /// </summary>
    [Table(nameof(PKRate))]
    public partial class PKRate
    {
        [Key]
        public int PKRateId { get; set; }

        /// <summary>
        /// 名次: 1-10
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 号:1-10号 + 大小单双
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 倍率
        /// </summary>
        public decimal Rate { get; set; }

        [ForeignKey(nameof(PK))]
        public int PKId { get; set; }
        public PK PK { get; set; }
    }
}
