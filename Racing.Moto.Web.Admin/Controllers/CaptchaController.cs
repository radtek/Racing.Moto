using Racing.Moto.Core.Captcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Web.Admin.Controllers
{
    public class CaptchaController : Controller
    {
        public FileContentResult Generate()
        {
            var captchaResult = new CaptchaHelper(12).Generate();

            Session[CaptchaConst.REG_CAPTCHA_SESSION] = captchaResult.CaptchaCode; 

            return File(captchaResult.CaptchaImage, "image/Gif");
        }
    }
}