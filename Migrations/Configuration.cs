namespace YorkScjool.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using YorkScjool.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<YorkScjool.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Create a role if it doesn't exist
            if (!roleManager.RoleExists("Student"))
            {
                roleManager.Create(new IdentityRole("Student"));
            }

            // Create a default user
            var user = new ApplicationUser
            {
                UserName = "Vitali",
                Email = "vitalikmarcuk509@gmail.com"
            };

            // Add the user to the database
            if (userManager.FindByEmail(user.Email) == null)
            {
                var result = userManager.Create(user, "pass123");
                if (result.Succeeded)
                {
                    // Assign the "Student" role to the user
                    userManager.AddToRole(user.Id, "Student");
                }
            }

            // Create Admin Role
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            // Create Admin User
            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com"
            };

            if (userManager.FindByEmail(adminUser.Email) == null)
            {
                var result = userManager.Create(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }
        }
    }
}
