using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YorkScjool.Models;

namespace YorkScjool.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ManageLessons()
        {
            var lessons = db.Lessons.ToList(); // Fetch lessons from the database
            return View("~/Views/Admin/ManageLessons.cshtml", lessons); // Explicitly specify Admin folderc

        }

        public ActionResult EditLesson(int id)
        {
            var lesson = db.Lessons.Find(id);
            if (lesson == null) return HttpNotFound();
            return View(lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLesson(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                // Ensure DateAdded is valid
                if (lesson.DateAdded == DateTime.MinValue)
                {
                    lesson.DateAdded = DateTime.Now;
                }

                db.Entry(lesson).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageLessons");
            }
            return View(lesson);
        }

        public ActionResult DeleteLesson(int id)
        {
            var lesson = db.Lessons.Find(id);
            if (lesson == null) return HttpNotFound();
            return View(lesson);
        }

        [HttpPost, ActionName("DeleteLesson")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDeleteLesson(int id)
        {
            var lesson = db.Lessons.Find(id);
            if (lesson == null) return HttpNotFound();

            db.Lessons.Remove(lesson);
            db.SaveChanges();
            return RedirectToAction("ManageLessons");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLesson(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("ManageLessons");
            }

            return View("ManageLessons", db.Lessons.ToList());
        }

        public ActionResult AddLesson()
        {
            var studentRole = db.Roles.FirstOrDefault(r => r.Name == "Student");
            var students = db.Users.Where(u => u.Roles.Any(r => r.RoleId == studentRole.Id)).ToList();

            ViewBag.Students = new SelectList(students, "Id", "UserName"); // ✅ Corrected!
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLesson(Lesson model, string studentId)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(studentId))
                {
                    ModelState.AddModelError("", "Please select a student.");
                    return View(model);
                }

                model.Student_Id = studentId;
                model.DateAdded = DateTime.Now;

                db.Lessons.Add(model);
                db.SaveChanges();

                return RedirectToAction("ManageLessons");
            }

            ViewBag.Students = new SelectList(db.Users, "Id", "UserName"); // Reload dropdown
            return View(model);
        }

        public ActionResult ManageCourses()
        {
            return View();
        }

        // Fetch only students and pass them to the view
        public ActionResult ManageStudents()
        {
            var studentRole = roleManager.FindByName("Student");
            if (studentRole == null)
            {
                ViewBag.Message = "No students found.";
                return View(new List<ApplicationUser>());
            }

            var students = userManager.Users
                .Where(u => userManager.IsInRole(u.Id, "Student"))
                .ToList();


            return View(students);
        }

        public ActionResult RegisterUser()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(RegisterViewModel model)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (userManager.FindByName(model.Email) != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = userManager.Create(user, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.Role))
                {
                    if (!roleManager.RoleExists(model.Role))
                    {
                        roleManager.Create(new IdentityRole(model.Role));
                    }

                    userManager.AddToRole(user.Id, model.Role);
                }

                return RedirectToAction("ManageUsers");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }


        public ActionResult ManageUsers()
        {
            var users = userManager.Users.ToList(); // Fetch users from the database
            return View(users);
        }

        // Edit User
        public ActionResult EditUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("ManageUsers");

            var user = userManager.FindById(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(ApplicationUser model, string role)
        {
            var user = userManager.FindById(model.Id);
            if (user == null) return HttpNotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;

            // Role Update
            var oldRoles = userManager.GetRoles(user.Id);
            userManager.RemoveFromRoles(user.Id, oldRoles.ToArray());

            if (!string.IsNullOrEmpty(role))
            {
                userManager.AddToRole(user.Id, role);
            }

            var result = userManager.Update(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(model);
        }


        // Delete User
        public ActionResult DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("ManageUsers");

            var user = userManager.FindById(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDeleteUser(string id)
        {
            var user = userManager.FindById(id);
            if (user == null) return HttpNotFound();

            // Ensure there are no related records before deleting
            var logins = user.Logins.ToList();
            foreach (var login in logins)
            {
                userManager.RemoveLogin(user.Id, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
            }

            userManager.RemoveFromRoles(user.Id, userManager.GetRoles(user.Id).ToArray());
            userManager.Delete(user);

            return RedirectToAction("ManageUsers");
        }

    }
}
