using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Racing.Moto.Services.Mvc
{
    public class BaseController : Controller
    {
        private User _loginUser = null;
        public User LoginUser
        {
            get
            {
                return _loginUser;
            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (HttpContext.User.Identity.IsAuthenticated && _loginUser == null)
            {
                if (System.Web.HttpContext.Current.Session[nameof(LoginUser)] == null)
                {
                    _loginUser = SqlMembershipProvider.Provider.GetUser(HttpContext.User.Identity.Name, true);

                    // LoginUser session
                    System.Web.HttpContext.Current.Session[nameof(LoginUser)] = _loginUser;
                }
                else
                {
                    _loginUser = System.Web.HttpContext.Current.Session[nameof(LoginUser)] as User;
                }
            }
            else
            {
                ViewBag.ReturnUrl = filterContext.HttpContext.Request.Url.ToString();
            }

            ViewBag.CurrentUser = _loginUser;
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new NewJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected JsonResult Json(object data, List<string> excludeProperty, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet)
        {
            return new NewJsonResult(excludeProperty)
            {
                Data = data,
                JsonRequestBehavior = behavior
            };
        }
    }

    public static class PKBag
    {
        public static User LoginUser
        {
            get
            {
                return System.Web.HttpContext.Current.Session[nameof(LoginUser)] as User;
            }
        }
    }

    #region JsonResult

    public class NewJsonResult : JsonResult
    {
        private JsonSerializerSettings Settings { get; set; }

        public NewJsonResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include
            };
        }

        public NewJsonResult(List<string> excludeProperty)
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new ExcludePropertiesContractResolver(excludeProperty)
            };
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data == null)
                return;

            response.Write(JsonConvert.SerializeObject(Data, Formatting.Indented, this.Settings));
        }
    }

    /// <summary>
    /// 排除某些属性
    /// </summary>
    public class ExcludePropertiesContractResolver : DefaultContractResolver
    {
        private List<string> ExcludeProperty { get; set; }

        public ExcludePropertiesContractResolver(List<string> excludeProperty)
        {
            ExcludeProperty = excludeProperty;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (ExcludeProperty.Contains(member.Name))
            {
                return null;
            }
            return base.CreateProperty(member, memberSerialization);
        }
    }

    #endregion
}
