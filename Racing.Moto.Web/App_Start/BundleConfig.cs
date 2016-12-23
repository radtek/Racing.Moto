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
            bundles.Add(new ScriptBundle("~/RacingMotoJs")
                .Include("~/Scripts/AngularJs/angular.js")
                .Include("~/Scripts/modernizr-{version}.js")
                .Include("~/Scripts/app.js")
            );
            bundles.Add(new ScriptBundle("~/jQuery")
                .Include("~/Scripts/jQuery/jquery-{version}.js")
            );

            //bundles.Add(new StyleBundle("~/RacingMotoCss")
            //    .Include("~/Contents/Site.css")
            //    .Include("~/Scripts/Plugins/xtForm/xtFrom.css")
            //);
        }
    }
}