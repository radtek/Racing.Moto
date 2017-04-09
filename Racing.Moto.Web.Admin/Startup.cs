using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Racing.Moto.Web.Admin.Startup))]
namespace Racing.Moto.Web.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
