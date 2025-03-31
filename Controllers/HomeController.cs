using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using YorkScjool.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks; // For ApplicationUser and ApplicationUserManager


namespace YorkScjool.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationUserManager _userManager;

        public HomeController()
        {
            _userManager = System.Web.HttpContext.Current?.GetOwinContext()?.GetUserManager<ApplicationUserManager>();
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Landing");
                }

                var currentUser = _userManager?.FindById(userId);
                if (currentUser == null)
                {
                    return RedirectToAction("Landing"); // Prevent crash if user is not found
                }

                bool isAdmin = _userManager.IsInRole(currentUser.Id, "Admin");

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (User.IsInRole("Student"))
                {
                    return RedirectToAction("StudentDashboard", "Home");
                }
            }

            return View("~/Views/Shared/Landing.cshtml");
        }


        [AllowAnonymous]
        public ActionResult Landing()
        {
            return View();
        }

        //public ActionResult StudentDashboard()
        //{
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    return View("~/Views/Shared/StudentDashboard.cshtml"); // Load from Shared folder
        //}

        public ActionResult StudentDashboard()
        {
            var userId = User.Identity.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.ErrorMessage = "User ID is null. Please log in again.";
                return View();
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager?.FindById(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found in the database!";
                return View();
            }

            bool isAdmin = userManager.IsInRole(user.Id, "Admin");

            ViewBag.IsAdmin = isAdmin;
            return View(user);
        }


        //private readonly ApplicationUserManager _userManager;

        //public HomeController()
        //{
        //    _userManager = System.Web.HttpContext.Current?.GetOwinContext()?.GetUserManager<ApplicationUserManager>();
        //}

        //public ActionResult Index()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var userId = User.Identity.GetUserId(); // Get the user ID from the authenticated user
        //        var currentUser = _userManager.FindById(userId); // Use the ApplicationUserManager to find the user by ID
        //        bool isAdmin = _userManager != null && _userManager.IsInRole(currentUser.Id, "Admin");


        //        // If the user is found, pass their ProfilePicture to the view
        //        if (currentUser != null)
        //        {
        //            ViewBag.ProfilePicture = currentUser.ProfilePicture;
        //        }

        //        if (User.IsInRole("Admin"))
        //        {
        //            return RedirectToAction("Dashboard", "Admin");
        //        }
        //        else if (User.IsInRole("Student"))
        //        {
        //            return RedirectToAction("StudentDashboard", "Home");
        //        }
        //    }

        //    return View("~/Views/Shared/Landing.cshtml");
        //}


    }
}
