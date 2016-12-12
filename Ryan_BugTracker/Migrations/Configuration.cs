namespace Ryan_BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Ryan_BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Ryan_BugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Administrator"))
            {
                roleManager.Create(new IdentityRole { Name = "Administrator" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "your email address"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rchapman@anomalysquared.com",
                    Email = "rchapman@anomalysquared.com",
                    FirstName = "Ryan",
                    LastName = "Chapman",
                    DisplayName = "Ryan Chapman",
                    PhoneNumber = "(919) 698-2849"
                }, "Chappy24!");
            }

            var userId_Super = userManager.FindByEmail("rchapman@anomalysquared.com").Id;
            userManager.AddToRole(userId_Super, "Administrator");
            userManager.AddToRole(userId_Super, "Project Manager");
            userManager.AddToRole(userId_Super, "Developer");
            userManager.AddToRole(userId_Super, "Submitter");

            //if (!context.Users.Any(u => u.Email == "your email address"))
            //{
            //    userManager.Create(new ApplicationUser
            //    {
            //        UserName = "administrator@bugtracker.com",
            //        Email = "administrator@bugtracker.com",
            //        FirstName = "Admin",
            //        LastName = "Istrator",
            //        DisplayName = "Administrator Demo",
            //        PhoneNumber = "(###) ###-####"
            //    }, "Password-1");
            //}

            //var userId_Admin = userManager.FindByEmail("administrator@bugtracker.com").Id;
            //userManager.AddToRole(userId_Admin, "Administrator");

            //if (!context.Users.Any(u => u.Email == "your email address"))
            //{
            //    userManager.Create(new ApplicationUser
            //    {
            //        UserName = "projectmanager@bugtracker.com",
            //        Email = "projectmanager@bugtracker.com",
            //        FirstName = "Project",
            //        LastName = "Manager",
            //        DisplayName = "Project Manager Demo",
            //        PhoneNumber = "(###) ###-####"
            //    }, "Password-1");
            //}

            //var userId_PManager = userManager.FindByEmail("projectmanager@bugtracker.com").Id;
            //userManager.AddToRole(userId_PManager, "Project Manager");

            //if (!context.Users.Any(u => u.Email == "your email address"))
            //{
            //    userManager.Create(new ApplicationUser
            //    {
            //        UserName = "developer@bugtracker.com",
            //        Email = "developer@bugtracker.com",
            //        FirstName = "Devel",
            //        LastName = "Oper",
            //        DisplayName = "Developer Demo",
            //        PhoneNumber = "(###) ###-####"
            //    }, "Password-1");
            //}

            //var userId_Developer = userManager.FindByEmail("developer@bugtracker.com").Id;
            //userManager.AddToRole(userId_Developer, "Developer");

            //if (!context.Users.Any(u => u.Email == "your email address"))
            //{
            //    userManager.Create(new ApplicationUser
            //    {
            //        UserName = "submitter@bugtracker.com",
            //        Email = "submitter@bugtracker.com",
            //        FirstName = "Sub",
            //        LastName = "Mitter",
            //        DisplayName = "Submitter Demo",
            //        PhoneNumber = "(###) ###-####"
            //    }, "Password-1");
            //}

            //var userId_Submitter = userManager.FindByEmail("submitter@bugtracker.com").Id;
            //userManager.AddToRole(userId_Submitter, "Submitter");
        }
    }
}
