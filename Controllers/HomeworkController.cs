using System.Linq;
using System.Web.Mvc;
using System.Data.Entity; // ✅ Required for EntityState
using YorkScjool.Models;

public class HomeworkController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeworkController()
    {
        _context = new ApplicationDbContext(); // ✅ Fix constructor issue
    }

    // ✅ View all homework
    public ActionResult Index()
    {
        var homework = _context.Homework.ToList();
        return View(homework);
    }

    // ✅ Show form to add homework
    public ActionResult Create()
    {
        return View();
    }

    // ✅ Save new homework to database
    [HttpPost]
    public ActionResult Create(Homework homework)
    {
        if (ModelState.IsValid)
        {
            _context.Homework.Add(homework);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(homework);
    }

    // ✅ Edit homework (GET)
    public ActionResult Edit(int id)
    {
        var homework = _context.Homework.Find(id);
        if (homework == null) return HttpNotFound();
        return View(homework);
    }

    // ✅ Edit homework (POST)
    [HttpPost]
    public ActionResult Edit(Homework homework)
    {
        if (ModelState.IsValid)
        {
            _context.Entry(homework).State = EntityState.Modified; // ✅ Fix Update error
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(homework);
    }

    // ✅ Delete homework (GET)
    public ActionResult Delete(int id)
    {
        var homework = _context.Homework.Find(id);
        if (homework == null) return HttpNotFound();
        return View(homework);
    }

    // ✅ Delete homework (POST)
    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
        var homework = _context.Homework.Find(id);
        if (homework != null)
        {
            _context.Homework.Remove(homework);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
