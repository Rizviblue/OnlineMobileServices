using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Models;
using OnlineMobileServices.Models.ViewModels;

namespace OnlineMobileServices.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // Admin Dashboard with statistics and charts
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            try
            {
                var vm = new AdminDashboardViewModel
                {
                    TotalUsers = await _context.Users.CountAsync(u => u.Role == "User"),
                    TotalRecharges = await _context.Recharges.CountAsync(),
                    TotalBills = await _context.PostPaidBills.CountAsync(),
                    TotalFeedback = await _context.Feedback.CountAsync(),
                    TotalRechargeRevenue = await _context.Recharges.SumAsync(r => r.Amount),
                    TotalBillRevenue = await _context.PostPaidBills.Where(b => b.PaidStatus == "Paid").SumAsync(b => b.Amount),
                    RecentRecharges = await _context.Recharges.OrderByDescending(r => r.RechargeDate).Take(5).ToListAsync(),
                    RecentBills = await _context.PostPaidBills.OrderByDescending(b => b.PaymentDate ?? b.DueDate).Take(5).ToListAsync(),
                    RecentFeedback = await _context.Feedback.OrderByDescending(f => f.FeedbackDate).Take(5).ToListAsync()
                };

                // Monthly revenue for chart (last 6 months)
                for (int i = 5; i >= 0; i--)
                {
                    var month = DateTime.Now.AddMonths(-i);
                    var startOfMonth = new DateTime(month.Year, month.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1);

                    vm.MonthLabels.Add(month.ToString("MMM yyyy"));

                    var rechargeRev = await _context.Recharges
                        .Where(r => r.RechargeDate >= startOfMonth && r.RechargeDate < endOfMonth)
                        .SumAsync(r => r.Amount);
                    vm.MonthlyRechargeRevenue.Add(rechargeRev);

                    var billRev = await _context.PostPaidBills
                        .Where(b => b.PaidStatus == "Paid" && b.PaymentDate >= startOfMonth && b.PaymentDate < endOfMonth)
                        .SumAsync(b => b.Amount);
                    vm.MonthlyBillRevenue.Add(billRev);
                }

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred loading the dashboard.";
                return View(new AdminDashboardViewModel());
            }
        }

        // User Management - List
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var users = await _context.Users.OrderByDescending(u => u.CreatedDate).ToListAsync();
            return View(users);
        }

        // Delete User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null && user.Role != "Admin")
                {
                    // Remove related data
                    var userServices = _context.UserServices.Where(us => us.UserId == id);
                    _context.UserServices.RemoveRange(userServices);

                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "User deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Cannot delete admin user.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred deleting the user.";
            }

            return RedirectToAction("Users");
        }

        // Edit User - GET
        public async Task<IActionResult> EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return RedirectToAction("Users");

            return View(user);
        }

        // Edit User - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, string fullName, string email, string role)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return RedirectToAction("Users");

                user.FullName = fullName;
                user.Email = email;
                user.Role = role;

                await _context.SaveChangesAsync();
                TempData["Success"] = "User updated successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred updating the user.";
            }

            return RedirectToAction("Users");
        }

        // View All Recharges
        public async Task<IActionResult> ViewRecharges()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var recharges = await _context.Recharges
                .Include(r => r.RechargePlan)
                .OrderByDescending(r => r.RechargeDate)
                .ToListAsync();

            return View(recharges);
        }

        // View All Bills
        public async Task<IActionResult> ViewBills()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var bills = await _context.PostPaidBills
                .OrderByDescending(b => b.PaymentDate ?? b.DueDate)
                .ToListAsync();

            return View(bills);
        }

        // View All Feedback
        public async Task<IActionResult> ViewFeedback()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var feedbacks = await _context.Feedback
                .OrderByDescending(f => f.FeedbackDate)
                .ToListAsync();

            return View(feedbacks);
        }

        // Delete Feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            try
            {
                var feedback = await _context.Feedback.FindAsync(id);
                if (feedback != null)
                {
                    _context.Feedback.Remove(feedback);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Feedback deleted.";
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred deleting the feedback.";
            }

            return RedirectToAction("ViewFeedback");
        }
    }
}