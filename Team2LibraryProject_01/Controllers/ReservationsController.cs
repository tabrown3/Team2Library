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

            switch (reservation.ISBN)
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

            var bookLoaned = db.Inventories.Where(x => x.ISBN == reservation.Book.ISBN).FirstOrDefault();

            if (bookLoaned == null)
            {
                ViewBag.Price = "Unavailable";
            }
            else
            {
                ViewBag.Price = bookLoaned.ItemPrice;
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
