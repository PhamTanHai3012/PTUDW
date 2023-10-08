using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyClass.Model;

namespace PTUDW.Controllers
{
    public class SiteController : Controller
    {
        // GET: Site
        public ActionResult Index()
        {   
            MyDBContext db = new MyDBContext();
            int somau = db.Products.Count();
            ViewBag.somau = somau;
            return View();
        }
    }
}