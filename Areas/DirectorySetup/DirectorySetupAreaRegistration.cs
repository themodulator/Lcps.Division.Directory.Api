using System.Web.Mvc;

namespace Lcps.Division.Directory.API.Areas.DirectorySetup
{
    public class DirectorySetupAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DirectorySetup";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DirectorySetup_default",
                "DirectorySetup/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}