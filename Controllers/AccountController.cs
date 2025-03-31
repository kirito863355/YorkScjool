using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using YorkScjool.Models;

namespace YorkScjool.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        // Dependency Injection Constructor
        public AccountController(
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public AccountController()
        {
            _userManager = System.Web.HttpContext.Current?.GetOwinContext()?.GetUserManager<ApplicationUserManager>();
            _signInManager = System.Web.HttpContext.Current?.GetOwinContext()?.Get<ApplicationSignInManager>();
            _roleManager = System.Web.HttpContext.Current?.GetOwinContext()?.Get<RoleManager<IdentityRole>>();
            _context = new ApplicationDbContext();
        }

        public async Task<ActionResult> StudentDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isAdmin = user != null && await _userManager.IsInRoleAsync(user, "Admin");

            ViewBag.Title = "Student Dashboard";
            // Your other logic here

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> UpdateProfilePicture(HttpPostedFileBase file)
        //{
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        var userId = User.Identity.GetUserId();
        //        var user = await _userManager.FindByIdAsync(userId);
        //        if (user != null)
        //        {
        //            // Ensure the UserImages folder exists
        //            string folderPath = Server.MapPath("~/UserImages/");
        //            if (!Directory.Exists(folderPath))
        //            {
        //                Directory.CreateDirectory(folderPath);
        //            }

        //            // Save the file with a unique name (userId + extension)
        //            string fileName = userId + Path.GetExtension(file.FileName);
        //            string filePath = Path.Combine(folderPath, fileName);
        //            file.SaveAs(filePath);

        //            // Update the user's profile picture
        //            user.ProfilePicture = "/UserImages/" + fileName;
        //            await _userManager.UpdateAsync(user);
        //        }
        //    }
        //    return RedirectToAction("StudentDashboard", "Home");
        //}


        // LOGIN
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        if (await _userManager.IsInRoleAsync(user.Id, "Admin"))
                            return RedirectToAction("Dashboard", "Admin");
                        if (await _userManager.IsInRoleAsync(user.Id, "Student"))
                            return RedirectToAction("StudentDashboard", "Home");
                    }
                    return RedirectToAction("Index", "Home"); // Default fallback

                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        // REGISTER STUDENTS (Admins Only)
        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Ensure role "Student" exists
                if (!_roleManager.RoleExists("Student"))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole("Student"));
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", "Failed to create Student role.");
                        return View(model);
                    }
                }

                // Assign role
                await _userManager.AddToRoleAsync(user.Id, "Student");
                await _context.SaveChangesAsync(); // Ensure changes are saved

                System.Diagnostics.Debug.WriteLine($"User Registered: {user.Email}");
                return RedirectToAction("ManageStudents", "Admin");
            }

            // Log errors if registration fails
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
                System.Diagnostics.Debug.WriteLine($"Error: {error}");
            }

            return View(model);
        }

        // MANAGE STUDENTS (Admins Only)
        [Authorize(Roles = "Admin")]
        public ActionResult ManageStudents()
        {
            var students = _userManager.Users
                .Where(u => u.Roles.Any(r => r.RoleId == _context.Roles.FirstOrDefault(x => x.Name == "Student").Id))
                .ToList();

            System.Diagnostics.Debug.WriteLine($"Total students: {students.Count}");

            return View(students);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            var student = _context.Users.Find(id);
            if (student == null) return HttpNotFound();

            var model = new EditStudentViewModel
            {
                Id = student.Id,
                Email = student.Email,
                UserName = student.UserName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditStudentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var student = _context.Users.Find(model.Id);
            if (student == null) return HttpNotFound();

            student.Email = model.Email;
            student.UserName = model.UserName;

            _context.SaveChanges();
            return RedirectToAction("ManageStudents");
        }

        // DELETE STUDENT (Admins Only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteStudent(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("ManageStudents");

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("ManageStudents");

                AddErrors(result);
            }

            return RedirectToAction("ManageStudents");
        }

        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            // Sign the user out using ASP.NET Identity
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            // Redirect to the login page or landing page after logging out
            return RedirectToAction("Login", "Account"); // Change "Login" to the correct action if needed
        }

        // HELPER METHODS
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
