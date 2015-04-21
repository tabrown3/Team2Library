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
    public class ReservationsController : Controller
    {
        private Team2LibraryEntities db = new Team2LibraryEntities();

        // GET: /Reservations/
        public ActionResult Index()
        {
            var reservations = db.Reservations.Include(r => r.Book).Include(r => r.Member);
            return View(reservations.ToList());
        }

        // GET: /Reservations/Details/5
        public ActionResult ReserveDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }

            ViewBag.Image = "~/Content/Images/Books/" + reservation.ISBN + ".png";

            var bookLoaned = db.Inventories.Where(x => x.ISBN == reservation.Book.ISBN).FirstOrDefault();

            if (bookLoaned == null)
            {
                ViewBag.Price = -1;
            }
            else
            {
                ViewBag.Price = bookLoaned.ItemPrice;
            }

            var availableBook = db.Inventories.Where(x => x.ISBN == reservation.Book.ISBN && x.OnShelf == true).ToList();

            if(availableBook.Count > 0)
            {
                ViewBag.Alert = "Good news! Your reserved title is in stock. Check out this title quickly to get it.";
            }

            ViewBag.BookTitle = reservation.Book.Title;
            return View(reservation);
        }

        // GET: /Reservations/Create
        public ActionResult Create()
        {
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName");
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName");
            return View();
        }

        // POST: /Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ReservationID,CardNo,ISBN,ReserveDate")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName", reservation.ISBN);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", reservation.CardNo);
            return View(reservation);
        }

        // GET: /Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName", reservation.ISBN);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", reservation.CardNo);
            return View(reservation);
        }

        // POST: /Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ReservationID,CardNo,ISBN,ReserveDate")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Author_FName", reservation.ISBN);
            ViewBag.CardNo = new SelectList(db.Members, "CardNo", "FName", reservation.CardNo);
            return View(reservation);
        }

        // GET: /Reservations/Delete/5
        public ActionResult DeleteReservation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: /Reservations/Delete/5
        [HttpPost, ActionName("DeleteReservation")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            db.SaveChanges();

            TempData["Success"] = "The reservation has been deleted.";
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
