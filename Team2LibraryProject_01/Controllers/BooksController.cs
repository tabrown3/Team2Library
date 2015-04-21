using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using Team2LibraryProject_01.Models;

namespace Team2LibraryProject_01.Controllers
{
    public class BooksController : Controller
    {
        private Team2LibraryEntities db = new Team2LibraryEntities();

        static string ISBN = null;

        //Book Report
        public ActionResult BookCatalogReport()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.BookDetailsViews.ToList());
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }
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

            ViewBag.Image = "~/Content/Images/Books/" + book.ISBN + ".png";

            var inStock = (from i in db.Inventories
                           where i.ISBN == id && i.OnShelf == true
                           select i).ToList();

            if(inStock.Count == 0)
            {
                ViewBag.inStock = false;
            }

            if(inStock.Count > 0)
            {
                ViewBag.inStock = true;
            }

            return View(book);
        }

        //Book Search
        public ActionResult BookSearch(string bookGenre, string searchString, string bookPublisher, string AuthorSearchString)
        {
            var genreList = new List<string>();
            var genreQuery = from g in db.Genres
                             orderby g.Genre1
                             select g.Genre1;

            var publisherList = new List<string>();
            var publisherQuery = from p in db.Books
                                 orderby p.Publisher
                                 select p.Publisher;

            genreList.AddRange(genreQuery.Distinct());
            ViewBag.bookGenre = new SelectList(genreList);

            publisherList.AddRange(publisherQuery.Distinct());
            ViewBag.bookPublisher = new SelectList(publisherList);

            var books = from b in db.Books
                        select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }
            if(!String.IsNullOrEmpty(AuthorSearchString))
            {
                books = books.Where(s => s.Author_FName.Contains(AuthorSearchString) || s.Author_LName.Contains(AuthorSearchString));
            }
            if (!string.IsNullOrEmpty(bookPublisher))
            {
                books = books.Where(x => x.Publisher == bookPublisher);
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
            if (User.IsInRole("Student") || User.IsInRole("Faculty") || User.IsInRole("Admin"))
            {
                //Check if user already reviewed the book
                var alreadyReviewed = (from r in db.Reviews
                                       where r.Member.CardNo == Globals.currentID && r.ISBN == ISBN
                                       select r.Member.Email).ToList();

                if(alreadyReviewed.Count != 0)
                {
                    return View("DuplicateReviewError");
                }
                else
                {
                    BookDetailsView book = db.BookDetailsViews.Find(ISBN);

                    ViewBag.BookTitle = book.Title;
                    return View();
                }
            }
            else
                return View("LoginError");
        }

        [HttpPost]
        public ActionResult Reviews([Bind(Include = "ReviewID,CardNo,Rating,ReviewText,ISBN")] Review review)
        {
            //Tie the review to the book ISBN
            review.ISBN = ISBN;

            //Random function 
            review.ReviewID = Guid.NewGuid().GetHashCode();
            review.CardNo = Globals.currentID;

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

            TempData["Success"] = "Success: Your review has been posted.";
            return RedirectToAction("BookDetails", new { id = ISBN });
        }

        //Login Error
        public ActionResult LoginError()
        {
            return View();
        }
        
        //Duplicate Review Error
        public ActionResult DuplicateReviewError()
        {
            return View();
        }

        //Duplicate Reserve Error
        public ActionResult DuplicateReserveError()
        {
            return View();
        }

        //Loan Confirmation Page
        public ActionResult LoanConfirmation(string id)
        {
            if (User.IsInRole("Admin"))
            {
                //List all books in inventory with the same ISBN as the selected book
                var bookToLoan = db.Inventories.Where(x => x.ISBN == id && x.OnShelf == true);

                var bookInfo = db.Books.Where(x => x.ISBN == id).FirstOrDefault();

                ViewBag.Author = bookInfo.Author_FName + " " + bookInfo.Author_LName;
                ViewBag.BookTitle = bookInfo.Title;
                ViewBag.Publisher = bookInfo.Publisher;

                ViewBag.ItemID = new SelectList(db.Inventories.Where(x => x.ISBN == id && x.OnShelf == true), "ItemID", "ItemID");
                ViewBag.CardNo = new SelectList(db.Members, "CardNo", "CardNo");

                if (ViewBag == null)
                {
                    //Return error of no inventory for the book
                    return HttpNotFound();
                }

                return View(bookToLoan);
            }
            else
                return View("LoginError");
        }

        [HttpPost]
        public ActionResult LoanConfirmation([Bind(Include = "LoanID,ItemID,CardNo,Title,CheckOutDate,DueDate,ReturnDate,Fines,FinesPaid")] Loan loan)
        {
            Random rand = new Random();
            int lID = rand.Next(0, 10000);

            if (ModelState.IsValid)
            {
                loan.LoanID = lID;
                DateTime today = DateTime.Now.Date;

                loan.CheckOutDate = today;

                Member loanMember = db.Members.Find(loan.CardNo);
                Inventory loanBook = db.Inventories.Find(loan.ItemID);

                //Due date dependent on membership
                if (loanMember.RoleID == 1)
                {
                    loan.DueDate = today.AddDays(30);
                }
                else if (loanMember.RoleID == 2)
                {
                    loan.DueDate = today.AddDays(120);
                }

                loan.Title = loanBook.Book.Title;
                loan.FinesPaid = true;

                loanBook.OnShelf = false;

                //Delete matching reservations if the member has any
                var reservation = db.Reservations.Where( x => x.CardNo == loan.CardNo);
                if(reservation != null)
                {

                    var reserveDeleteSQL = @"DELETE FROM dbo.Reservation WHERE CardNo = {0} AND ISBN = {1}";
                    db.Database.ExecuteSqlCommand(reserveDeleteSQL, loan.CardNo, loanBook.ISBN);
                }

                db.Loans.Add(loan);
                db.SaveChanges();

                TempData["Success"] = "Success: The book has been loaned.";
                return RedirectToAction("Books", "Home");
            }

            return View();
        }

        //Reserve Confirmation Page
        public ActionResult ReserveConfirmation(string id)
        {
            BookDetailsView book = db.BookDetailsViews.Find(id);
            

            if (book == null)
            {
                return HttpNotFound();
            }

            //Check if member already reserved the same title
            var reservedAlready = (from r in db.Reservations
                                  where r.CardNo == Globals.currentID && r.ISBN == book.ISBN
                                  select r).ToList();

            if(reservedAlready.Count > 0)
            {
                return View("DuplicateReserveError");
            }

            return View(book);
        }

        [HttpPost]

        public ActionResult ReserveConfirmation([Bind(Include = "ReservationID,CardNo,ISBN,ReserveDate")] Reservation reservation)
        {
            Random rand = new Random();
            int rID = rand.Next(0, 10000);

            if(ModelState.IsValid)
            {
                reservation.ReservationID = rID;
                DateTime today = DateTime.Now.Date;

                reservation.ReserveDate = today;

                reservation.ISBN = ISBN;
                reservation.CardNo = Globals.currentID;

                db.Reservations.Add(reservation);
                db.SaveChanges();

                TempData["Success"] = "The book has been reserved.";
                return RedirectToAction("Books", "Home");
            }

            return View();
        }            
        // GET: Books
        public ActionResult BookIndex()
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
        public ActionResult AddBook()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.Genre = new SelectList(db.Genres, "GenreID", "Genre1");
                ViewBag.Language = new SelectList(db.Languages, "LanguageID", "Language1");
                return View();
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBook([Bind(Include = "ISBN,Author_FName,Author_LName,Publisher,NumOfPages,Title,Year,Genre,Language,Rating,Synopsis,Shelf")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();

                TempData["Success"] = "Success: The book has been added to the catalog.";
                return RedirectToAction("BookIndex");
            }

            ViewBag.Genre = new SelectList(db.Genres, "GenreID", "Genre1", book.Genre);
            ViewBag.Language = new SelectList(db.Languages, "LanguageID", "Language1", book.Language);
            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult EditBook(string id)
        {
            if (User.IsInRole("Admin"))
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
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBook([Bind(Include = "ISBN,Author_FName,Author_LName,Publisher,NumOfPages,Title,Year,Genre,Language,Rating,Synopsis,Shelf")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Success"] = "Success: The book details are changed.";
                return RedirectToAction("BookIndex");
            }
            ViewBag.Genre = new SelectList(db.Genres, "GenreID", "Genre1", book.Genre);
            ViewBag.Language = new SelectList(db.Languages, "LanguageID", "Language1", book.Language);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult DeleteBook(string id)
        {
            if (User.IsInRole("Admin"))
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
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("DeleteBook")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();

            TempData["Success"] = "Success: The book has been removed from the catalog.";
            return RedirectToAction("BookIndex");
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
