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

            switch (review.ISBN)
            {
                case "553593714":
                    ViewBag.Image = "~/Content/Images/Books/got1_cover.png";
                    break;
                case "1844167372":
                    ViewBag.Image = "~/Content/Images/Books/ravenor_cover.png";
                    break;
                case "399173358":
                    ViewBag.Image = "~/Content/Images/Books/tc_ff_cover.png";
                    break;
                case "385474547":
                    ViewBag.Image = "~/Content/Images/Books/things_cover.png";
                    break;
                case "140481346":
                    ViewBag.Image = "~/Content/Images/Books/dsales_cover.png";
                    break;
                case "486298574":
                    ViewBag.Image = "~/Content/Images/Books/ywall_cover.png";
                    break;
                case "345391802":
                    ViewBag.Image = "~/Content/Images/Books/hitch_cover.png";
                    break;
                case "486404536":
                    ViewBag.Image = "~/Content/Images/Books/calc_cover.png";
                    break;
                case "1439048479":
                    ViewBag.Image = "~/Content/Images/Books/alg_trig_cover.png";
                    break;
                case "195391144":
                    ViewBag.Image = "~/Content/Images/Books/mineral_cover.png";
                    break;
                case "142400556":
                    ViewBag.Image = "~/Content/Images/Books/martin_cover.png";
                    break;
                case "156027321":
                    ViewBag.Image = "~/Content/Images/Books/life_pi_cover.png";
                    break;
                case "1439190143":
                    ViewBag.Image = "~/Content/Images/Books/rin_tin_cover.png";
                    break;
                case "262033844":
                    ViewBag.Image = "~/Content/Images/Books/algorithms_cover.png";
                    break;
                case "486426912":
                    ViewBag.Image = "~/Content/Images/Books/gen_morals_cover.png";
                    break;
                case "140447474":
                    ViewBag.Image = "~/Content/Images/Books/crit_reason_cover.png";
                    break;
                case "143034901":
                    ViewBag.Image = "~/Content/Images/Books/shadow_wind_cover.png";
                    break;
                case "140442936":
                    ViewBag.Image = "~/Content/Images/Books/dream_red_cover.png";
                    break;
                case "765367297":
                    ViewBag.Image = "~/Content/Images/Books/halo_reach_cover.png";
                    break;
                case "143039695":
                    ViewBag.Image = "~/Content/Images/Books/noli_cover.png";
                    break;
            }
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

                TempData["Success"] = "The review has been edited.";
                return RedirectToAction("Index", "Manage");
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName", review.ISBN);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", review.CardNo);

            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult DeleteReview(int? id)
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
            if ((int)sum == 0)
            {
                db.Database.ExecuteSqlCommand(bookUpdateSQL, 0, currentISBN);
            }
            else
            {
                db.Database.ExecuteSqlCommand(bookUpdateSQL, System.Math.Round(ratingAv, 2), currentISBN);
            }

            TempData["Success"] = "The review has been deleted.";
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
