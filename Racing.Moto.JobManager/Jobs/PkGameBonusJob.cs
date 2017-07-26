using App.Core.OnlineStat;
using Newtonsoft.Json;
using NLog;
using Quartz;
using Racing.Moto.Data.Models;
using Racing.Moto.Game.Data.Constants;
using Racing.Moto.Game.Data.Entities;
using Racing.Moto.Game.Data.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Racing.Moto.JobManager.Jobs
{
    /// <summary>
    /// 生成奖金
    /// </summary>
    [DisallowConcurrentExecution]
    public class PkGameBonusJob : IJob
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private int _minDummyUserId = 10000000;

        /// <summary>
        /// 生成奖金
        /// 每隔10秒执行一次
        /// </summary>
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var startInfo = "[PkGameBonusJob] Start at : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine(startInfo);
                _logger.Info(startInfo);

                Run();
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        public void Run()
        {
            var pkService = new PKService();
            var bonusService = new PKBonusService();

            var pk = pkService.GetNotBonusPKs();
            if (pk != null)
            {
                // 生成奖金
                if (!pk.IsBonused)
                {
                    // 更新 奖金生成标志, 防止多次计算
                    pkService.UpdateIsBonused(pk.PKId, true);


                    // 生成奖金
                    var bonus = GetBonus(pk);
                    if (bonus != null && bonus.Count > 0)
                    {
                        bonusService.GenerateBonus(bonus);
                    }

                    var msg = string.Format("[PkGameBonusJob] Generate Bonus - PKId : {0} - Time : {1} - Bonus Count : {2}", pk.PKId, DateTime.Now.ToString(DateFormatConst.yMd_Hms), bonus.Count);
                    _logger.Info(msg);
                }
            }
        }

        /// <summary>
        /// 从api获得当前用户, 生成Bonus
        /// </summary>
        /// <returns></returns>
        private List<PKBonus> GetBonus(PK pk)
        {
            var bonus = new List<PKBonus>();

            try
            {
                var onlieUsers = GetOnlineUsers();

                if (onlieUsers.Count == 0)
                {
                    return bonus;
                }

                /*
                    游戏结束后，结算如果是用户得了第一名，就获得50分奖励
                    初级中级高级
                    0-5000分可以在初级玩
                    5000-50000分，在中级玩
                    50000-500000分，可以在高级玩                 
                 */
                var bunusAmouts = new List<int> { 50 };//目前只有第一名有奖励, 只有第一名50分

                //房间(初中高级)
                for (var roomLevel = 1; roomLevel <= 3; roomLevel++)
                {
                    var room = pk.PKRooms.Where(r => r.PKId == pk.PKId && r.PKRoomLevel == roomLevel).First();

                    //桌子
                    for (var deskNo = 1; deskNo <= 8; deskNo++)
                    {
                        var users = onlieUsers.Where(u => u.UniqueID < _minDummyUserId && u.RoomLevel == roomLevel && u.DeskNo == deskNo).ToList();
                        if (users.Count > 0)
                        {
                            var desk = room.PKRoomDesks.Where(d => d.DeskNo == deskNo).First();
                            var rankList = desk.Ranks.Split(',');//6,7,4,10,2,8,3,1,9,5

                            int rank = 1;//名次
                            foreach (var amount in bunusAmouts)
                            {
                                var num = Convert.ToInt32(rankList[rank - 1]);//车号

                                //存在第n名的车号
                                var user = users.Where(u => u.Num == num).FirstOrDefault();
                                if (user != null)
                                {
                                    bonus.Add(new PKBonus
                                    {
                                        Rank = rank,
                                        Num = num,
                                        Amount = amount,
                                        UserId = user.UniqueID,
                                        PKRoomDeskId = desk.PKRoomDeskId,
                                        PKRoomId = room.PKRoomId,
                                        PKId = pk.PKId
                                    });
                                }

                                rank++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }

            return bonus;
        }

        private List<OnlineUser> GetOnlineUsers()
        {
            var users = new List<OnlineUser>();

            try
            {
                var racingGameWebUrl = System.Configuration.ConfigurationManager.AppSettings["RacingGameWebUrl"];
                RestClient client = new RestClient(racingGameWebUrl);
                var request = new RestRequest("/api/OnlineUser/GetOnlineUsers", Method.POST);
                //request.AddJsonBody(model);

                var response = client.Execute(request);

                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult>(response.Content);

                    if (result.Success)
                    {
                        users = JsonConvert.DeserializeObject<List<OnlineUser>>(result.Data.ToString());
                    }
                    else
                    {
                        _logger.Info(response);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }

            #region test

            if (users.Count == 0)
            {
                users.Add(new OnlineUser
                {
                    UniqueID = 1,
                    RoomLevel = 3,
                    DeskNo = 8,
                    Num = 6
                });
            }

            #endregion

            return users;
        }
    }
}