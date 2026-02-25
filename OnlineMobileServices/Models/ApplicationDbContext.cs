using Microsoft.EntityFrameworkCore;

namespace OnlineMobileServices.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RechargePlan> RechargePlans { get; set; }
        public DbSet<Recharge> Recharges { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<UserService> UserServices { get; set; }
        public DbSet<PostPaidBill> PostPaidBills { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
    }
}