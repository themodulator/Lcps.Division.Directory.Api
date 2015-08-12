using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Lcps.Division.Directory.Repository;
using Lcps.Division.Directory.Infrastructure;

namespace Lcps.Division.Directory.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // ---------- Instantiating this instance of the context adds the connection 
            // string to the properties section
            Lcps.Division.Directory.API.Infrastructure.LcpsApiRepositoryContext db = new Infrastructure.LcpsApiRepositoryContext();

        }
    }
}
