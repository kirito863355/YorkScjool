using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YorkScjool.Models;

namespace YorkScjool.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Users()
        {
            var currentUserId = User.Identity.GetUserId();
            var users = db.Users
                .Where(u => u.Id != currentUserId)
                .Select(u => new ChatUser { Id = u.Id, Name = u.UserName })
                .ToList();
            return View(users);
        }

        public ActionResult Index(string withUserId)
        {
            ViewBag.WithUserId = withUserId;
            ViewBag.CurrentUserId = User.Identity.GetUserId();

            if (User.IsInRole("Admin"))
                return View("~/Views/Admin/Chat.cshtml");
            else
                return View("~/Views/Student/Chat.cshtml");
        }

        [HttpPost]
        public ActionResult SaveMessage(string receiverId, string text)
        {
            var senderId = User.Identity.GetUserId();
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Text = text
            };
            db.Messages.Add(message);
            db.SaveChanges();
            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public JsonResult GetMessages(string withUserId)
        {
            var currentUserId = User.Identity.GetUserId();
            var messages = db.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == withUserId) ||
                            (m.SenderId == withUserId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    m.SenderId,
                    m.ReceiverId,
                    m.Text,
                    m.Timestamp
                })
                .ToList();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

    }


}