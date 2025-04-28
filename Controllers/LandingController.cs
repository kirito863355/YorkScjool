using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YorkScjool.Models;

namespace YorkScjool.Controllers
{
    public class LandingController : Controller
    {
        // GET: Landing
        public ActionResult Index()
        {
            return View("Landing");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(TrialLessonRequest model)
        {
            if (ModelState.IsValid)
            {
                var body = $@"
                    <p><strong>Ім’я😒:</strong> {model.Name}</p>
                    <p><strong>Телефон:</strong> {model.PhoneCode} {model.Phone}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>requests:</strong> {model.Messenger}</p>
                    <p><strong>Для кого:</strong> {model.Recipient}</p>";

                var message = new MailMessage();
                message.To.Add(new MailAddress("dimascherbak7@gmail.com")); // 💌 замените на свою почту
                message.Subject = "Новая заявка на пробный урок";
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                //ViewBag.Message = "Форма успешно отправлена!";

                TempData["Success"] = ViewBag.Message = "Форма успешно отправлена!";
                //TempData["Success"] = true;

                //return View("Landing");
                return Redirect("/Landing#formatic");

            }

            return View(model);
        }
    }
}