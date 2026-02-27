using Microsoft.AspNetCore.Mvc;
using OnlineMobileServices.Models;
using System;
using System.Linq;

namespace OnlineMobileServices.Controllers
{
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillController(ApplicationDbContext context)
        {
            _context = context;
        }

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

            return RedirectToAction("GenerateBill", new { mobile = mobileNumber });
        }
        public IActionResult GenerateBill(string mobile)
        {
            var bill = new PostPaidBill
            {
                MobileNumber = mobile,
                Amount = new Random().Next(500, 3000),
                DueDate = DateTime.Now.AddDays(7)
            };

            _context.PostPaidBills.Add(bill);
            _context.SaveChanges();

            return RedirectToAction("Payment", new { id = bill.BillId });
        }
        public IActionResult Payment(int id)
        {
            var bill = _context.PostPaidBills.FirstOrDefault(b => b.BillId == id);
            return View(bill);
        }

        [HttpPost]
        public IActionResult PaymentConfirm(int id)
        {
            var bill = _context.PostPaidBills.FirstOrDefault(b => b.BillId == id);

            bill.PaidStatus = "Paid";
            bill.TransactionId = Guid.NewGuid().ToString();
            bill.PaymentDate = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Receipt", new { id = bill.BillId });
        }
        public IActionResult Receipt(int id)
        {
            var bill = _context.PostPaidBills.FirstOrDefault(b => b.BillId == id);
            return View(bill);
        }
    }
}