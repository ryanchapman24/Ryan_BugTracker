using Microsoft.AspNet.Identity;
using Ryan_BugTracker.Models;
using Ryan_BugTracker.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Ryan_BugTracker.Controllers
{
    //[RequireHttps]
    public class AdminController : UserNames
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Clients/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult CreateClient()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClient([Bind(Include = "Id,Name,IsActive")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Clients.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult EditClient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClient([Bind(Include = "Id,Name,IsActive")] Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteClient(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("DeleteClient")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteClientConfirmed(int id)
        {
            Client client = db.Clients.Find(id);
            db.Clients.Remove(client);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Clients/Details/5
        [Authorize(Roles = "Administrator")]
        public ActionResult ClientDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Admin/Index
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            List<AdminUserListModels> users = new List<AdminUserListModels>();
            UserRolesHelper helper = new UserRolesHelper(db);
            foreach (var user in db.Users.ToList())
            {
                var eachUser = new AdminUserListModels();
                eachUser.roles = new List<string>();
                eachUser.user = user;
                eachUser.roles = helper.ListUserRoles(user.Id).ToList();

                users.Add(eachUser);
            }
            ViewBag.Clients = db.Clients.OrderBy(c => c.Name).ToList();
            return View(users.OrderBy(u => u.user.LastName).ToList());
        }

        // Get: Admin/EditUserRoles
        [Authorize(Roles = "Administrator")]
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            UserRolesHelper helper = new UserRolesHelper(db);
            var model = new AdminUserViewModels();
            model.Name = user.DisplayName;
            model.Id = user.Id;
            model.SelectedRoles = helper.ListUserRoles(id).ToArray();
            model.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);

            return View(model);
        }

        // Post: Admin/EditUserRoles
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult EditUserRoles(AdminUserViewModels model)
        {
            var user = db.Users.Find(model.Id);
            UserRolesHelper helper = new UserRolesHelper(db);

            foreach (var role in db.Roles.Select(r => r.Name).ToList())
            {
                helper.RemoveUserFromRole(user.Id, role);
            }

            if (model.SelectedRoles != null)
            {
                foreach (var role in model.SelectedRoles)
                {
                    helper.AddUserToRole(user.Id, role);
                }

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        //// GET: Admin/Delete/5
        //[Authorize(Roles = "Administrator")]
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationUser user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //// POST: Admin/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    ApplicationUser user = db.Users.Find(id);
        //    db.Users.Remove(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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