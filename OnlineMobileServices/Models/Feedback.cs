using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineMobileServices.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string Message { get; set; }
        public DateTime FeedbackDate { get; set; }
    }
}

