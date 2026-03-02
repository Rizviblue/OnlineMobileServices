using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Models;
using System.Linq;

namespace OnlineMobileServices.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.UserId == int.Parse(userId));

            return View(user);
        }
        public IActionResult Services()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var services = _context.Services.ToList();
            ViewBag.UserId = int.Parse(userId);

            return View(services);
        }

        [HttpPost]
        public IActionResult ActivateService(int serviceId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var userService = new UserService
            {
                UserId = userId,
                ServiceId = serviceId,
                IsActive = true
            };

            _context.UserServices.Add(userService);
            _context.SaveChanges();

            return RedirectToAction("Services");
        }
        public IActionResult EditProfile()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User updatedUser)
        {
            _context.Users.Update(updatedUser);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}