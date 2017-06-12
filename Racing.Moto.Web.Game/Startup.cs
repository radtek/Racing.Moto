using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Racing.Moto.Game.Web.Startup))]
namespace Racing.Moto.Game.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
