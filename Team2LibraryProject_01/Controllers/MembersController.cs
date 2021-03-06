﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Team2LibraryProject_01.Models;

namespace Team2LibraryProject_01.Controllers
{
    public class MembersController : Controller
    {
        private Team2LibraryEntities db = new Team2LibraryEntities();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public MembersController()
        {

        }
        public MembersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        // GET: /Default1/
        public ActionResult MemberIndex()
        {
            var members = db.Members.Include(m => m.Role);
            return View(members.ToList());
        }

        // GET: /Default1/Details/5
        public ActionResult MemberDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        // GET: /Default1/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Role1");
            return View();
        }

        // POST: /Default1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="CardNo,RoleID,FName,LName,Email,Password")] Member member)
        {
            if (ModelState.IsValid)
            {
                db.Members.Add(member);
                db.SaveChanges();
                return RedirectToAction("MemberIndex");
            }

            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Role1", member.RoleID);
            return View(member);
        }      

        // GET: /Default1/Delete/5
        public ActionResult DeleteMember(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }


        // POST: /Default1/Delete/5
        [HttpPost, ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Member member = db.Members.Find(id);

            //Check if the inventory item is currently on loan
            var loan = (from l in db.Loans
                        where l.CardNo == member.CardNo
                        select l).ToList();

            if (loan.Count > 0)
            {
                TempData["Success"] = "Error: This member is currently loaning a book. Cannot delete identity.";
                return RedirectToAction("MemberIndex");
            }


            //Remove member from Library database
            db.Members.Remove(member);
            db.SaveChanges();

            //Remove member from ASP.NET account database
            var user = await UserManager.FindByEmailAsync(member.Email);
            var deleteRole = UserManager.RemoveFromRole(user.Id, "Student");
            var deleteMember = await UserManager.DeleteAsync(user);

            TempData["Success"] = "Success: The member has been deleted.";
            return RedirectToAction("MemberIndex");
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
