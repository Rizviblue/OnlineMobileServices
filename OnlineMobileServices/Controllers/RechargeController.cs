using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Models;
using System.Linq;

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
            return View();
        }

        [HttpPost]
        public IActionResult EnterMobile(string mobileNumber)
        {
            if (mobileNumber.Length != 10 || !mobileNumber.All(char.IsDigit))
            {
                ViewBag.Error = "Mobile number must be 10 digits.";
                return View();
            }

            return RedirectToAction("SelectType", new { mobile = mobileNumber });
        }

        public IActionResult SelectType(string mobile)
        {
            ViewBag.Mobile = mobile;
            return View();
        }
        public IActionResult Plans(string type, string mobile)
        {
            var plans = _context.RechargePlans
                .Where(p => p.PlanType == type)
                .ToList();

            ViewBag.Mobile = mobile;
            ViewBag.Type = type;

            return View(plans);
        }
        public IActionResult Payment(int planId, string mobile)
        {
            var plan = _context.RechargePlans.FirstOrDefault(p => p.PlanId == planId);

            ViewBag.Mobile = mobile;
            return View(plan);
        }

        [HttpPost]
        public IActionResult PaymentConfirm(int planId, string mobile)
        {
            var plan = _context.RechargePlans.FirstOrDefault(p => p.PlanId == planId);

            var recharge = new Recharge
            {
                MobileNumber = mobile,
                PlanId = planId,
                Amount = plan.Amount,
                TransactionId = Guid.NewGuid().ToString()
            };

            _context.Recharges.Add(recharge);
            _context.SaveChanges();

            return RedirectToAction("Receipt", new { id = recharge.RechargeId });
        }
        public IActionResult Receipt(int id)
        {
            var recharge = _context.Recharges
                .FirstOrDefault(r => r.RechargeId == id);

            return View(recharge);
        }
    }
}