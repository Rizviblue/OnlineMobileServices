using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMobileServices.Models
{
    public class Recharge
    {
        [Key]
        public int RechargeId { get; set; }

        [Required]
        [StringLength(10)]
        public string MobileNumber { get; set; }

        [Required]
        public int PlanId { get; set; }

        [ForeignKey("PlanId")]
        public RechargePlan RechargePlan { get; set; }

        public decimal Amount { get; set; }

        public string TransactionId { get; set; }

        public string PaymentStatus { get; set; } = "Success";

        public DateTime RechargeDate { get; set; } = DateTime.Now;
    }
}