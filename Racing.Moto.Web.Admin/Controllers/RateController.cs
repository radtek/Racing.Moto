using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Admin.Controllers
{
    public class RateController : AdminBaseController
    {
        #region 赔率设置
        public ActionResult Setting(int id = 0)
        {
            // 0:竞技场, 1: 娱乐场a, 2: 娱乐场b, 3: 娱乐场c
            ViewBag.RateType = id;

            return View();
        }
        #endregion
    }
}