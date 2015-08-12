using System.Collections.Generic;
using System.Web;
using System.Web.Optimization;
using System.Configuration;

namespace Lcps.Division.Directory.API
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));


            

            List<string> l = new List<string>();


            l.Add("~/content/font-awesome.css");
            l.Add("~/content/site.css");

            /*
            if (System.Configuration.ConfigurationManager.AppSettings["currentTheme"] == null)
                l.Add("~/content/bootstrap.css");
            else
            {
                string theme = System.Configuration.ConfigurationManager.AppSettings["currentTheme"];
                string path = System.Configuration.ConfigurationManager.AppSettings[theme];
                l.Add(path);
            }
            */    



            bundles.Add(new StyleBundle("~/Content/css").Include(l.ToArray()));
        }
    }
}
