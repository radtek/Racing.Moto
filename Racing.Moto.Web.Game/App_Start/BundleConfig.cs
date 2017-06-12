using System.Web;
using System.Web.Optimization;

namespace Racing.Moto.Game.Web
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // js
            bundles.Add(new ScriptBundle("~/RacingMotoJs")
                .Include("~/Scripts/jQuery/jquery-{version}.js")
                .Include("~/Scripts/modernizr-{version}.js")
            //.Include("~/Scripts/app.js")
            );
            bundles.Add(new ScriptBundle("~/Racing")
                .Include("~/Scripts/jquery.easing.1.3.js")
                .Include("~/Scripts/jquery.timer.js")
                .Include("~/Scripts/jquery.floatingBg.js")
                .Include("~/Scripts/jquery.floating.js")
                .Include("~/Scripts/PK/Racing.js")
            );


            // css
            bundles.Add(new StyleBundle("~/RacingMotoCss")
                .Include("~/Content/Site.css")
            );
        }
    }
}
