using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Models;

namespace OnlineMobileServices.Controllers
{
    public class RechargeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RechargeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Step 1 - Enter Mobile Number
        public IActionResult EnterMobile()
        {
            // Pre-fill if user is logged in
            var mobile = HttpContext.Session.GetString("UserMobile");
            if (!string.IsNullOrEmpty(mobile))
                ViewBag.Mobile = mobile;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnterMobile(string mobileNumber)
        {
            if (string.IsNullOrEmpty(mobileNumber) || mobileNumber.Length != 10 || !mobileNumber.All(char.IsDigit))
            {
                TempData["Error"] = "Please enter a valid 10-digit mobile number.";
                return View();
            }

            return RedirectToAction("SelectType", new { mobile = mobileNumber });
        }

        // Step 2 - Select Recharge Type
        public IActionResult SelectType(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return RedirectToAction("EnterMobile");

            ViewBag.Mobile = mobile;
            return View();
        }

        // Step 3 - Show Plans
        public async Task<IActionResult> Plans(string type, string mobile)
        {
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(type))
                return RedirectToAction("EnterMobile");

            var plans = await _context.RechargePlans
                .Where(p => p.PlanType == type)
                .OrderBy(p => p.Amount)
                .ToListAsync();

            ViewBag.Mobile = mobile;
            ViewBag.Type = type;

            return View(plans);
        }

        // Step 4 - Payment page
        public async Task<IActionResult> Payment(int planId, string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return RedirectToAction("EnterMobile");

            var plan = await _context.RechargePlans.FindAsync(planId);
            if (plan == null)
            {
                TempData["Error"] = "Invalid plan selected.";
                return RedirectToAction("EnterMobile");
            }

            ViewBag.Mobile = mobile;
            return View(plan);
        }

        // Step 5 - Process Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentConfirm(int planId, string mobile)
        {
            try
            {
                var plan = await _context.RechargePlans.FindAsync(planId);
                if (plan == null)
                {
                    TempData["Error"] = "Invalid plan.";
                    return RedirectToAction("EnterMobile");
                }

                var recharge = new Recharge
                {
                    MobileNumber = mobile,
                    PlanId = planId,
                    Amount = plan.Amount,
                    TransactionId = "TXN" + Guid.NewGuid().ToString("N")[..9].ToUpper(),
                    PaymentStatus = "Success",
                    RechargeDate = DateTime.Now
                };

                _context.Recharges.Add(recharge);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Recharge successful!";
                return RedirectToAction("Receipt", new { id = recharge.RechargeId });
            }
            catch (Exception)
            {
                TempData["Error"] = "Payment failed. Please try again.";
                return RedirectToAction("EnterMobile");
            }
        }

        // Step 6 - Receipt
        public async Task<IActionResult> Receipt(int id)
        {
            var recharge = await _context.Recharges
                .Include(r => r.RechargePlan)
                .FirstOrDefaultAsync(r => r.RechargeId == id);

            if (recharge == null)
            {
                TempData["Error"] = "Transaction not found.";
                return RedirectToAction("EnterMobile");
            }

            return View(recharge);
        }
    }
}