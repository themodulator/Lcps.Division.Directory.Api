using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Lcps.Division.Directory.Providers;
using Lcps.Division.Directory.Repository;
using Lcps.Division.Directory.API.Areas.Manage.Models;
using PagedList;
using Lcps.Division.Directory.Infrastructure;

namespace Lcps.Division.Directory.API.Areas.Manage.Controllers
{
    public class MemberImportController : Controller
    {
        public const string varName = "DirectoryMemberImport";

        private LcpsAccountManager _userManager;

        public LcpsAccountManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<LcpsAccountManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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

            foreach (DirectoryMember member in txt.Items)
            {
                member.GetSyncStatus();
            }

            return RedirectToAction("MemberPreview");
        }

        public ActionResult MemberPreview(int? page, ImportSyncStatus? sync, string search)
        {
            page = (page == null) ? 1 : page;

            ImportTextFile<DirectoryMember> importFile = (ImportTextFile<DirectoryMember>)Session[varName];

            ViewBag.Total = importFile.Items.Count();
            ViewBag.Page = page.Value;
            ViewBag.Insert = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Insert).Count();
            ViewBag.Current = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Current).Count();
            ViewBag.Update = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Update).Count();
            ViewBag.Error = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Error).Count();


            List<DirectoryMember> mm = importFile.Items;

            if (sync != null)
                mm = importFile.Items.Where(x => x.SyncStatus == sync).ToList();
            
            if(search != null)
                if (search != null)
                    mm = importFile.Items.Where(x => x.GetMembershipScopeCaption().Contains(search) |
                        x.SurName.ToLower().Contains(search.ToLower()) |
                        x.GivenName.ToLower().Contains(search.ToLower()) |
                        x.InternalId.ToLower().Contains(search.ToLower()) |
                        x.UserName.ToLower().Contains(search.ToLower())).ToList();


            PagedList<DirectoryMember> pg = new PagedList<DirectoryMember>(mm, page.Value, 12);
            

            return View(pg);
        }

        public ActionResult ImportMembers()
        {
            ImportTextFile<DirectoryMember> importFile = (ImportTextFile<DirectoryMember>)Session[varName];

            foreach(DirectoryMember m in importFile.Items)
            {
                switch(m.SyncStatus)
                {
                    case ImportSyncStatus.Error :
                        m.ImportStatus = BootstrapStatus.danger;
                        break;
                    case ImportSyncStatus.Insert:
                        m.UserManager = this.UserManager;
                        m.Insert();
                        break;
                    case ImportSyncStatus.Update:
                        m.Update();
                        break;
                }
            }

            Session[varName] = importFile;

            return RedirectToAction("ImportReport");
        }

        public ActionResult ImportReport(ImportSyncStatus? sync, BootstrapStatus? import, int? page)
        {
            ImportTextFile<DirectoryMember> importFile = (ImportTextFile<DirectoryMember>)Session[varName];

            ViewBag.Total = importFile.Items.Count();
            
            ViewBag.InsertSuccess = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Insert & x.ImportStatus == BootstrapStatus.success).Count();
            ViewBag.InsertErrors = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Insert & x.ImportStatus == BootstrapStatus.danger).Count();

            ViewBag.UpdateSuccess = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Update & x.ImportStatus == BootstrapStatus.success).Count();
            ViewBag.UpdateErrors = importFile.Items.Where(x => x.SyncStatus == ImportSyncStatus.Update & x.ImportStatus == BootstrapStatus.danger).Count();

            ViewBag.Errors = importFile.Items.Where(x => x.ImportStatus == BootstrapStatus.danger).Count();

            List<DirectoryMember> members = null;

            if(sync == null & import == null)
            {
                members = importFile.Items;
            }

            if (sync != null & import == null)
            {
                members = importFile.Items.Where(x => x.SyncStatus == sync).ToList();
            }

            if (sync == null & import != null)
            {
                members = importFile.Items.Where(x => x.ImportStatus == import).ToList();
            }

            if (sync != null & import != null)
            {
                members = importFile.Items.Where(x => x.SyncStatus == sync & x.ImportStatus == import).ToList();
            }

            ViewBag.Page = (page == null) ? 1 : page;

            PagedList<DirectoryMember> pg = new PagedList<DirectoryMember>(members, ViewBag.Page, 12);

            return View(pg);



        }
        
    }
}