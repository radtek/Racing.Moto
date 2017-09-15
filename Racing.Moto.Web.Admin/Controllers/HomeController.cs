using Racing.Moto.Data.Entities;
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
    public class HomeController : AdminBaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region AppConfig
        public ActionResult AppConfig()
        {
            var appConfig = new AppConfigService().GetAppConfig(DBConst.RacingMoto_Run_Allowable);
            return View(appConfig);
        }

        [HttpPost]
        public JsonResult SaveAppConfig(AppConfig model)
        {
            var result = new ResponseResult();

            try
            {
                new AppConfigService().Update(model.Name, model.Value);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = MessageConst.System_Error;
            }

            return Json(result);
        }
        #endregion
    }
}