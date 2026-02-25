using System.ComponentModel.DataAnnotations;

namespace OnlineMobileServices.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        public string ServiceName { get; set; }

        public string Description { get; set; }
    }
}