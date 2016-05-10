using Microsoft.AspNet.Identity;
using Ryan_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ryan_BugTracker.Controllers
{
    [RequireHttps]
    public class HomeController : UserNames
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Landing()
        {

            return View();
        }

        [Authorize]
        public ActionResult Index(int? id)
        {
            //if (id != null)
            //{
            //    //var project = db.Projects.Find(id);
            //    //var tuser = db.Users.Find(User.Identity.GetUserId());
            //    //var plist = project.Tickets.ToList();
            //    ViewBag.Tickets = db.Tickets.Where(u => u.ProjectId == id).ToList();
            //    var pList = db.Projects.Where(u => u.Id == id).ToList();

            //    return View(pList);
            //}

            var uId = User.Identity.GetUserId();
            ApplicationUser user = new ApplicationUser();
            user = db.Users.Find(uId);

            if (User.IsInRole("Administrator"))
            {
                ViewBag.tickets = db.Tickets.ToList();
            }
            else if(User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            {
                var userTickets = db.Tickets.Where(t => t.AssignedToUserId.Equals(uId) || t.AuthorUserId.Equals(uId)).ToList();
                var projectTickets = user.Projects.SelectMany(p => p.Tickets).ToList();
                ViewBag.tickets = userTickets.Union(projectTickets).ToList();
            }
            else
            { 
                ViewBag.tickets = db.Tickets.Where(t => t.AuthorUserId.Equals(uId)).ToList();
            }

            return View(db.Projects.ToList());
        }

        [Authorize]
        public ActionResult ProfilePage()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            return View(user);
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}