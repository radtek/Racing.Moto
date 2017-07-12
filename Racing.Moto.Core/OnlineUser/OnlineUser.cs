/* 
 * OnlineUser.cs @Microsoft Visual Studio 2008 <.NET Framework 2.0 (or Higher)>
 * AfritXia
 * 2008-03-06
 * 
 * Copyright(c) http://www.AfritXia.NET/
 * 
 */

using System;
using System.Collections.Generic;

namespace App.Core.OnlineStat
{
    /// <summary>
    /// 在线用户类
    /// </summary>
    public class OnlineUser
    {
        // 用户 ID
        private int m_uniqueID;
        // 父级用户Id
        private int? m_p_userId;
        // 祖父级用户Id
        private int? m_g_userId;
        // 名称
        private string m_userName;
        // 身份
        private int m_userDegree;
        // 登录时间
        private DateTime? m_loginTime;
        // 最后活动时间
        private DateTime m_activeTime;
        // 最后请求地址
        private string m_requestURL;
        // SessionID
        private string m_sessionID;
        // IP 地址
        private string m_clientIP;

        //房间级别 初中高级
        private int m_roomLevel;

        //房间桌子号
        private int m_deskNo;

        // 车号
        private int m_num;
        // 头像
        public string m_avatar;

        #region 类构造器
        /// <summary>
        /// 类默认构造器
        /// </summary>
        public OnlineUser()
        {
        }

        /// <summary>
        /// 类参数构造器
        /// </summary>
        /// <param name="uniqueID">用户 ID</param>
        /// <param name="userName">用户名称</param>
        public OnlineUser(int uniqueID, string userName)
        {
            this.UniqueID = uniqueID;
            this.UserName = userName;
        }
        #endregion

        /// <summary>
        /// guid
        /// 如: 7724621e-3556-4427-b933-c8c93aa81182
        /// 用于限制用户只能在一处登录
        /// </summary>
        public string AuthenticationId { get; set; }

        /// <summary>
        /// 设置或获取用户 ID
        /// </summary>
        public int UniqueID
        {
            set
            {
                this.m_uniqueID = value;
            }

            get
            {
                return this.m_uniqueID;
            }
        }

        /// <summary>
        /// 父级用户ID
        /// </summary>
        public int? ParentUserId
        {
            set
            {
                this.m_p_userId = value;
            }

            get
            {
                return this.m_p_userId;
            }
        }

        /// <summary>
        /// 祖父级用户名
        /// </summary>
        public int? GrandUserId
        {
            set
            {
                this.m_g_userId = value;
            }

            get
            {
                return this.m_g_userId;
            }
        }

        /// <summary>
        /// 设置或获取用户昵称
        /// </summary>
        public string UserName
        {
            set
            {
                this.m_userName = value;
            }

            get
            {
                return this.m_userName;
            }
        }

        /// <summary>
        /// 设置或获取用户身份
        /// </summary>
        public int UserDegree
        {
            set
            {
                this.m_userDegree = value;
            }

            get
            {
                return this.m_userDegree;
            }
        }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? LoginTime
        {
            set
            {
                this.m_loginTime = value;
            }

            get
            {
                return this.m_loginTime;
            }
        }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime ActiveTime
        {
            set
            {
                this.m_activeTime = value;
            }

            get
            {
                return this.m_activeTime;
            }
        }

        /// <summary>
        /// 最后请求地址
        /// </summary>
        public string RequestURL
        {
            set
            {
                this.m_requestURL = value;
            }

            get
            {
                return this.m_requestURL;
            }
        }

        /// <summary>
        /// 设置或获取 SessionID
        /// </summary>
        public string SessionID
        {
            set
            {
                this.m_sessionID = value;
            }

            get
            {
                return this.m_sessionID;
            }
        }

        /// <summary>
        /// 设置或获取 IP 地址
        /// </summary>
        public string ClientIP
        {
            set
            {
                this.m_clientIP = value;
            }

            get
            {
                return this.m_clientIP;
            }
        }

        /// <summary>
        /// 设置或获取 房间级别 1-初级, 2-中级, 3-高级
        /// </summary>
        public int RoomLevel
        {
            set
            {
                this.m_roomLevel = value;
            }

            get
            {
                return this.m_roomLevel;
            }
        }

        /// <summary>
        /// 设置或获取 房间桌子号
        /// </summary>
        public int DeskNo
        {
            set
            {
                this.m_deskNo = value;
            }

            get
            {
                return this.m_deskNo;
            }
        }

        /// <summary>
        /// 车号
        /// </summary>
        public int Num
        {
            set
            {
                this.m_num = value;
            }

            get
            {
                return this.m_num;
            }
        }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar
        {
            set
            {
                this.m_avatar = value;
            }

            get
            {
                return this.m_avatar;
            }
        }
    }
}