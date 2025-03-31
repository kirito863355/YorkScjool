using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Web.Services.Description;
using YorkScjool.Models;

[assembly: OwinStartup(typeof(YorkScjool.Startup))]

namespace YorkScjool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndAdminUser();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout")
            });


            // ✅ Register dependencies for UserManager, SignInManager, and RoleManager
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.CreatePerOwinContext<RoleManager<IdentityRole>>((options, context) =>
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>())));
        }



        private void CreateRolesAndAdminUser()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                // Create Admin Role
                if (!roleManager.RoleExists("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    roleManager.Create(role);
                }

                // Create Student Role
                if (!roleManager.RoleExists("Student"))
                {
                    var role = new IdentityRole("Student");
                    roleManager.Create(role);
                }

                // Create an Admin user if it doesn't exist
                var adminEmail = "admin@example.com";
                var adminPassword = "Admin@123";

                var adminUser = userManager.FindByEmail(adminEmail);
                if (adminUser == null)
                {
                    var newAdmin = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                    var createUserResult = userManager.Create(newAdmin, adminPassword);

                    if (createUserResult.Succeeded)
                    {
                        userManager.AddToRole(newAdmin.Id, "Admin");
                    }
                }
            }
        }
    }
}
