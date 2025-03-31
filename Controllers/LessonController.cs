using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using YorkScjool.Models;
using System.Collections.Generic; // ✅ Fix namespace typo

namespace YorkSchool.Controllers
{
    public class LessonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LessonController()
        {
            _context = new ApplicationDbContext(); // ✅ Ensure correct constructor
        }

        // ✅ View all lessons
        public ActionResult Index()
        {
            var lessons = _context.Lessons.ToList();
            return View(lessons);
        }

        // ✅ Show form to add a lesson
        public ActionResult Create()
        {
            ViewBag.Students = new SelectList(_context.Students, "Id", "Name");
            return View();
        }

        // ✅ Save new lesson to database
        [HttpPost]
        public ActionResult Create(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                lesson.DateAdded = DateTime.Now;
                _context.Lessons.Add(lesson);
                _context.SaveChanges();
                return RedirectToAction("ManageLessons");
            }

            ViewBag.Students = new SelectList(_context.Students, "Id", "Name", lesson.Student_Id);
            return View(lesson);
        }

        // ✅ Edit lesson (GET)
        public ActionResult Edit(int id)
        {
            var lesson = _context.Lessons.Find(id);
            if (lesson == null) return HttpNotFound();

            ViewBag.Students = new SelectList(_context.Students, "Id", "Name", lesson.Student_Id);
            return View(lesson);
        }

        // ✅ Edit lesson (POST)
        [HttpPost]
        public ActionResult Edit(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(lesson).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("ManageLessons");
            }

            ViewBag.Students = new SelectList(_context.Students, "Id", "Name", lesson.Student_Id);
            return View(lesson);
        }

        // ✅ Delete lesson (GET)
        public ActionResult Delete(int id)
        {
            var lesson = _context.Lessons.Find(id);
            if (lesson == null) return HttpNotFound();
            return View(lesson);
        }

        // ✅ Delete lesson (POST)
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var lesson = _context.Lessons.Find(id);
            if (lesson != null)
            {
                _context.Lessons.Remove(lesson);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageLessons");
        }

        // ✅ Display lessons in ManageLessons view
        public ActionResult ManageLessons()
        {
            var lessons = _context.Lessons.ToList();
            return View("~/Views/Admin/ManageLessons.cshtml", lessons); // Explicitly specify Admin folderc
        }

        // GET: Lessons/AddLesson
        public ActionResult AddLesson()
        {
            return View("~/Views/Admin/AddLesson.cshtml"); // Force MVC to load the correct view
        }

        // POST: Lessons/AddLesson
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLesson(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                lesson.DateAdded = DateTime.Now; // Ensure the date is added
                _context.Lessons.Add(lesson);
                _context.SaveChanges();
                return RedirectToAction("ManageLessons"); // ✅ Redirect after saving
            }

            return View(lesson); // If invalid, return to the form with errors
        }

    }
}
