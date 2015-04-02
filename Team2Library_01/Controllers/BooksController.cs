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
    public class BooksController : Controller
    {
        static int bookID = 0;          //What am I doingggggggg
        static int dvdID = 0;
        static string ISBN = null;

        private T2_LibraryEntities db = new T2_LibraryEntities();

        //Books In Catalog
        public ActionResult AGameofThronesASongofIceandFireBook1()
        {
            Book book = db.Books.Find(1, "553593714");
            if (book == null)
            {
                return HttpNotFound();
            }

            bookID = book.BookID;
            ISBN = book.ISBN;

            return View(book);
        }


        public ActionResult RavenorTheOmnibus()
        {
            Book book = db.Books.Find(2, "1844167372");
            if (book == null)
            {
                return HttpNotFound();
            }

            bookID = book.BookID;
            ISBN = book.ISBN;

            return View(book);
        }

        public ActionResult TomClancyFullForceandEffect()
        {
            Book book = db.Books.Find(3, "399173358");
            if (book == null)
            {
                return HttpNotFound();
            }

            bookID = book.BookID;
            ISBN = book.ISBN;

            return View(book);
        }
        public ActionResult ThingsFallApart()
        {
            Book book = db.Books.Find(4, "385474547");
            if (book == null)
            {
                return HttpNotFound();
            }

            bookID = book.BookID;
            ISBN = book.ISBN;

            return View(book);
        }

        //Book Reviews
        public ActionResult Reviews()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reviews([Bind(Include = "ReviewID,BookID,DvdID,CardNo,Rating,ReviewText")] Review review)
        {
            if (bookID == 0)                //All of these are really bad ideas
            {
                review.BookID = null;
            }
            else
            {
                review.BookID = bookID;
            }
            if (dvdID == 0)
            {
                review.DvdID = null;
            }
            else
            {
                review.DvdID = dvdID;
            }

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
            return View(db.Books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
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
            return View();
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookID,ISBN,Author_FName,Author_LName,Publisher,NumOfPages,Title,Year,Genre,Language,Rating,Synopsis,Shelf")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookID,ISBN,Author_FName,Author_LName,Publisher,NumOfPages,Title,Year,Genre,Language,Rating,Synopsis,Shelf")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
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
        public ActionResult DeleteConfirmed(int id)
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
