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
    public class LoansController : Controller
    {
        private Team2LibraryEntities db = new Team2LibraryEntities();

        // GET: Loans
        public ActionResult LoanIndex()
        {
            if (User.IsInRole("Admin"))
            {
                var loans = db.Loans.Include(l => l.Inventory).Include(l => l.Member).Where(x => !x.ReturnDate.HasValue);

                ViewBag.ItemID = new SelectList(db.Loans.Where(x => x.ReturnDate == null), "ItemID", "ItemID");

                List<Team2LibraryProject_01.Models.Loan> loansList = loans.ToList();

                bool itemChanged = false;

                foreach (Team2LibraryProject_01.Models.Loan item in loansList)
                {

                    if (item.DueDate.CompareTo(DateTime.Today) < 0)
                    {
                        // Old database values
                        float oldFines = item.Fines;
                        bool oldFinesPaid = item.FinesPaid;

                        float newFine = (float)(0.5*((DateTime.Today - item.DueDate).Days));

                        if (newFine <= item.Inventory.ItemPrice)
                        {
                            // New calculated values
                            item.Fines = newFine;
                            item.FinesPaid = false;
                        }
                        else
                        {
                            // New calculated values
                            item.Fines = item.Inventory.ItemPrice;
                            item.FinesPaid = false;
                        }

                        // Was anything actually changed?
                        if((oldFines != item.Fines) || (oldFinesPaid != item.FinesPaid))
                            itemChanged = true;
                    }
                }

                // If something was changed, save it to the DB
                if(itemChanged)
                    db.SaveChanges();

                List<Loan> completedLoan = (from l in db.Loans
                                            where l.ReturnDate.HasValue
                                            select l).ToList();

                ViewBag.CompletedLoan = completedLoan;

                return View(loansList);
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        //Submit Loans
        [HttpPost]
        public ActionResult LoanIndex(Loan loan)
        {
            if (ModelState.IsValid)
            {
                DateTime today = DateTime.Now.Date;

                //Workaround - find the same loan referred to by the ItemID and set those values for update
                var savedLoan = (from l in db.Loans
                                where l.ItemID == loan.ItemID && l.ReturnDate == null
                                select l).FirstOrDefault();

                loan.LoanID = savedLoan.LoanID;
                loan.CardNo = savedLoan.CardNo;
                loan.CheckOutDate = savedLoan.CheckOutDate;
                loan.DueDate = savedLoan.DueDate;
                loan.Title = savedLoan.Title;
                loan.ReturnDate = today.Date;
                loan.Fines = savedLoan.Fines;
                loan.FinesPaid = true;

                //Set the book copy to be available on shelf
                var bookUpdateSQL = @"UPDATE dbo.Inventory SET OnShelf = {0} WHERE ItemID = {1}";
                db.Database.ExecuteSqlCommand(bookUpdateSQL, true, loan.ItemID);

                //Update the loan to have a return date
                var loanUpdateSQL = @"UPDATE dbo.Loan SET CardNo = {0}, CheckOutDate = {1}, DueDate = {2}, ReturnDate = {3}, Fines = {4}, FinesPaid = {5}, Title = {6} WHERE LoanID = {7}";
                db.Database.ExecuteSqlCommand(loanUpdateSQL, savedLoan.CardNo, savedLoan.CheckOutDate, savedLoan.DueDate, loan.ReturnDate, loan.Fines, loan.FinesPaid, savedLoan.Title, savedLoan.LoanID);           

                TempData["Success"] = "Success: The book has been returned.";
                return RedirectToAction("LoanIndex", "Loans");
            }

            ViewBag.ItemID = new SelectList(db.Inventories, "ItemID", "ItemID", loan.ItemID);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", loan.CardNo);
            return View(loan);
        }

        // GET: Loans/Details/5
        public ActionResult LoanDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }

            ViewBag.Image = "~/Content/Images/Books/" + loan.Inventory.ISBN + ".png";

            ViewBag.BookTitle = loan.Inventory.Book.Title;
            return View(loan);
        }

        // GET: Loans/Create
        public ActionResult Create()
        {
            ViewBag.ItemID = new SelectList(db.Inventories, "ItemID", "ItemID");
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoanID,ItemID,CardNo,Title,CheckOutDate,DueDate,ReturnDate,Fines,FinesPaid")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                db.Loans.Add(loan);
                db.SaveChanges();
                return RedirectToAction("Admin", "Home");
            }

            ViewBag.ItemID = new SelectList(db.Inventories, "ItemID", "ISBN", loan.ItemID);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", loan.CardNo);
            return View(loan);
        }

        // GET: Loans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemID = new SelectList(db.Inventories, "ItemID", "ISBN", loan.ItemID);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", loan.CardNo);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoanID,ItemID,CardNo,Title,CheckOutDate,DueDate,ReturnDate,Fines,FinesPaid")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemID = new SelectList(db.Inventories, "ItemID", "ISBN", loan.ItemID);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", loan.CardNo);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Loan loan = db.Loans.Find(id);
            db.Loans.Remove(loan);
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
