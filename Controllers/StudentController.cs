using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YorkScjool.Models;

namespace YorkScjool.Controllers
{
    [Authorize(Roles = "Student,Admin")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController()
        {
            _context = new ApplicationDbContext();
        }

        // ✅ STUDENT DASHBOARD
        public ActionResult StudentDashboard()
        {
            return View("~/Views/Shared/StudentDashboard.cshtml");
        }

        // ✅ FETCH LESSONS ONLY FOR LOGGED-IN STUDENT
        public ActionResult Lessons()
        {
            string studentId = User.Identity.GetUserId();
            var student = _context.Students.FirstOrDefault(s => s.UserId == studentId);
            if (student == null) return PartialView(new List<Lesson>());

            var lessons = _context.Lessons.Where(l => l.Student_Id == student.Id.ToString()).ToList();
            return PartialView(lessons);
        }

        // ✅ OTHER PAGES (PLACEHOLDERS)
        public ActionResult HomeWork() => PartialView();
        public ActionResult HomeWork2() => PartialView();
        public ActionResult HomeWork3() => PartialView();
        public ActionResult Subject() => PartialView();
        public ActionResult Dictionary() => PartialView();
        public ActionResult Chat() => PartialView();

        // ✅ ACCOUNT PAGE - SHOW STUDENT DETAILS
        public ActionResult Account()
        {
            string userId = User.Identity.GetUserId();
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);

            if (student == null) return RedirectToAction("Settings");

            return View(student);
        }

        // ✅ SETTINGS PAGE - EDIT STUDENT PROFILE
        public ActionResult Settings()
        {
            string userId = User.Identity.GetUserId();
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);

            if (student == null)
            {
                student = new Student
                {
                    UserId = userId,
                    Email = User.Identity.Name ?? "unknown@example.com",
                    FullName = "Default Name",
                    Mobile = "+0000000000",
                    Country = "Unknown",
                    ProfilePicture = "/Content/student/default.png",
                    Telegram = "",
                    Instagram = "",
                    Facebook = "",
                    Snapchat = ""
                };

                try
                {
                    _context.Students.Add(student);
                    _context.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    return Content("Validation error! Check Debug Output for details.");
                }
            }

            return View(student);
        }

        // ✅ UPDATE STUDENT INFO (PROFILE IMAGE & SOCIAL MEDIA)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Student model, HttpPostedFileBase file)
        {
            System.Diagnostics.Debug.WriteLine("Update() method called!");

            string userId = User.Identity.GetUserId();
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);

            if (student == null)
            {
                System.Diagnostics.Debug.WriteLine("Student not found!");
                return HttpNotFound();
            }

            // Update basic details
            student.FullName = model.FullName;
            student.Email = model.Email;
            student.Mobile = model.Mobile;
            student.Country = model.Country;
            student.Telegram = model.Telegram;
            student.Instagram = model.Instagram;
            student.Facebook = model.Facebook;
            student.Snapchat = model.Snapchat;

            // ✅ Handle profile picture upload
            if (file != null && file.ContentLength > 0)
            {
                // Ensure the directory exists
                string path = Server.MapPath("~/Content/student/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);  // Create directory if it doesn't exist
                }

                // Save the file with the correct path
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(path, fileName);
                file.SaveAs(filePath); // Save file to server

                // Update student's profile picture URL
                student.ProfilePicture = "/Content/student/" + fileName;
            }


            _context.SaveChanges();
            System.Diagnostics.Debug.WriteLine("Profile picture updated successfully!");

            return RedirectToAction("Account");
        }
    }
}
