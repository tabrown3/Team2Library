using System;
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

        public ActionResult Books(string bookGenre, string bookPublisher)
        {

            //Catalog Filter
            var genreList = new List<string>();
            var genreQuery = from g in db.Genres
                             orderby g.Genre1
                             select g.Genre1;

            var publisherList = new List<string>();
            var publisherQuery = from p in db.Books
                                 orderby p.Publisher
                                 select p.Publisher;

            genreList.AddRange(genreQuery.Distinct());
            publisherList.AddRange(publisherQuery.Distinct());

            ViewBag.bookGenre = new SelectList(genreList);
            ViewBag.bookPublisher = new SelectList(publisherList);

            var books = from b in db.Books
                        select b;

            if (!string.IsNullOrEmpty(bookGenre))
            {
                switch (bookGenre)
                {
                    case "Nonfiction":
                        books = books.Where(x => x.Genre == 1);
                        break;
                    case "Fiction":
                        books = books.Where(x => x.Genre == 2);
                        break;
                    case "Fantasy":
                        books = books.Where(x => x.Genre == 3);
                        break;
                    case "Thriller":
                        books = books.Where(x => x.Genre == 4);
                        break;
                    case "Short Story":
                        books = books.Where(x => x.Genre == 5);
                        break;
                    case "Tragedy":
                        books = books.Where(x => x.Genre == 6);
                        break;
                    case "Science Fiction":
                        books = books.Where(x => x.Genre == 7);
                        break;
                    case "Philosophy":
                        books = books.Where(x => x.Genre == 8);
                        break;
                    case "Reference":
                        books = books.Where(x => x.Genre == 9);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(bookPublisher))
            {
                books = books.Where(x => x.Publisher == bookPublisher);
            }
            return View(books);
        }

    }
}            