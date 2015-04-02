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
    public class DVDsController : Controller
    {
        static int bookID = 0;          //What am I doingggggggg
        static int dvdID = 0;

        private T2_LibraryEntities db = new T2_LibraryEntities();

        //DVDs in Catalog
        public ActionResult IronMan3()
        {
            DVD dvd = db.DVDs.Find(1, "Iron Man 3", 2013, 131, "Shane", "Black");
            if (dvd == null)
            {
                return HttpNotFound();
            }

            dvdID = dvd.DvdID;

            return View(dvd);
        }

        public ActionResult Titanic()
        {
            DVD dvd = db.DVDs.Find(2, "Titanic", 1997, 194, "James", "Cameron");
            if (dvd == null)
            {
                return HttpNotFound();
            }

            dvdID = dvd.DvdID;

            return View(dvd);
        }

        public ActionResult TheBigLebowski()
        {
            DVD dvd = db.DVDs.Find(3, "The Big Lebowski", 1998, 119, "Joel", "Coen");
            if (dvd == null)
            {
                return HttpNotFound();
            }

            dvdID = dvd.DvdID;

            return View(dvd);
        }

        public ActionResult TransformersDarkoftheMoon()
        {
            DVD dvd = db.DVDs.Find(4, "Transformers: Dark of the Moon", 2011, 154, "Michael", "Bay");
            if (dvd == null)
            {
                return HttpNotFound();
            }

            dvdID = dvd.DvdID;

            return View(dvd);
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
                return RedirectToAction("DVDs", "Home");
            }
            return View(review);
        }

        // GET: DVDs
        public ActionResult Index()
        {
            return View(db.DVDs.ToList());
        }

        // GET: DVDs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DVD dVD = db.DVDs.Find(id);
            if (dVD == null)
            {
                return HttpNotFound();
            }
            return View(dVD);
        }

        // GET: DVDs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DVDs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DvdID,Title,Year,Length,Director_FName,Director_LName,Studio,Genre,Language,Rating,Synopsis,Shelf")] DVD dVD)
        {
            if (ModelState.IsValid)
            {
                db.DVDs.Add(dVD);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dVD);
        }

        // GET: DVDs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DVD dVD = db.DVDs.Find(id);
            if (dVD == null)
            {
                return HttpNotFound();
            }
            return View(dVD);
        }

        // POST: DVDs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DvdID,Title,Year,Length,Director_FName,Director_LName,Studio,Genre,Language,Rating,Synopsis,Shelf")] DVD dVD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dVD).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dVD);
        }

        // GET: DVDs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DVD dVD = db.DVDs.Find(id);
            if (dVD == null)
            {
                return HttpNotFound();
            }
            return View(dVD);
        }

        // POST: DVDs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DVD dVD = db.DVDs.Find(id);
            db.DVDs.Remove(dVD);
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
