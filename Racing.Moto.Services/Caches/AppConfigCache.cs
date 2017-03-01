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

        #region 奖金分配比率, 退水

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

        /*
        A盘、B盘、C盘

           1、开通总代理，总代理的退水分3个档，分别为A盘、B盘、C盘
           2、用户可在A盘、B盘、C盘调水的额度
           3、退水的额度也可以随时进行手动调整，不能超出上级所设置的最大额度
           4、水点调节额度，只能越来越少，不能超出上级所设置的最大额度
           5、退水可能精确到0.01单位

           *超级管理员：
              他给【总代理】设置A\B\C的最大水额度，默认：A盘4个水、B盘3个水，C盘2个水
           *总代理：
              他给【代理】设置A\B\C的最大水额度，默认：A盘4个水、B盘3个水，C盘2个水
           *代理：
              他给【会员】只能设置1个盘，最早会员在购买、下注、结算的时候，按照该盘的水点进行返点
           *会员：
              只允许存在A盘、B盘、C盘其中一个盘，作为自己的退水返点            

       */
        /// <summary>
        /// A盘
        /// </summary>
        public static decimal Rate_Rebate_A
        {
            get { return GetAppConfig("Rate_Rebate_A") != null ? decimal.Parse(GetAppConfig("Rate_Rebate_A").Value) : 0.04M; }
        }

        /// <summary>
        /// B盘
        /// </summary>
        public static decimal Rate_Rebate_B
        {
            get { return GetAppConfig("Rate_Rebate_B") != null ? decimal.Parse(GetAppConfig("Rate_Rebate_B").Value) : 0.03M; }
        }

        /// <summary>
        /// C盘
        /// </summary>
        public static decimal Rate_Rebate_C
        {
            get { return GetAppConfig("Rate_Rebate_C") != null ? decimal.Parse(GetAppConfig("Rate_Rebate_C").Value) : 0.02M; }
        }

        /// <summary>
        /// 单注限额
        /// </summary>
        public static decimal Rate_Rebate_MaxBetAmount
        {
            get { return GetAppConfig("Rate_Rebate_MaxBetAmount") != null ? decimal.Parse(GetAppConfig("Rate_Rebate_MaxBetAmount").Value) : 100000; }
        }

        /// <summary>
        /// 单期限额
        /// </summary>
        public static decimal Rate_Rebate_MaxPKAmount
        {
            get { return GetAppConfig("Rate_Rebate_MaxPKAmount") != null ? decimal.Parse(GetAppConfig("Rate_Rebate_MaxPKAmount").Value) : 50000; }
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
