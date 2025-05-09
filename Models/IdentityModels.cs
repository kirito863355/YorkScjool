﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;


namespace YorkScjool.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string ProfilePicture { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) { }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lesson> Lessons { get; set; }  // ✅ Add this
        public DbSet<Homework> Homework { get; set; }  // ✅ If using homework too

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // Ensure these exist
        //public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityUserRole> UserRoles { get; set; } // Fix UserRoles issue
    }

}