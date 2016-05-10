using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ryan_BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace Ryan_BugTracker.Controllers
{
    [RequireHttps]
    public class TicketsController : UserNames
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.AuthorUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize]
        public ActionResult Create(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var project = db.Projects.Find(id);

            if (project == null)
            {
                return HttpNotFound();
            }


            var user = db.Users.Find(User.Identity.GetUserId());

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectName = project.Title;
            Ticket ticket = new Ticket();
            ticket.ProjectId = project.Id;
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(IEnumerable<HttpPostedFileBase> files, [Bind(Include = "Id,Title,Body,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
               
                ticket.AuthorUserId = User.Identity.GetUserId();
                ticket.Created = System.DateTime.Now;
                ticket.TicketStatusId = 1;
                db.Tickets.Add(ticket);
                db.SaveChanges();

                TicketAttachment attachment = new TicketAttachment();

                var path = Server.MapPath("~/TicketAttachments/" + ticket.Id);
                Directory.CreateDirectory(path);

                

                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        attachment.TicketId = ticket.Id;

                        file.SaveAs(Path.Combine(Server.MapPath("~/TicketAttachments/" + attachment.TicketId), Path.GetFileName(file.FileName)));
                        attachment.FileUrl = file.FileName;
                        
                        attachment.AuthorUserId = User.Identity.GetUserId();
                        attachment.Created = System.DateTime.Now;
                        db.TicketAttachments.Add(attachment);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Body,AuthorUserId,AssignedToUserId,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Updated = System.DateTime.Now;
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Title").IsModified = true;
                db.Entry(ticket).Property("Body").IsModified = true;
                db.Entry(ticket).Property("Updated").IsModified = true;
                db.Entry(ticket).Property("AssignedToUserId").IsModified = true;
                db.Entry(ticket).Property("ProjectId").IsModified = true;
                db.Entry(ticket).Property("TicketTypeId").IsModified = true;
                db.Entry(ticket).Property("TicketPriorityId").IsModified = true;
                db.Entry(ticket).Property("TicketStatusId").IsModified = true;               

                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Tickets/EditProjectAssignments
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult EditTicketAssignments(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            ApplicationDbContext context = new ApplicationDbContext();

            Project project = db.Projects.Find(ticket.ProjectId);
            var projectUsers = context.Users.Where(u => u.Projects.Any(p => p.Title == project.Title));

            var role = context.Roles.SingleOrDefault(u => u.Name == "Developer");
            var usersInRole = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));

            var displayUsers = usersInRole.Where(u => u.Projects.Any(p => (p.Title == project.Title)));

            ViewBag.AssignedToUserId = new SelectList(displayUsers, "Id", "DisplayName", ticket.AssignedToUserId);

            return View(ticket);
        }

        // Post: Tickets/EditProjectAssignments
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult EditTicketAssignments([Bind(Include = "Id,Created,Updated,Title,Body,AuthorUserId,AssignedToUserId,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")]Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Updated = System.DateTime.Now;
                ticket.TicketStatusId = 2;
                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property("Updated").IsModified = true;
                db.Entry(ticket).Property("AssignedToUserId").IsModified = true;
                db.Entry(ticket).Property("TicketStatusId").IsModified = true;

                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateComment(TicketComment comment)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(comment.TicketId);
                ticket.Updated = System.DateTime.Now;               
                db.Entry(ticket).Property("Updated").IsModified = true;

                comment.AuthorUserId = User.Identity.GetUserId();
                comment.Created = System.DateTime.Now;
                db.TicketComments.Add(comment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
        }

        // GET: Tickets/EditComment/5
        [Authorize(Roles = "Administrator")]
        public ActionResult EditComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = db.TicketComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Tickets/EditComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult EditComment(TicketComment comment, int ticketid)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(comment.TicketId);
                ticket.Updated = System.DateTime.Now;
                db.Entry(ticket).Property("Updated").IsModified = true;

                db.TicketComments.Attach(comment);
                db.Entry(comment).Property("Body").IsModified = true;
                db.Entry(comment).Property("Updated").IsModified = true;
                comment.Updated = System.DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticketid });
            }
            return View(comment);
        }

        // GET: Tickets/DeleteComment/5
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = db.TicketComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Tickets/DeleteComment/5
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteCommentConfirmed(int? id, int ticketid)
        {
            var ticket = db.Tickets.Find(ticketid);
            ticket.Updated = System.DateTime.Now;
            db.Entry(ticket).Property("Updated").IsModified = true;

            TicketComment comment = db.TicketComments.Find(id);
            db.TicketComments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketid });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateAttachment(IEnumerable<HttpPostedFileBase> files, TicketAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(attachment.TicketId);
                ticket.Updated = System.DateTime.Now;
                db.Entry(ticket).Property("Updated").IsModified = true;

                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        file.SaveAs(Path.Combine(Server.MapPath("~/TicketAttachments/" + attachment.TicketId), Path.GetFileName(file.FileName)));
                        attachment.FileUrl = file.FileName;

                        attachment.AuthorUserId = User.Identity.GetUserId();
                        attachment.Created = System.DateTime.Now;                       
                        db.TicketAttachments.Add(attachment);
                        db.SaveChanges();
                    }
                }               
            }
            return RedirectToAction("Details", "Tickets", new { id = attachment.TicketId });
        }

        // GET: Tickets/DeleteAttachment/5
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteAttachment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment attachment = db.TicketAttachments.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        // POST: Tickets/DeleteAttachment/5
        [HttpPost, ActionName("DeleteAttachment")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteAttachmentConfirmed(int? id, int ticketid)
        {
            var ticket = db.Tickets.Find(ticketid);
            ticket.Updated = System.DateTime.Now;
            db.Entry(ticket).Property("Updated").IsModified = true;

            TicketAttachment attachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(attachment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketid });
        }
    }
}
