using System.Web;
using System.Web.Mvc;

namespace Lcps.Division.Directory.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
