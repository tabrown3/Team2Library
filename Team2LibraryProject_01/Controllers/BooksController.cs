using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team2LibraryProject_01.Models;

namespace Team2LibraryProject_01.Controllers
{
    public class BooksController : Controller
    {
        private Team2LibraryEntities db = new Team2LibraryEntities();

        static string ISBN = null;

        //Books In Catalog
       
        [HttpGet]
        public ActionResult BookDetails(string id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }

            ISBN = book.ISBN;
            ViewBag.Title = book.Title;
            switch(ISBN)
            {
                case "553593714":
                    ViewBag.Image = "~/Content/Images/got1_cover.png";
                    break;
                case "1844167372":
                    ViewBag.Image = "~/Content/Images/ravenor_cover.png";
                    break;
                case "399173358":
                    ViewBag.Image = "~/Content/Images/tc_ff_cover.png";
                    break;
                case "385474547":
                    ViewBag.Image = "~/Content/Images/things_cover.png";
                    break;
                case "140481346":
                    ViewBag.Image = "~/Content/Images/dsales_cover.png";
                    break;
                case "486298574":
                    ViewBag.Image = "~/Content/Images/ywall_cover.png";
                    break;
                case "345391802":
                    ViewBag.Image = "~/Content/Images/hitch_cover.png";
                    break;
            }
            return View(book);
        }

        //Book Reviews
        public ActionResult Reviews()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reviews([Bind(Include = "ReviewID,CardNo,Rating,ReviewText,ISBN")] Review review)
        {

            review.ISBN = ISBN;                                 //Tie the review to the book ISBN

            review.ReviewID = Guid.NewGuid().GetHashCode();     //Random function

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Books", "Home");
            }
            return View(review);
        }


        // GET: Books
        public ActionResult Index()
        {
            var books = db.Books.Include(b => b.Genre1).Include(b => b.Language1);
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.Genre = new SelectList(db.Genres, "GenreID", "Genre1");
            ViewBag.Language = new SelectList(db.Languages, "LanguageID", "Language1");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ISBN,Author_FName,Author_LName,Publisher,NumOfPages,Title,Year,Genre,Language,Rating,Synopsis,Shelf")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Genre = new SelectList(db.Genres, "GenreID", "Genre1", book.Genre);
            ViewBag.Language = new SelectList(db.Languages, "LanguageID", "Language1", book.Language);
            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.Genre = new SelectList(db.Genres, "GenreID", "Genre1", book.Genre);
            ViewBag.Language = new SelectList(db.Languages, "LanguageID", "Language1", book.Language);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ISBN,Author_FName,Author_LName,Publisher,NumOfPages,Title,Year,Genre,Language,Rating,Synopsis,Shelf")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Genre = new SelectList(db.Genres, "GenreID", "Genre1", book.Genre);
            ViewBag.Language = new SelectList(db.Languages, "LanguageID", "Language1", book.Language);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
