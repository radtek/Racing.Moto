using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Controllers
{
    public class MotoController : Controller
    {
        // 竞技场
        public ActionResult Arena()
        {
            return View();
        }
    }
}