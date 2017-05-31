using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Racing.Moto.Web.Game.Startup))]
namespace Racing.Moto.Web.Game
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
