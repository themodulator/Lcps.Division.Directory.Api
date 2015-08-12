using Lcps.Division.Directory.API.Areas.Manage.Models;
using Lcps.Division.Directory.API.Infrastructure;
using Lcps.Division.Directory.Infrastructure;
using Lcps.Division.Directory.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Lcps.Division.Directory.API.Areas.Manage.Controllers
{
    public class DirectoryMembersController : Controller
    {
        private DirectoryContext db = new DirectoryContext();
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

        // GET: Manage/DirectoryMembers
        public ActionResult Index(int? page, long? filter, string search, string theme)
        {

            List<DirectoryMemberInfo> mm;

            if (filter == null)
                mm = db.DirectoryMembers.Get().OrderBy(x => x.SurName + x.GivenName).ToList();
            else
            {
                mm = DirectoryMember.GetByFilter(filter.Value);
                ViewBag.Filter = filter.Value;
            }

            if(theme != null)
                Session["Theme"] = theme;



            List<DirectoryMember> dd = new List<DirectoryMember>();
            foreach (DirectoryMemberInfo d in mm)
            {
                dd.Add(new DirectoryMember(d));
            }

            

            page = (page == null) ? 1 : page;

            ViewBag.Page = page.Value;




            if (search != null)
                dd = dd.Where(x => x.GetMembershipScopeCaption().Contains(search) |
                    x.SurName.ToLower().Contains(search.ToLower()) |
                    x.GivenName.ToLower().Contains(search.ToLower()) |
                    x.InternalId.ToLower().Contains(search.ToLower()) |
                    x.UserName.ToLower().Contains(search.ToLower())).ToList();


            ViewBag.Total = dd.Count();

            PagedList<DirectoryMember> pg = new PagedList<DirectoryMember>(dd, page.Value, 12);

            return View(pg);
        }


        public ActionResult Filter(long? filter)
        {
            List<MembershipScope> scopes = db.MembershipScopes.Get().OrderBy(x => x.Caption).ToList();
            System.Type t = scopes.ToEnum(x => x.LiteralName, x => x.BinaryValue);

            filter = filter ?? 0;

            ViewBag.Filter = filter.Value;

            List<MembershipScopeEditorItem> items = new List<MembershipScopeEditorItem>();
            foreach (MembershipScope scope in scopes)
            {
                items.Add(new MembershipScopeEditorItem()
                {
                    Challenge = filter.Value,
                    Scope = scope,
                    MembershipScopeType = t
                });
            }

            return View(items);
        }

        public ActionResult MembershipScope(string memberId)
        {
            DirectoryMemberInfo p = null;

            if (memberId == null)
                p = new DirectoryMemberInfo()
                {
                    UserName = "johnd",
                    GivenName = "John",
                    SurName = "Doe",
                    MembershipScope = 81
                };
            else
            {
                p = db.DirectoryMembers.GetByID(memberId);
            }


            List<MembershipScope> scopes = db.MembershipScopes.Get().OrderBy(x => x.Caption).ToList();
            System.Type t = scopes.ToEnum(x => x.LiteralName, x => x.BinaryValue);

            List<MembershipScopeEditorItem> items = new List<MembershipScopeEditorItem>();
            foreach (MembershipScope scope in scopes)
            {
                items.Add(new MembershipScopeEditorItem()
                {
                    Challenge = p.MembershipScope,
                    Scope = scope,
                    MembershipScopeType = t
                });
            }


            MembershipScopeModel m = new MembershipScopeModel()
            {
                Person = p,
                MembershipScopes = items
            };


            return View(m);
        }

        // GET: Manage/DirectoryMembers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectoryMemberInfo directoryMember = db.DirectoryMembers.GetByID(id);
            if (directoryMember == null)
            {
                return HttpNotFound();
            }
            return View(directoryMember);
        }

        // GET: Manage/DirectoryMembers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manage/DirectoryMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,InternalId,GivenName,MiddleName,SurName,DOB,Gender,InitialPassword,ConfirmPassword,MembershipScope")] DirectoryMemberInfo directoryMember)
        {
            directoryMember.Id = Guid.NewGuid().ToString();
            if (directoryMember.InitialPassword != directoryMember.ConfirmPassword)
                ModelState.AddModelError("", "The passwords do not match");


            string id = Guid.NewGuid().ToString();

            directoryMember.Id = Guid.NewGuid().ToString();



            if (ModelState.IsValid)
            {
                try
                {
                    string pwd = directoryMember.InitialPassword;
                    directoryMember.ConfirmPassword = null;

                    Anvil.RijndaelEnhanced re = new Anvil.RijndaelEnhanced(DirectoryMember.GuidKey);
                    directoryMember.InitialPassword = re.Encrypt(pwd);

                    IdentityResult r = UserManager.Create(directoryMember, pwd);
                    if (r.Succeeded)
                        return RedirectToAction("MembershipScope", new { memberId = directoryMember.Id });
                    else
                    {
                        foreach (string e in r.Errors)
                        {
                            ModelState.AddModelError("", e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Anvil.Repository.ExceptionCollector ec = new Anvil.Repository.ExceptionCollector(ex);
                    foreach (string e in ec)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            return View(directoryMember);
        }

        // GET: Manage/DirectoryMembers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectoryMemberInfo directoryMember = db.DirectoryMembers.GetByID(id);
            DirectoryMember m = new DirectoryMember(directoryMember);

            if (directoryMember == null)
            {
                return HttpNotFound();
            }
            return View(m);
        }

        public ActionResult UpdateScope(string memberId, long binaryValue)
        {
            DirectoryMemberInfo m = db.DirectoryMembers.GetByID(memberId);
            m.MembershipScope = binaryValue;
            db.DirectoryMembers.Update(m);
            return Content(binaryValue.ToString());
        }

        // POST: Manage/DirectoryMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,InternalId,GivenName,MiddleName,SurName,DOB,Gender,InitialPassword,MembershipScope")] DirectoryMemberInfo directoryMember)
        {
            if (ModelState.IsValid)
            {
                db.DirectoryMembers.Update(directoryMember);
                return RedirectToAction("Index");
            }
            return View(directoryMember);
        }

        // GET: Manage/DirectoryMembers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectoryMemberInfo directoryMember = db.DirectoryMembers.GetByID(id);
            if (directoryMember == null)
            {
                return HttpNotFound();
            }
            return View(directoryMember);
        }

        // POST: Manage/DirectoryMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DirectoryMemberInfo directoryMember = db.DirectoryMembers.GetByID(id);
            db.DirectoryMembers.Delete(directoryMember);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
