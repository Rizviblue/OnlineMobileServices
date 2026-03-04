using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Models;

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

        // GET: Create Feedback
        public IActionResult Create()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            // Pre-fill user info
            ViewBag.UserName = HttpContext.Session.GetString("Username");
            ViewBag.UserMobile = HttpContext.Session.GetString("UserMobile");
            return View(new Feedback());
        }

        // POST: Submit Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            try
            {
                if (string.IsNullOrWhiteSpace(feedback.Name) ||
                    string.IsNullOrWhiteSpace(feedback.MobileNumber) ||
                    string.IsNullOrWhiteSpace(feedback.Message))
                {
                    TempData["Error"] = "Please fill in all fields.";
                    ViewBag.UserName = HttpContext.Session.GetString("Username");
                    ViewBag.UserMobile = HttpContext.Session.GetString("UserMobile");
                    return View(feedback);
                }

                feedback.FeedbackDate = DateTime.Now;
                _context.Feedback.Add(feedback);
                await _context.SaveChangesAsync();

                return RedirectToAction("ThankYou");
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred submitting feedback. Please try again.";
                return View(feedback);
            }
        }

        // Thank You page
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}