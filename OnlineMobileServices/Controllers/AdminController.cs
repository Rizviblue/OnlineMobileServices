using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Models;
using System.Linq;

namespace OnlineMobileServices.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalRecharges = _context.Recharges.Count();
            ViewBag.TotalBills = _context.PostPaidBills.Count();
            ViewBag.TotalFeedback = _context.Feedback.Count();

            return View();
        }
        public IActionResult ViewFeedback()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var feedbacks = _context.Feedback
                .OrderByDescending(f => f.FeedbackDate)
                .ToList();

            return View(feedbacks);
        }
        public IActionResult ViewRecharges()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var recharges = _context.Recharges
                .OrderByDescending(r => r.RechargeDate)
                .ToList();

            return View(recharges);
        }
        public IActionResult ViewBills()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");

            var bills = _context.PostPaidBills
                .OrderByDescending(b => b.PaymentDate)
                .ToList();

            return View(bills);
        }
    }
}