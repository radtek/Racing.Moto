using NLog;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Admin.Controllers
{
    [Authorize]
    public class MyCenterController : AdminBaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        #region 用户信息

        // 用户信息
        public ActionResult UserInfo()
        {
            return View();
        }

        #endregion

        #region 修改密码

        // 修改密码
        public ActionResult ChangePwd()
        {
            return View();
        }

        #endregion

        #region 历史注单

        // 历史注单
        public ActionResult BetHistory()
        {
            return View();
        }

        #endregion

        #region 流水一览

        public ActionResult Bonus()
        {
            return View();
        }

        #endregion
    }
}