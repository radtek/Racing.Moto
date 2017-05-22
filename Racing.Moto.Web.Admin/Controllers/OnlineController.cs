using App.Core.OnlineStat;
using Newtonsoft.Json;
using NLog;
using Racing.Moto.Core.Extentions;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Admin.Controllers
{
    public class OnlineController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region  在线人数
        /// <summary>
        /// 在线人数
        /// </summary>
        /// <param name="id">0: 全部, 2:总代理, 3: 代理, 4: 会员</param>
        /// <returns></returns>
        public ActionResult Management(int? id)
        {

            return Redirect("/News/Index");

            //ViewBag.UserType = id ?? 0;

            //var statistics = GetOnlineStatistics();

            //return View(statistics);
        }

        /// <summary>
        /// 踢出用户
        /// </summary>
        /// <param name="id">userName</param>
        /// <returns></returns>
        public JsonResult KickOut(string id)
        {
            var result = new ResponseResult();

            try
            {
                var user = PKBag.OnlineUserRecorder.GetUser(id);
                if (user != null)
                {
                    // 管理员/总代理/代理
                    PKBag.OnlineUserRecorder.Delete(user);
                }
                else
                {
                    // 会员
                    KickOutBetUser(id);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        #endregion

        public JsonResult GetOnlineUsers(UserSearchModel searchModel)
        {
            var result = new ResponseResult();

            try
            {
                var allUsers = GetOnlineUsers();

                var skip = (searchModel.PageIndex - 1) * searchModel.PageSize;
                var onlienUsers = new List<OnlineUser>();

                var isAdmin = LoginUser.UserRoles.Any(u => u.RoleId == RoleConst.Role_Id_Admin);//管理员
                if (searchModel.UserType == 0)
                {
                    //在线人数页面
                    if (isAdmin)
                    {
                        onlienUsers = allUsers.OrderBy(u => u.UserName).Skip(skip).Take(searchModel.PageSize).ToList();
                    }
                    else
                    {
                        onlienUsers = allUsers.Where(u => u.UniqueID == LoginUser.UserId || u.ParentUserId == LoginUser.UserId || u.GrandUserId == LoginUser.UserId)
                            .OrderBy(u => u.UserName).Skip(skip).Take(searchModel.PageSize).ToList();
                    }
                }
                else
                {
                    //在线会员/在线代理/在线总代理
                    if (isAdmin)
                    {
                        onlienUsers = allUsers
                            .Where(u => u.UserDegree == searchModel.UserType)
                            .OrderBy(u => u.UserName).Skip(skip).Take(searchModel.PageSize).ToList();
                    }
                    else
                    {
                        onlienUsers = allUsers
                            .Where(u => u.UserDegree == searchModel.UserType
                                && (u.UniqueID == LoginUser.UserId || u.ParentUserId == LoginUser.UserId || u.GrandUserId == LoginUser.UserId))
                            .OrderBy(u => u.UserName).Skip(skip).Take(searchModel.PageSize).ToList();
                    }
                }
                //var onlienUsers = searchModel.UserType > 0
                //    ? allUsers.Where(u => u.UserDegree == searchModel.UserType).OrderBy(u => u.UserName).Skip(skip).Take(searchModel.PageSize).ToList()
                //    : allUsers.OrderBy(u => u.UserName).Skip(skip).Take(searchModel.PageSize).ToList();
                //var userNames = onlienUsers.Select(u => u.UserName).ToList();
                //var users = new UserService().GetUsers(userNames);

                var pager = new PagerResult<OnlineUser>();
                pager.Items = onlienUsers;
                pager.RowCount = PKBag.OnlineUserRecorder.GetUserList().Count;
                pager.PageCount = pager.RowCount % searchModel.PageSize == 0 ? pager.RowCount / searchModel.PageSize : pager.RowCount / searchModel.PageSize + 1;

                result.Data = pager;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }

        #region private
        private List<OnlineUser> GetOnlineUsers()
        {
            // 盘口端登录用户, 从盘口接口取数据
            var betUsers = GetBetOnlineUsers();

            // 后台登录用户
            var manageUsers = PKBag.OnlineUserRecorder.GetUserList();

            // 所有用户
            var allUsers = new List<OnlineUser>();
            allUsers.AddRange(manageUsers);

            foreach (var user in betUsers)
            {
                if (!allUsers.Any(u => u.UserName == user.UserName))
                {
                    allUsers.Add(user);
                }
            }

            return allUsers;
        }

        /// <summary>
        /// 取盘口在线用户
        /// </summary>
        /// <returns></returns>
        private List<OnlineUser> GetBetOnlineUsers()
        {
            var onlineUsers = new List<OnlineUser>();

            try
            {
                var betPortUrl = ConfigurationManager.AppSettings["BetPortalUrl"];

                RestClient client = new RestClient(betPortUrl);
                var request = new RestRequest("/api/OnlineUser/GetOnlineUsers", Method.POST);
                //request.AddJsonBody(model);

                var response = client.Execute(request);

                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult>(response.Content);

                    if (result.Success)
                    {
                        onlineUsers = JsonConvert.DeserializeObject<List<OnlineUser>>(result.Data.ToString());
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

            return onlineUsers;
        }

        /// <summary>
        /// 踢出盘口用户
        /// </summary>
        /// <param name="userName"></param>
        private void KickOutBetUser(string userName)
        {
            try
            {
                var betPortUrl = ConfigurationManager.AppSettings["BetPortalUrl"];

                RestClient client = new RestClient(betPortUrl);
                var request = new RestRequest("/api/OnlineUser/KickOutUser", Method.POST);

                var onlineUser = new OnlineUser
                {
                    UserName = userName
                };
                request.AddJsonBody(onlineUser);

                var response = client.Execute(request);

                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    var result = JsonConvert.DeserializeObject<ResponseResult>(response.Content);

                    if (!result.Success)
                    {
                        _logger.Info(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex);
            }
        }

        #endregion

        #region 在线用户统计
        /// <summary>
        /// 在线用户统计
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOnlineUsersStatistics()
        {
            var result = new ResponseResult();

            try
            {
                result.Data = GetOnlineStatistics();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
                _logger.Info(ex);
            }

            return Json(result);
        }


        private List<RoleModel> GetOnlineStatistics()
        {
            var allUsers = GetOnlineUsers();
            var statistics = allUsers.Where(u => u.UniqueID == LoginUser.UserId || u.ParentUserId == LoginUser.UserId || u.GrandUserId == LoginUser.UserId).GroupBy(u => u.UserDegree).Select(g => new RoleModel
            {
                RoleId = g.Key,
                RoleName = RoleConst.GetRoleName(g.Key),
                Count = g.Count()
            }).OrderBy(r => r.RoleId).ToList();

            return statistics;
        }

        #endregion

        #region 内部管理

        /// <summary>
        /// 内部管理-在线会员
        /// </summary>
        /// <param name="id">1: admin, 2:总代理, 3: 代理, 4: 会员</param>
        /// <returns></returns>
        public ActionResult Internal(int id)
        {
            var roleId = (id > 1 && id <= 4) ? id : 4;
            ViewBag.UserType = roleId;
            ViewBag.UserTypeName = RoleConst.GetRoleName(roleId);

            return View();
        }

        #endregion
    }
}