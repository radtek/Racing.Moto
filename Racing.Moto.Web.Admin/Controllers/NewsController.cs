using Racing.Moto.Services.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Admin.Controllers
{
    [Authorize]
    public class NewsController : AdminBaseController
    {
        // GET: Admin/News
        public ActionResult Index()
        {
            return View();
        }
    }
}