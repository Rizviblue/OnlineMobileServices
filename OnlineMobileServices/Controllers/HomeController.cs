using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Models;

namespace OnlineMobileServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Pass some stats for the homepage
            ViewBag.TotalUsers = _context.Users.Count(u => u.Role == "User");
            ViewBag.TotalRecharges = _context.Recharges.Count();
            ViewBag.TotalPlans = _context.RechargePlans.Count();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
