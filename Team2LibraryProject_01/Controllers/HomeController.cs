﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team2LibraryProject_01.Models;

namespace Team2LibraryProject_01.Controllers
{
    public class HomeController : Controller
    {

        private Team2LibraryEntities db = new Team2LibraryEntities();

        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return View("Admin");
            else
            {
                var pendingLoan = db.Loans.Where(x => x.CardNo == Globals.currentID).ToList();
                foreach(var item in pendingLoan)
                {
                    if(item.Fines > 0 && item.FinesPaid == false)
                    {
                        ViewBag.Alert = "Alert! You have pending fines. Please go to the account page as soon as possible.";
                        return View();
                    }
                }

                return View();
            }
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

        public ActionResult Books(string bookGenre, string bookPublisher)
        {
            var books = from b in db.Books
                        select b;

           
            return View(books);
        }

        public ActionResult Admin()
        {
            if (User.IsInRole("Admin"))
                return View();
            else
                return View("Index");
        }
    }
}            