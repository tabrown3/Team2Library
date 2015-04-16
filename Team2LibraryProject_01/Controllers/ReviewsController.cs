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
    public class ReviewsController : Controller
    {
        private Team2LibraryEntities db = new Team2LibraryEntities();

        static string currentISBN;

        //Review Report
        public ActionResult ReviewReport()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.MemberReviewsViews.ToList());
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // GET: Reviews
        public ActionResult Index()
        {
            var reviews = db.Reviews.Include(r => r.Book).Include(r => r.Member);
            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        [HttpGet]
        public ActionResult ReviewDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }

            ViewBag.BookTitle = review.Book.Title;

            return View(review);
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName");
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReviewID,CardNo,Rating,ReviewText,ISBN")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName", review.ISBN);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", review.CardNo);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName", review.ISBN);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", review.CardNo);
            currentISBN = review.ISBN;
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReviewID,CardNo,Rating,ReviewText,ISBN")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.CardNo = Globals.currentID;
                review.ISBN = currentISBN;
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName", review.ISBN);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", review.CardNo);

            //Pulling all reviews related to the book
            var ratings = db.Database.SqlQuery<Single>("SELECT Rating FROM dbo.Review WHERE ISBN = {0}", currentISBN).ToArray();

            float sum = 0;
            float ratingAv = 0;

            //Average the review
            for (int i = 0; i < ratings.Length; i++)
            {
                sum = sum + ratings[i];
            }

            ratingAv = sum / ratings.Length;

            //Directly update table values
            var bookUpdateSQL = @"UPDATE dbo.Book SET Rating = {0} WHERE ISBN = {1}";
            db.Database.ExecuteSqlCommand(bookUpdateSQL, System.Math.Round(ratingAv, 2), currentISBN);

            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("DeleteReview")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();

            //Pulling all reviews related to the book
            var ratings = db.Database.SqlQuery<Single>("SELECT Rating FROM dbo.Review WHERE ISBN = {0}", currentISBN).ToArray();

            float sum = 0;
            float ratingAv = 0;

            //Average the review
            for (int i = 0; i < ratings.Length; i++)
            {
                sum = sum + ratings[i];
            }

            ratingAv = sum / ratings.Length;

            //Directly update table values
            var bookUpdateSQL = @"UPDATE dbo.Book SET Rating = {0} WHERE ISBN = {1}";
            db.Database.ExecuteSqlCommand(bookUpdateSQL, System.Math.Round(ratingAv, 2), currentISBN);

            return RedirectToAction("Index", "Manage");
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
