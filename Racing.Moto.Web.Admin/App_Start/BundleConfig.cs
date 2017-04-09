using System.Web;
using System.Web.Optimization;

namespace Racing.Moto.Web.Admin
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/RacingMotoBetJs")
                .Include("~/Scripts/AngularJs/angular.js")
                .Include("~/Scripts/AngularJs/angular-filter-{version}.js")
                .Include("~/Scripts/Jquery/jquery-{version}.js")
                .Include("~/Scripts/jquery.timer.js")
                .Include("~/Scripts/timeCountDown.js")
                .Include("~/Scripts/Jquery.inputmask/jquery.inputmask.bundle-{version}.js")
                .Include("~/Scripts/modernizr-{version}.js")
            //.Include("~/Scripts/app.js")
            //.Include("~/Scripts/ngApp.js")
            );
        }
    }
}
