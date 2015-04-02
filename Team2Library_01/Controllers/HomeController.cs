using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team2Library_01.Models;

namespace Team2Library_01.Controllers
{
    public class HomeController : Controller
    {

        private T2_LibraryEntities db = new T2_LibraryEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Books()
        {
            return View(db.Books.ToList());
        }

        public ActionResult DVDs()
        {
            return View(db.DVDs.ToList());
        }

    }
}