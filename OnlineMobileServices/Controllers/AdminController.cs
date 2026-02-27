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
    }
}