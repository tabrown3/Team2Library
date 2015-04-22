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

        public ActionResult DeleteReport(string itemID, string isbn, string dateRemoved1, string dateRemoved2)
        {
            var invDeleteLogList = new List<InventoryDeleteLog>();
            List<string> filterList = new List<string>();

            bool searchUsed = false;

            if (!String.IsNullOrEmpty(itemID))
            {
                filterList.Add("ItemID = '" + itemID + "'");
                searchUsed = true;
            }
            if (!String.IsNullOrEmpty(isbn))
            {
                filterList.Add("ISBN = '" + isbn + "'");
                searchUsed = true;
            }
            if (!string.IsNullOrEmpty(dateRemoved1) || !string.IsNullOrEmpty(dateRemoved2))
            {
                if (string.IsNullOrEmpty(dateRemoved2))
                {

                    filterList.Add("DateRemoved = '" + dateRemoved1 + "'");
                }
                else if (string.IsNullOrEmpty(dateRemoved1))
                {

                    filterList.Add("DateRemoved = '" + dateRemoved2 + "'");
                }
                else
                {
                    filterList.Add("(DateRemoved BETWEEN '" + dateRemoved1 + "' AND '" + dateRemoved2 + "')");
                }

                searchUsed = true;
            }

            if (searchUsed == true)
            {

                string whereClause = "WHERE " + string.Join(" AND ", filterList);
                invDeleteLogList = db.Database.SqlQuery<InventoryDeleteLog>("SELECT * FROM dbo.InventoryDeleteLog " + whereClause).ToList();
            }

            if (User.IsInRole("Admin"))
            {
                //return View(db.MemberReviewsViews.ToList());
                return View(invDeleteLogList);
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }

        public ActionResult InventoryReport(string isbn, string title, string authorName, string itemPrice, string dateAdded1, string dateAdded2, string onShelf)
        {
            List<string> onShelfOptions = new List<string>(new string[] { "ALL", "TRUE", "FALSE" });
            var bookInvViewList = new List<BookInventoryView>();
            List<string> filterList = new List<string>();

            ViewBag.onShelf = new SelectList(onShelfOptions);

            bool searchUsed = false;

            if (!String.IsNullOrEmpty(isbn))
            {
                filterList.Add("ISBN = '" + isbn + "'");
                searchUsed = true;
            }
            if (!String.IsNullOrEmpty(title))
            {
                filterList.Add("Title = '" + title + "'");
                searchUsed = true;
            }
            if (!String.IsNullOrEmpty(authorName))
            {
                filterList.Add("Author_LName = " + authorName);
                searchUsed = true;
            }
            if (!string.IsNullOrEmpty(itemPrice))
            {
                filterList.Add("ItemPrice = '" + itemPrice + "'");
                searchUsed = true;
            }
            if (!string.IsNullOrEmpty(dateAdded1) || !string.IsNullOrEmpty(dateAdded2))
            {
                if (string.IsNullOrEmpty(dateAdded2))
                {

                    filterList.Add("DateAdded = '" + dateAdded1 + "'");
                }
                else if (string.IsNullOrEmpty(dateAdded1))
                {

                    filterList.Add("DateAdded = '" + dateAdded2 + "'");
                }
                else
                {
                    filterList.Add("(DateAdded BETWEEN '" + dateAdded1 + "' AND '" + dateAdded2 + "')");
                }

                searchUsed = true;
            }

            if (!string.IsNullOrEmpty(onShelf))
            {
                if (onShelf != "ALL")
                {
                    filterList.Add("OnShelf = '" + onShelf + "'");
                }
                else
                {
                    filterList.Add("(OnShelf = 'TRUE' OR OnShelf = 'FALSE')");
                }

                searchUsed = true;
            }

            if (searchUsed == true)
            {

                string whereClause = "WHERE " + string.Join(" AND ", filterList);
                bookInvViewList = db.Database.SqlQuery<BookInventoryView>("SELECT * FROM dbo.BookInventoryView " + whereClause).ToList();
            }

            if (User.IsInRole("Admin"))
            {
                //return View(db.MemberReviewsViews.ToList());
                return View(bookInvViewList);
            }
            else
                return View("~/Views/Home/Index.cshtml");
        }
       
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
            int inventoryID = rand.Next(0, 10000);

            if (ModelState.IsValid)
            {
                inventory.ItemID = inventoryID;
                inventory.OnShelf = true;
                db.Inventories.Add(inventory);
                db.SaveChanges();

                TempData["Success"] = "Success: The inventory item has been added.";
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

                TempData["Success"] = "Success: The inventory item has been edited.";
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

            //Check if the inventory item is currently on loan
            var loan = (from l in db.Loans
                        where l.ItemID == inventory.ItemID && l.ReturnDate == null
                        select l).ToList();

            if (loan.Count > 0)
            {
                TempData["Success"] = "Error: A member is currently loaning that copy. Unable to delete item.";
                return RedirectToAction("InventoryIndex");
            }

            db.Inventories.Remove(inventory);
            db.SaveChanges();

            TempData["Success"] = "Success: The inventory item has been deleted.";
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
