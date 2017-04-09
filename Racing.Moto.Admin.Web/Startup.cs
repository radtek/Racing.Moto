using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Racing.Moto.Admin.Web.Startup))]
namespace Racing.Moto.Admin.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
