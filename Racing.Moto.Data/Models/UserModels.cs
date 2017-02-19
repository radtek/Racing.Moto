﻿using Racing.Moto.Data.Enums;
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
        /// 父UserId
        /// </summary>
        public int? FatherUserId { get; set; }

        /// <summary>
        /// 祖父UserId
        /// </summary>
        public int? GrandFatherUserId { get; set; }

        /// <summary>
        /// 用户类型 = 角色RoleId
        /// </summary>
        public int UserType { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 冻结
        /// </summary>
        public bool? IsLocked { get; set; }

        /// <summary>
        /// 页号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
    }
}
