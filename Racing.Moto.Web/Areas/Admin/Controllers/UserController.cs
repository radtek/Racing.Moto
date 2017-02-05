using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        #region 所有用户

        public ActionResult All()
        {
            return View();
        }

        #endregion

        #region 总代理

        public ActionResult GeneralAgent()
        {
            return View();
        }

        #endregion

        #region 代理

        public ActionResult Agent()
        {
            return View();
        }

        #endregion

        #region 会员

        public ActionResult Member()
        {
            return View();
        }

        #endregion

        #region 游客

        public ActionResult Vistor()
        {
            return View();
        }

        #endregion
    }
}