using NLog;
using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Controllers
{
    public class MotoController : BaseController
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        // 竞技场
        public ActionResult Arena()
        {
            return View();
        }


        // 下注
        public ActionResult Bet()
        {
            return View();
        }
    }
}