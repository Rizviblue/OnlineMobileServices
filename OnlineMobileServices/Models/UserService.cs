using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMobileServices.Models
{
    public class UserService
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ServiceId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
    }
}