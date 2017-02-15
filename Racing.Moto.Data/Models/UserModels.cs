using Racing.Moto.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Models
{
    /// <summary>
    /// 用户检索
    /// </summary>
    public class UserSearchModel
    {
        /// <summary>
        /// 用户类型 = 角色
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 页号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 用户类型
    /// RoleId	RoleName
    /// 1	管理员
    /// 2	总代理
    /// 3	代理
    /// 4	会员
    /// </summary>
    public class UserType
    {
        public const int All = 0;
        public const int Admin = 1;
        public const int GeneralAgent = 2;
        public const int Agent = 3;
        public const int Member = 4;
        //public const int Vistor = 5;
    }
}
