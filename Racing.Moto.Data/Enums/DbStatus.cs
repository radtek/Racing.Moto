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
}
