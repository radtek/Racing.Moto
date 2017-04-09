using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Controllers
{
    public class RouteController : Controller
    {
        // GET: Route
        public ActionResult Index()
        {
            ViewBag.RouteKeyMatched = false;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string id)
        {
            var routeKey = ConfigurationManager.AppSettings["RouteKey"];
            if (!string.IsNullOrEmpty(id) && id == routeKey)
            {
                ViewBag.RouteKeyMatched = true;
            }
            else
            {
                ViewBag.RouteKeyMatched = false;
            }
            return View();
        }
    }
}