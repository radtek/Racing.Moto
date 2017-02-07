﻿/* 
 * OnlineUserDeleteCmd.cs @Microsoft Visual Studio 2008 <.NET Framework 2.0 (or Higher)>
 * AfritXia
 * 2008-03-06
 * 
 * Copyright(c) http://www.AfritXia.NET/
 * 
 */

using System;

namespace App.Core.OnlineStat
{
    /// <summary>
    /// 删除命令
    /// </summary>
    internal class OnlineUserDeleteCmd : OnlineUserCmdBase
    {
        #region 类构造器
        /// <summary>
        /// 类默认构造器
        /// </summary>
        public OnlineUserDeleteCmd()
            : base()
        {
        }

        /// <summary>
        /// 类参数构造器
        /// </summary>
        /// <param name="db">在线用户数据库</param>
        /// <param name="currUser">当前被删除的用户</param>
        public OnlineUserDeleteCmd(OnlineUserDB db, OnlineUser currUser)
            : base(db, currUser)
        {
        }
        #endregion

        /// <summary>
        /// 执行命令
        /// </summary>
        public override void Execute()
        {
            this.OnlineUserDB.Delete(this.CurrentUser);
        }
    }
}