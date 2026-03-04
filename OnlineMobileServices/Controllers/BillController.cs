using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Models;

namespace OnlineMobileServices.Controllers
{
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Step 1 - Enter Mobile Number
        public IActionResult EnterMobile()
        {
            var mobile = HttpContext.Session.GetString("UserMobile");
            if (!string.IsNullOrEmpty(mobile))
                ViewBag.Mobile = mobile;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterMobile(string mobileNumber)
        {
            if (string.IsNullOrEmpty(mobileNumber) || mobileNumber.Length != 10 || !mobileNumber.All(char.IsDigit))
            {
                TempData["Error"] = "Please enter a valid 10-digit mobile number.";
                return View();
            }

            // Check for existing unpaid bill
            var existingBill = await _context.PostPaidBills
                .FirstOrDefaultAsync(b => b.MobileNumber == mobileNumber && b.PaidStatus == "Unpaid");

            if (existingBill != null)
            {
                return RedirectToAction("Payment", new { id = existingBill.BillId });
            }

            // Generate a new demo bill
            var bill = new PostPaidBill
            {
                MobileNumber = mobileNumber,
                Amount = new Random().Next(500, 3000),
                DueDate = DateTime.Now.AddDays(7),
                PaidStatus = "Unpaid"
            };

            _context.PostPaidBills.Add(bill);
            await _context.SaveChangesAsync();

            return RedirectToAction("Payment", new { id = bill.BillId });
        }

        // Step 2 - Show Bill & Payment
        public async Task<IActionResult> Payment(int id)
        {
            var bill = await _context.PostPaidBills.FindAsync(id);
            if (bill == null)
            {
                TempData["Error"] = "Bill not found.";
                return RedirectToAction("EnterMobile");
            }

            if (bill.PaidStatus == "Paid")
            {
                TempData["Info"] = "This bill has already been paid.";
                return RedirectToAction("Receipt", new { id = bill.BillId });
            }

            return View(bill);
        }

        // Step 3 - Process Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentConfirm(int id)
        {
            try
            {
                var bill = await _context.PostPaidBills.FindAsync(id);
                if (bill == null)
                {
                    TempData["Error"] = "Bill not found.";
                    return RedirectToAction("EnterMobile");
                }

                bill.PaidStatus = "Paid";
                bill.TransactionId = "TXN" + Guid.NewGuid().ToString("N")[..9].ToUpper();
                bill.PaymentDate = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Bill payment successful!";
                return RedirectToAction("Receipt", new { id = bill.BillId });
            }
            catch (Exception)
            {
                TempData["Error"] = "Payment failed. Please try again.";
                return RedirectToAction("EnterMobile");
            }
        }

        // Step 4 - Receipt
        public async Task<IActionResult> Receipt(int id)
        {
            var bill = await _context.PostPaidBills.FindAsync(id);
            if (bill == null)
            {
                TempData["Error"] = "Transaction not found.";
                return RedirectToAction("EnterMobile");
            }

            return View(bill);
        }
    }
}