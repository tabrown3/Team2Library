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
    public class InventoriesController : Controller
    {
        private Team2LibraryEntities db = new Team2LibraryEntities();

       
        // GET: Inventories
        public ActionResult InventoryIndex()
        {
            if (User.IsInRole("Admin"))
            {
                var inventories = db.Inventories.Include(i => i.Book);
                return View(inventories.ToList());
            }
            else
                return View("~/Views/Home/Index.cshtml");
           
        }

        // GET: Inventories/Details/5
        public ActionResult InventoryItemDetails(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Inventory inventory = db.Inventories.Find(id);
                if (inventory == null)
                {
                    return HttpNotFound();
                }
                return View(inventory);
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // GET: Inventories/Create
        public ActionResult CreateInventoryItem()
        {
            if (User.IsInRole("Admin"))
            {
                ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Title");
                return View();
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // POST: Inventories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInventoryItem([Bind(Include = "ItemID,ItemPrice,DateAdded,ISBN")] Inventory inventory)
        {
            //Insert randomized inventory ID
            Random rand = new Random();
            int inventoryID = rand.Next(0, 5000);

            if (ModelState.IsValid)
            {
                inventory.ItemID = inventoryID;
                db.Inventories.Add(inventory);
                db.SaveChanges();
                return RedirectToAction("InventoryIndex");
            }

            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Title", inventory.ISBN);
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public ActionResult EditInventoryItem(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Inventory inventory = db.Inventories.Find(id);
                if (inventory == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Title", inventory.ISBN);
                return View(inventory);
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // POST: Inventories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInventoryItem([Bind(Include = "ItemID,ItemPrice,DateAdded,ISBN")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("InventoryIndex");
            }
            ViewBag.ISBN = new SelectList(db.Books, "ISBN", "Title", inventory.ISBN);
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public ActionResult DeleteInventoryItem(int? id)
        {
            if (User.IsInRole("Admin"))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Inventory inventory = db.Inventories.Find(id);
                if (inventory == null)
                {
                    return HttpNotFound();
                }
                return View(inventory);
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("DeleteInventoryItem")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventory inventory = db.Inventories.Find(id);
            db.Inventories.Remove(inventory);
            db.SaveChanges();
            return RedirectToAction("InventoryIndex");
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
