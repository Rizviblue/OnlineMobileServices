using System.ComponentModel.DataAnnotations;

namespace OnlineMobileServices.Models
{
    public class RechargePlan
    {
        [Key]
        public int PlanId { get; set; }

        [Required]
        public string PlanType { get; set; } // TopUp or Special

        [Required]
        public string PlanName { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string Description { get; set; }
    }
}