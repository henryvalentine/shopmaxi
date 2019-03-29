using System.Web;
using System.Web.Optimization;

namespace ShopKeeper
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/require.js",
                        "~/Scripts/r.js",
                        //"~/Scripts/angular-file-upload-shim.min.js",
                         "~/Scripts/angular.min.js",
                          "~/Scripts/angular-route.js",
                        // "~/Scripts/angular-file-upload.min.js",
                         "~/Scripts/angularAMD.js",
                         "~/Scripts/angular-route.min.js",
                        //  "~/Scripts/angular-block-ui.min.js",
                        // "~/Scripts/ngDialog.js",
                        // "~/Scripts/angular-route.min.js",
                         "~/Scripts/ui-bootstrap-tpls-0.11.2.js",
                         "~/Scripts/angular-resource.min.js",
                         "~/Scripts/angular-sanitize.min.js",
                         "~/Scripts/html5.js",
                         "~/Scripts/jquery.dataTables.js",
                        // "~/Scripts/dataTableDescription.js",
                        // "~/Scripts/typeahead.bundle.js",
                         //"~/Scripts/Highcharts-4.0.1/js/highcharts.js",
                         "~/Scripts/exporting.js",
                         "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/dateScript.js",
                        "~/Scripts/jquery-ui-1.10.4.min.js"
                        //"~/Scripts/respond.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/jquery.dataTables.css",
                      "~/Content/bootstrap.css",
                      "~/Content/ngDialog.css",
                      "~/Content/ngDialog-theme-flat.css",
                      "~/Content/ngDialog-theme-default.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/bootstrap-datepicker3.css",
                      "~/Content/site.css",
                      "~/Content/custom.css",
                       "~/Content/feedbackmessage.css",
                       "~/Content/formControls.css"));
        }
    }
}
