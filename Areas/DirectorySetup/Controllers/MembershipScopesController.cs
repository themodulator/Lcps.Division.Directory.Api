using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Lcps.Division.Directory.API.Infrastructure;
using Lcps.Division.Directory.Repository;
using Lcps.Division.Directory.Infrastructure;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;

namespace Lcps.Division.Directory.API.Areas.DirectorySetup.Controllers
{
    public class MembershipScopesController : Controller
    {
        private DirectoryContext db = new DirectoryContext();

        [HttpGet]
        public ActionResult CaptionExists(string caption)
        {
            MembershipScope s = db.MembershipScopes.First(x => x.Caption.ToLower().Equals(caption.ToLower()));
            if (s == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            string l = Regex.Replace(caption, @"[^A-Za-z0-9]+", "");
            s = db.MembershipScopes.First(x => x.LiteralName.ToLower().Equals(l.ToLower()));

            return Json(s != null, JsonRequestBehavior.AllowGet);
        }

        // GET: DirectorySetup/MembershipScopes
        public ActionResult Index()
        {
           

            List<MembershipScope> items = new List<MembershipScope>();

            items = db.MembershipScopes.Get().OrderBy(x => x.Caption).ToList();

            return View(items);
            
        }

        // GET: DirectorySetup/MembershipScopes/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipScope membershipScope = db.MembershipScopes.GetByID(id);
            if (membershipScope == null)
            {
                return HttpNotFound();
            }
            return View(membershipScope);
        }

        // GET: DirectorySetup/MembershipScopes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DirectorySetup/MembershipScopes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Caption,ScopeQualifier")] MembershipScope membershipScope)
        {
            MembershipScope ms = db.MembershipScopes.First(x => x.Caption.ToLower().Equals(membershipScope.Caption.ToLower()));
            if (ms != null)
                ModelState.AddModelError("", string.Format("{0} already exists as {1}", ms.Caption, ms.ScopeQualifier.ToString()));


            string lit = MembershipScopeRepository.GenerateLiteralName(membershipScope.Caption);
            ms = db.MembershipScopes.First(x => x.LiteralName.ToLower().Equals(lit.ToLower()));
            if (ms != null)
                ModelState.AddModelError("", string.Format("Generating the scope's literal value resulted in a duplicate Caption as {1}", ms.ScopeQualifier.ToString()));


            if (membershipScope.ScopeQualifier == MembershipScopeQualifier.None)
                ModelState.AddModelError("", "Please choose a valid qualifier");

            if (ModelState.IsValid)
            {
                membershipScope.MembershipScopeId = Guid.NewGuid();
                db.MembershipScopes.Insert(membershipScope);
                return RedirectToAction("Index");
            }

            return View(membershipScope);
        }

        public ActionResult ExportXml()
        {
            List<MembershipScope> items = db.MembershipScopes.Get().OrderBy(x => x.Caption).ToList();
            MemoryStream ms = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(typeof(List<MembershipScope>));
            xml.Serialize(ms, items);

            return File(ms.ToArray(), "text/xml", "MembershipScopes.xml");
        }

        // GET: DirectorySetup/MembershipScopes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipScope membershipScope = db.MembershipScopes.GetByID(id);
            if (membershipScope == null)
            {
                return HttpNotFound();
            }
            return View(membershipScope);
        }

        // POST: DirectorySetup/MembershipScopes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MembershipScopeId,BinaryValue,Caption,ScopeQualifier")] MembershipScope membershipScope)
        {
            if (ModelState.IsValid)
            {
                membershipScope.LiteralName = MembershipScopeRepository.GenerateLiteralName(membershipScope.Caption);
                db.MembershipScopes.Update(membershipScope);
                return RedirectToAction("Index");
            }
            return View(membershipScope);
        }

        // GET: DirectorySetup/MembershipScopes/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipScope membershipScope = db.MembershipScopes.GetByID(id);
            if (membershipScope == null)
            {
                return HttpNotFound();
            }
            return View(membershipScope);
        }

        // POST: DirectorySetup/MembershipScopes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MembershipScope membershipScope = db.MembershipScopes.GetByID(id);
            db.MembershipScopes.Delete(membershipScope);
            return RedirectToAction("Index");
        }

    }
}
