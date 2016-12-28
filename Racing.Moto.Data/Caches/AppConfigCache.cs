using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Data.Caches
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
        /// 管理员利润比率
        /// </summary>
        public static decimal Rate_Admin
        {
            get { return GetAppConfig("Rate_Admin") != null ? decimal.Parse(GetAppConfig("Rate_Admin").Value) : 0; }
        }

        /// <summary>
        /// 总代理利润比率
        /// </summary>
        public static decimal Rate_Main_Agent
        {
            get { return GetAppConfig("Rate_Main_Agent") != null ? decimal.Parse(GetAppConfig("Rate_Main_Agent").Value) : 0; }
        }

        /// <summary>
        /// 代理利润比率
        /// </summary>
        public static decimal Rate_Agent
        {
            get { return GetAppConfig("Rate_Agent") != null ? decimal.Parse(GetAppConfig("Rate_Agent").Value) : 0; }
        }

        /// <summary>
        /// 会员利润比率
        /// </summary>
        public static decimal Rate_Member
        {
            get { return GetAppConfig("Rate_Member") != null ? decimal.Parse(GetAppConfig("Rate_Member").Value) : 0; }
        }

        #endregion

        #endregion
    }
}
