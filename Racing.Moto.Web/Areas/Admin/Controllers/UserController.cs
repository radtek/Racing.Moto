using NLog;
using Racing.Moto.Core.Extentions;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Models;
using Racing.Moto.Services;
using Racing.Moto.Services.Constants;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 设计版

        #region 所有用户

        public ActionResult All()
        {
            ViewBag.UserType = UserType.All;

            return View();
        }

        #endregion
        
        #region 游客

        public ActionResult Vistor()
        {
            //ViewBag.UserType = UserType.Vistor;

            return View();
        }

        #endregion

        #endregion

        public JsonResult GetUsers(UserSearchModel searchModel)
        {
            var result = new ResponseResult();

            try
            {
                var pager = new UserService().GetUsers(searchModel);

                // 在线用户
                var onlienUsers = PKBag.OnlineUserRecorder.GetUserList();
                foreach (var user in pager.Items)
                {
                    user.IsOnline = onlienUsers.Where(u => u.UserName == user.UserName).Any();
                }

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

        #region 网站参考版

        #region 在线

        /// <summary>
        /// 在线
        /// </summary>
        /// <param name="id">1: admin, 2:总代理, 3: 代理, 4: 会员</param>
        /// <returns></returns>
        public ActionResult Online(int id)
        {
            ViewBag.UserType = id;

            return View();
        }

        public JsonResult GetOnlineUsers(UserSearchModel searchModel)
        {
            var result = new ResponseResult();

            try
            {
                var skip = (searchModel.PageIndex - 1) * searchModel.PageSize;
                var onlienUsers = PKBag.OnlineUserRecorder.GetUserList().Skip(skip).Take(searchModel.PageSize).ToList();
                var userNames = onlienUsers.Select(u => u.UserName).ToList();
                var users = new UserService().GetUsers(userNames);

                var pager = new PagerResult<User>();
                pager.Items = users;
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

        #endregion

        #region 用户管理
        #region 总代理

        public ActionResult GeneralAgent()
        {
            ViewBag.UserType = UserType.GeneralAgent;

            return View();
        }

        #endregion

        #region 代理

        public ActionResult Agent()
        {
            ViewBag.UserType = UserType.Agent;

            return View();
        }

        #endregion

        #region 会员

        public ActionResult Member()
        {
            ViewBag.UserType = UserType.Member;

            return View();
        }

        #endregion

        ///// <summary>
        ///// 用户
        ///// </summary>
        ///// <param name="id">2:总代理, 3: 代理, 4: 会员</param>
        ///// <returns></returns>
        //public ActionResult List(int id)
        //{
        //    ViewBag.UserType = id;

        //    return View();
        //}

        #endregion

        #endregion
    }
}