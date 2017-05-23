using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Racing.Moto.Core.Utils
{
    public class AuthenticationUtil
    {
        public static string GetLoginUserGuid()
        {
            var guid = "";

            HttpCookie authCookie = HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
            System.Web.Security.FormsAuthenticationTicket authTicket = System.Web.Security.FormsAuthentication.Decrypt(authCookie.Value);

            guid = authTicket.UserData;

            return guid;
        }
    }
}
