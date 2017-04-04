using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Racing.Moto.Web
{
    public class BundleConfig
    {
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


            // css
            bundles.Add(new StyleBundle("~/RacingMotoCss")
                .Include("~/Content/Site.css")
            );
            bundles.Add(new StyleBundle("~/RacingMotoBetCss")
                .Include("~/Content/bet.css")
            );
        }
    }
}