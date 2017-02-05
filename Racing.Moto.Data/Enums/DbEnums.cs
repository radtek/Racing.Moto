using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Enums
{
    /// <summary>
    /// Post 状态:0-待审, 1-通过, 2-驳回
    /// </summary>
    public enum PostStatus
    {
        Init = 0,
        Pass = 1,
        Reject = 2
    }

    /// <summary>
    /// 赔率类型
    /// 0:竞技场, 1: 娱乐场a, 2: 娱乐场b, 3: 娱乐场c
    /// </summary>
    public enum RateType : byte
    {
        Arena = 0,// 竞技场
        Casino1 = 1,// 娱乐场a
        Casino2 = 2,// 娱乐场b
        Casino3 = 3,// 娱乐场c
    }
}
