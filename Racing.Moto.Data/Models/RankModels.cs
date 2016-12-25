using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// 押注车奖金
    /// 每辆车押注的奖金
    /// </summary>
    public class MotoAmountModel
    {
        /// <summary>
        /// 车号
        /// </summary>
        public int MotoNo { get; set; }

        /// <summary>
        /// 奖金
        /// </summary>
        public decimal Amount { get; set; }
    }
}
