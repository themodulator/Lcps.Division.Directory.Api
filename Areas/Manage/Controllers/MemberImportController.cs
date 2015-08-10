using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lcps.Division.Directory.Providers;

namespace Lcps.Division.Directory.API.Areas.Manage.Controllers
{
    public class MemberImportController : Controller
    {
        // GET: Manage/MemberImport
        public ActionResult Index()
        {

            ImportTextFile m = new ImportTextFile("Import Division Members", "\t", new MvcRouteDefinition() { Action = "Preview", Area = "Manage", Controller = "MemberImport" });
            return View(m);
        }

        [HttpPost]
        public ActionResult Preview(ImportTextFile txt)
        {
            txt.Load();
            return View(txt);
        }
    }
}