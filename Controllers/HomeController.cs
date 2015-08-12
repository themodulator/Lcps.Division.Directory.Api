using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lcps.Division.Directory.API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            //return View();

            return RedirectToAction("Index", "DirectoryMembers", new { area = "Manage" });
        }
    }
}
