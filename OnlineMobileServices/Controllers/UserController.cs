using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Models;
using OnlineMobileServices.Models.ViewModels;

namespace OnlineMobileServices.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? GetUserId()
        {
            var uid = HttpContext.Session.GetString("UserId");
            return uid != null ? int.Parse(uid) : null;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null) return RedirectToAction("Login", "Account");

                var recharges = await _context.Recharges
                    .Where(r => r.MobileNumber == user.MobileNumber)
                    .OrderByDescending(r => r.RechargeDate)
                    .Take(5)
                    .ToListAsync();

                var bills = await _context.PostPaidBills
                    .Where(b => b.MobileNumber == user.MobileNumber)
                    .OrderByDescending(b => b.PaymentDate ?? b.DueDate)
                    .Take(5)
                    .ToListAsync();

                var activeServices = await _context.UserServices
                    .Include(us => us.Service)
                    .Where(us => us.UserId == userId.Value && us.IsActive)
                    .ToListAsync();

                var totalSpent = await _context.Recharges
                    .Where(r => r.MobileNumber == user.MobileNumber)
                    .SumAsync(r => r.Amount)
                    + await _context.PostPaidBills
                    .Where(b => b.MobileNumber == user.MobileNumber && b.PaidStatus == "Paid")
                    .SumAsync(b => b.Amount);

                var vm = new UserDashboardViewModel
                {
                    User = user,
                    RecentRecharges = recharges,
                    RecentBills = bills,
                    ActiveServices = activeServices,
                    TotalRecharges = await _context.Recharges.CountAsync(r => r.MobileNumber == user.MobileNumber),
                    TotalBillsPaid = await _context.PostPaidBills.CountAsync(b => b.MobileNumber == user.MobileNumber && b.PaidStatus == "Paid"),
                    TotalSpent = totalSpent
                };

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred loading your dashboard.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Profile()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null) return RedirectToAction("Login", "Account");

            return View(user);
        }

        public async Task<IActionResult> EditProfile()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null) return RedirectToAction("Login", "Account");

            var vm = new EditProfileViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            try
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null) return RedirectToAction("Login", "Account");

                user.FullName = model.FullName;
                user.Email = model.Email;

                await _context.SaveChangesAsync();

                // Update session
                HttpContext.Session.SetString("Username", user.FullName);

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred updating your profile.";
                return View(model);
            }
        }

        public async Task<IActionResult> Services()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var services = await _context.Services.ToListAsync();
            var userServices = await _context.UserServices
                .Where(us => us.UserId == userId.Value && us.IsActive)
                .Select(us => us.ServiceId)
                .ToListAsync();

            ViewBag.UserServices = userServices;
            return View(services);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateService(int serviceId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            try
            {
                // Check if already active
                var existing = await _context.UserServices
                    .FirstOrDefaultAsync(us => us.UserId == userId.Value && us.ServiceId == serviceId);

                if (existing != null)
                {
                    existing.IsActive = true;
                }
                else
                {
                    var userService = new UserService
                    {
                        UserId = userId.Value,
                        ServiceId = serviceId,
                        IsActive = true
                    };
                    _context.UserServices.Add(userService);
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Service activated successfully!";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred activating the service.";
            }

            return RedirectToAction("Services");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateService(int serviceId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            try
            {
                var existing = await _context.UserServices
                    .FirstOrDefaultAsync(us => us.UserId == userId.Value && us.ServiceId == serviceId);

                if (existing != null)
                {
                    existing.IsActive = false;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Service deactivated successfully.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred deactivating the service.";
            }

            return RedirectToAction("Services");
        }

        public async Task<IActionResult> RechargeHistory()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var mobile = HttpContext.Session.GetString("UserMobile");
            var recharges = await _context.Recharges
                .Include(r => r.RechargePlan)
                .Where(r => r.MobileNumber == mobile)
                .OrderByDescending(r => r.RechargeDate)
                .ToListAsync();

            return View(recharges);
        }

        public async Task<IActionResult> BillHistory()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var mobile = HttpContext.Session.GetString("UserMobile");
            var bills = await _context.PostPaidBills
                .Where(b => b.MobileNumber == mobile)
                .OrderByDescending(b => b.PaymentDate ?? b.DueDate)
                .ToListAsync();

            return View(bills);
        }
    }
}