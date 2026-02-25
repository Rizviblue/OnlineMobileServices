using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineMobileServices.Models
{
    public class PostPaidBill
    {
        [Key]
        public int BillId { get; set; }

        [Required]
        [StringLength(10)]
        public string MobileNumber { get; set; }

        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        public string PaidStatus { get; set; } = "Unpaid";

        public string TransactionId { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}