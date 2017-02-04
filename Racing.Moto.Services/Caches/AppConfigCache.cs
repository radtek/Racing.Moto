using Racing.Moto.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services.Caches
{
    public class AppConfigCache
    {
        private static List<AppConfig> appConfigs = null;

        public static List<AppConfig> GetAllAppConfigs()
        {
            if (appConfigs == null)
            {
                appConfigs = new AppConfigService().GetAll();
            }

            return appConfigs;
        }

        public static AppConfig GetAppConfig(string name)
        {
            var appConfig = GetAllAppConfigs().Where(a => a.Name == name).FirstOrDefault();

            return appConfig;
        }

        public static void Update()
        {
            appConfigs = new AppConfigService().GetAll();
        }

        #region Static

        #region 比赛时段设置

        /// <summary>
        /// 开盘时长, 按秒记录, 默认8m
        /// </summary>
        public static int Racing_Opening_Seconds
        {
            get { return GetAppConfig("Racing_Opening_Seconds") != null ? int.Parse(GetAppConfig("Racing_Opening_Seconds").Value) : 60 * 8; }
        }

        /// <summary>
        /// 封盘时长, 按秒记录, 默认1m
        /// </summary>
        public static int Racing_Close_Seconds
        {
            get { return GetAppConfig("Racing_Close_Seconds") != null ? int.Parse(GetAppConfig("Racing_Close_Seconds").Value) : 60; }
        }

        /// <summary>
        /// 比赛时长, 按秒记录, 50s
        /// </summary>
        public static int Racing_Game_Seconds
        {
            get { return GetAppConfig("Racing_Game_Seconds") != null ? int.Parse(GetAppConfig("Racing_Game_Seconds").Value) : 50; }
        }

        /// <summary>
        /// 开奖时长, 按秒记录, 10s
        /// </summary>
        public static int Racing_Lottery_Seconds
        {
            get { return GetAppConfig("Racing_Lottery_Seconds") != null ? int.Parse(GetAppConfig("Racing_Lottery_Seconds").Value) : 10; }
        }

        /// <summary>
        /// 比赛总时长
        /// </summary>
        public static int Racing_Total_Seconds
        {
            get { return Racing_Opening_Seconds + Racing_Close_Seconds + Racing_Game_Seconds + Racing_Lottery_Seconds; }
        }

        #endregion

        #region 奖金分配比率

        /// <summary>
        /// 管理员利润比率,吃二出八
        /// </summary>
        public static decimal Rate_Admin
        {
            get { return GetAppConfig("Rate_Admin") != null ? decimal.Parse(GetAppConfig("Rate_Admin").Value) : 0.2M; }
        }

        /// <summary>
        /// 退水，总代理+代理+会员=4%
        /// </summary>
        public static decimal Rate_Return
        {
            get { return GetAppConfig("Rate_Return") != null ? decimal.Parse(GetAppConfig("Rate_Return").Value) : 0.04M; }
        }

        #endregion


        #region 站内消息

        /// <summary>
        /// 公告
        /// </summary>
        public static int News_Announcement
        {
            get { return GetAppConfig("News_Announcement") != null ? int.Parse(GetAppConfig("News_Announcement").Value) : 1; }
        }

        /// <summary>
        /// 跑马灯
        /// </summary>
        public static int News_Marquee
        {
            get { return GetAppConfig("News_Marquee") != null ? int.Parse(GetAppConfig("News_Marquee").Value) : 2; }
        }

        #endregion

        #endregion
    }
}
