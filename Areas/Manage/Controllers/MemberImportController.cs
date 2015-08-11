using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lcps.Division.Directory.Providers;
using Lcps.Division.Directory.Repository;
using Lcps.Division.Directory.API.Areas.Manage.Models;
using PagedList;

namespace Lcps.Division.Directory.API.Areas.Manage.Controllers
{
    public class MemberImportController : Controller
    {
        public const string varName = "DiretcoryMember";


        // GET: Manage/MemberImport
        public ActionResult Index()
        {

            ImportTextFile<DirectoryMember> m = new ImportTextFile<DirectoryMember>("Import Division Members", "\t", new MvcRouteDefinition() { Action = "Preview", Area = "Manage", Controller = "MemberImport" });
            return View(m);
        }

        [HttpPost]
        public ActionResult Preview(ImportTextFile<DirectoryMember> txt)
        {
            txt.Load();
            Session[varName] = txt;
            return RedirectToAction("MemberPreview");

            
        }

        public ActionResult MemberPreview(int? page)
        {
            page = (page == null) ? 1 : page;

            ImportTextFile<DirectoryMember> items = (ImportTextFile<DirectoryMember>)Session[varName];

            ViewBag.Total = items.Items.Count();
            ViewBag.Page = page.Value;

            PagedList<DirectoryMember> members = new PagedList<DirectoryMember>(items.Items, page.Value, 12);

            return View(members);
        }
    }
}