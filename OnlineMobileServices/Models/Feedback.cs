using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineMobileServices.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.Now;
    }
}