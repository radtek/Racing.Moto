using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Areas.Admin.Controllers
{
    public class BetController : AdminBaseController
    {
        // GET: Admin/Bet
        public ActionResult Raise()
        {
            return View();
        }
    }
}