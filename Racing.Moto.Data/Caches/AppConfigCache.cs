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

        /// <summary>
        /// 开盘时长, 按秒记录
        /// </summary>
        public static int Racing_Opening_Seconds
        {
            get { return GetAppConfig("Racing_Opening_Seconds") != null ? int.Parse(GetAppConfig("Racing_Opening_Seconds").Value) : 0; }
        }

        /// <summary>
        /// 封盘时长, 按秒记录
        /// </summary>
        public static int Racing_Close_Seconds
        {
            get { return GetAppConfig("Racing_Close_Seconds") != null ? int.Parse(GetAppConfig("Racing_Close_Seconds").Value) : 0; }
        }

        /// <summary>
        /// 比赛时长, 按秒记录
        /// </summary>
        public static int Racing_Game_Seconds
        {
            get { return GetAppConfig("Racing_Game_Seconds") != null ? int.Parse(GetAppConfig("Racing_Game_Seconds").Value) : 0; }
        }

        /// <summary>
        /// 开奖时长, 按秒记录
        /// </summary>
        public static int Racing_Lottery_Seconds
        {
            get { return GetAppConfig("Racing_Lottery_Seconds") != null ? int.Parse(GetAppConfig("Racing_Lottery_Seconds").Value) : 0; }
        }

        #endregion
    }
}
