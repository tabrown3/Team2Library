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
                var loans = db.Loans.Include(l => l.Inventory).Include(l => l.Member).Where(x => x.ReturnDate == null);

                ViewBag.ItemID = new SelectList(db.Loans.Where(x => x.ReturnDate == null), "ItemID", "ItemID");

                return View(loans.ToList());
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
                loan.ReturnDate = today;
                loan.Fines = 0;
                loan.FinesPaid = true;

                //Set the book copy to be available on shelf
                var bookUpdateSQL = @"UPDATE dbo.Inventory SET OnShelf = {0} WHERE ItemID = {1}";
                db.Database.ExecuteSqlCommand(bookUpdateSQL, true, loan.ItemID);

                //Update the loan to have a return date
                var loanUpdateSQL = @"UPDATE dbo.Loan SET CardNo = {0}, CheckOutDate = {1}, DueDate = {2}, ReturnDate = {3}, Fines = {4}, FinesPaid = {5}, Title = {6} WHERE LoanID = {7}";
                db.Database.ExecuteSqlCommand(loanUpdateSQL, savedLoan.CardNo, savedLoan.CheckOutDate, savedLoan.DueDate, loan.ReturnDate, loan.Fines, loan.FinesPaid, savedLoan.Title, savedLoan.LoanID);           

                TempData["Success"] = "Success: The book has been returned.";
                return RedirectToAction("Admin", "Home");
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

            switch (loan.Inventory.ISBN)
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

            ViewBag.BookTitle = loan.Inventory.Book.Title;
            return View(loan);
        }

        // GET: Loans/Create
        public ActionResult Create()
        {
            ViewBag.ItemID = new SelectList(db.Inventories, "ItemID", "ISBN");
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
                return RedirectToAction("Index");
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
