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
            BookDetailsView book = db.BookDetailsViews.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }

            ISBN = book.ISBN;
            ViewBag.Title = book.Title;
            switch(ISBN)
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

            return View(book);
        }

        //Book Search
        public ActionResult BookSearch(string bookGenre, string searchString)
        {
            var genreList = new List<string>();
            var genreQuery = from g in db.Genres
                             orderby g.Genre1
                             select g.Genre1;

            genreList.AddRange(genreQuery.Distinct());
            ViewBag.bookGenre = new SelectList(genreList);

            var books = from b in db.Books
                        select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(bookGenre))
            {
                switch(bookGenre)
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

            return View(books);
        }

        //Book Reviews
        public ActionResult Reviews()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reviews([Bind(Include = "ReviewID,CardNo,Rating,ReviewText,ISBN")] Review review)
        {
            //Tie the review to the book ISBN
            review.ISBN = ISBN;

            //Random function 
            review.ReviewID = Guid.NewGuid().GetHashCode();     

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
            }

            //Pulling all reviews related to the book
            var ratings = db.Database.SqlQuery<Single>("SELECT Rating FROM dbo.Review WHERE ISBN = {0}", ISBN).ToArray();

            float sum = 0;
            float ratingAv = 0;

            //Average the review
            for(int i = 0; i < ratings.Length; i++)
            {
                sum = sum + ratings[i]; 
            }

            ratingAv = sum / ratings.Length;
            
            //Directly update table values
            var bookUpdateSQL = @"UPDATE dbo.Book SET Rating = {0} WHERE ISBN = {1}";
            db.Database.ExecuteSqlCommand(bookUpdateSQL, System.Math.Round(ratingAv, 2), ISBN);


            return RedirectToAction("Books", "Home");
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
