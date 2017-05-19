using App.Core.OnlineStat;
using NLog;
using Racing.Moto.Core.Extentions;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
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
            ViewBag.UserType = id ?? 0;

            var statistics = GetOnlineStatistics();

            return View(statistics);
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
                    PKBag.OnlineUserRecorder.Delete(user);
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
                        onlienUsers = allUsers.Where(u => u.ParentUserId == LoginUser.UserId || u.GrandUserId == LoginUser.UserId)
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
                                && (u.ParentUserId == LoginUser.UserId || u.GrandUserId == LoginUser.UserId))
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

        private List<OnlineUser> GetOnlineUsers()
        {
            // 盘口端登录用户 TODO 从盘口接口取数据
            var betUsers = new List<OnlineUser>();

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
            var statistics = PKBag.OnlineUserRecorder.GetUserList().GroupBy(u => u.UserDegree).Select(g => new RoleModel
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