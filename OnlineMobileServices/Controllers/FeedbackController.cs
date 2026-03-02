using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Models;
using Microsoft.AspNetCore.Http;

namespace OnlineMobileServices.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("UserId") != null;
        }

        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Feedback feedback)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            feedback.FeedbackDate = DateTime.Now;

            _context.Feedback.Add(feedback);
            _context.SaveChanges();

            ViewBag.Message = "Feedback submitted successfully!";
            return View();
        }
    }
}