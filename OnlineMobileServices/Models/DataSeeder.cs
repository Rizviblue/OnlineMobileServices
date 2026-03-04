using System.Security.Cryptography;
using System.Text;

namespace OnlineMobileServices.Models
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed Admin User
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                context.Users.Add(new User
                {
                    MobileNumber = "9999999999",
                    FullName = "System Admin",
                    Email = "admin@mobilesvc.com",
                    PasswordHash = HashPassword("Admin@123"),
                    Role = "Admin",
                    CreatedDate = DateTime.Now
                });
                context.SaveChanges();
            }

            // Seed Demo User
            if (!context.Users.Any(u => u.Role == "User"))
            {
                context.Users.Add(new User
                {
                    MobileNumber = "9876543210",
                    FullName = "Rahul Sharma",
                    Email = "rahul@example.com",
                    PasswordHash = HashPassword("User@123"),
                    Role = "User",
                    CreatedDate = DateTime.Now.AddDays(-30)
                });
                context.Users.Add(new User
                {
                    MobileNumber = "9123456789",
                    FullName = "Priya Patel",
                    Email = "priya@example.com",
                    PasswordHash = HashPassword("User@123"),
                    Role = "User",
                    CreatedDate = DateTime.Now.AddDays(-15)
                });
                context.SaveChanges();
            }

            // Seed Recharge Plans
            if (!context.RechargePlans.Any())
            {
                var plans = new List<RechargePlan>
                {
                    // TopUp Plans
                    new() { PlanType = "TopUp", PlanName = "Basic TopUp", Amount = 49, Description = "?38 Talktime + 100MB Data, 14 days validity" },
                    new() { PlanType = "TopUp", PlanName = "Standard TopUp", Amount = 99, Description = "?81 Talktime + 500MB Data, 28 days validity" },
                    new() { PlanType = "TopUp", PlanName = "Value TopUp", Amount = 199, Description = "?162 Talktime + 1GB Data, 28 days validity" },
                    new() { PlanType = "TopUp", PlanName = "Super TopUp", Amount = 299, Description = "?245 Talktime + 2GB Data, 28 days validity" },
                    new() { PlanType = "TopUp", PlanName = "Full TopUp", Amount = 500, Description = "Full Talktime ?500 + 5GB Data, 56 days validity" },

                    // Special Plans
                    new() { PlanType = "Special", PlanName = "Unlimited Starter", Amount = 149, Description = "Unlimited Calls + 1GB/day Data, 24 days validity" },
                    new() { PlanType = "Special", PlanName = "Unlimited Plus", Amount = 249, Description = "Unlimited Calls + 1.5GB/day Data + 100 SMS/day, 28 days validity" },
                    new() { PlanType = "Special", PlanName = "Unlimited Pro", Amount = 449, Description = "Unlimited Calls + 2GB/day Data + 100 SMS/day, 56 days validity" },
                    new() { PlanType = "Special", PlanName = "Unlimited Max", Amount = 599, Description = "Unlimited Calls + 3GB/day Data + 100 SMS/day + Disney+ Hotstar, 56 days validity" },
                    new() { PlanType = "Special", PlanName = "Annual Plan", Amount = 2999, Description = "Unlimited Calls + 2.5GB/day Data + 100 SMS/day, 365 days validity" }
                };
                context.RechargePlans.AddRange(plans);
                context.SaveChanges();
            }

            // Seed Services
            if (!context.Services.Any())
            {
                var services = new List<Service>
                {
                    new() { ServiceName = "Caller Tune", Description = "Set your favourite song as caller tune for ?49/month" },
                    new() { ServiceName = "Data Booster", Description = "Additional 1GB high-speed data for ?19" },
                    new() { ServiceName = "International Roaming", Description = "Enable international roaming with special packs" },
                    new() { ServiceName = "Night Data Pack", Description = "Unlimited data from 12AM-6AM for ?29/month" },
                    new() { ServiceName = "SMS Bundle", Description = "500 SMS pack valid for 28 days at ?39" }
                };
                context.Services.AddRange(services);
                context.SaveChanges();
            }

            // Seed Demo Recharges
            if (!context.Recharges.Any())
            {
                var plans = context.RechargePlans.ToList();
                if (plans.Any())
                {
                    var recharges = new List<Recharge>
                    {
                        new() { MobileNumber = "9876543210", PlanId = plans[0].PlanId, Amount = plans[0].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddDays(-25) },
                        new() { MobileNumber = "9876543210", PlanId = plans[5].PlanId, Amount = plans[5].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddDays(-10) },
                        new() { MobileNumber = "9123456789", PlanId = plans[2].PlanId, Amount = plans[2].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddDays(-5) },
                        new() { MobileNumber = "9876543210", PlanId = plans[7].PlanId, Amount = plans[7].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddDays(-2) },
                        new() { MobileNumber = "9123456789", PlanId = plans[1].PlanId, Amount = plans[1].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddDays(-1) },

                        // Older data for chart
                        new() { MobileNumber = "9876543210", PlanId = plans[3].PlanId, Amount = plans[3].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddMonths(-1) },
                        new() { MobileNumber = "9123456789", PlanId = plans[4].PlanId, Amount = plans[4].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddMonths(-2) },
                        new() { MobileNumber = "9876543210", PlanId = plans[6].PlanId, Amount = plans[6].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddMonths(-3) },
                        new() { MobileNumber = "9123456789", PlanId = plans[8].PlanId, Amount = plans[8].Amount, TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentStatus = "Success", RechargeDate = DateTime.Now.AddMonths(-4) },
                    };
                    context.Recharges.AddRange(recharges);
                    context.SaveChanges();
                }
            }

            // Seed Demo Bills
            if (!context.PostPaidBills.Any())
            {
                var bills = new List<PostPaidBill>
                {
                    new() { MobileNumber = "9876543210", Amount = 850, DueDate = DateTime.Now.AddDays(-20), PaidStatus = "Paid", TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentDate = DateTime.Now.AddDays(-22) },
                    new() { MobileNumber = "9876543210", Amount = 1250, DueDate = DateTime.Now.AddDays(7), PaidStatus = "Unpaid", TransactionId = null, PaymentDate = null },
                    new() { MobileNumber = "9123456789", Amount = 675, DueDate = DateTime.Now.AddDays(-10), PaidStatus = "Paid", TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentDate = DateTime.Now.AddDays(-12) },
                    new() { MobileNumber = "9123456789", Amount = 980, DueDate = DateTime.Now.AddDays(5), PaidStatus = "Unpaid", TransactionId = null, PaymentDate = null },

                    // Older for chart
                    new() { MobileNumber = "9876543210", Amount = 720, DueDate = DateTime.Now.AddMonths(-1), PaidStatus = "Paid", TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentDate = DateTime.Now.AddMonths(-1).AddDays(-2) },
                    new() { MobileNumber = "9123456789", Amount = 1100, DueDate = DateTime.Now.AddMonths(-2), PaidStatus = "Paid", TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentDate = DateTime.Now.AddMonths(-2).AddDays(-1) },
                    new() { MobileNumber = "9876543210", Amount = 950, DueDate = DateTime.Now.AddMonths(-3), PaidStatus = "Paid", TransactionId = Guid.NewGuid().ToString("N")[..12].ToUpper(), PaymentDate = DateTime.Now.AddMonths(-3).AddDays(-3) },
                };
                context.PostPaidBills.AddRange(bills);
                context.SaveChanges();
            }

            // Seed Demo Feedback
            if (!context.Feedback.Any())
            {
                var feedbacks = new List<Feedback>
                {
                    new() { Name = "Rahul Sharma", MobileNumber = "9876543210", Message = "Great service! Recharge was instant and smooth. Highly recommend to everyone.", FeedbackDate = DateTime.Now.AddDays(-5) },
                    new() { Name = "Priya Patel", MobileNumber = "9123456789", Message = "The bill payment process is very easy. Love the user-friendly interface.", FeedbackDate = DateTime.Now.AddDays(-3) },
                    new() { Name = "Amit Kumar", MobileNumber = "9988776655", Message = "Would love to see more recharge plans and cashback offers in future.", FeedbackDate = DateTime.Now.AddDays(-1) },
                    new() { Name = "Neha Singh", MobileNumber = "9112233445", Message = "Quick and reliable. The transaction receipt feature is very helpful for record keeping.", FeedbackDate = DateTime.Now },
                };
                context.Feedback.AddRange(feedbacks);
                context.SaveChanges();
            }

            // Seed User Services
            if (!context.UserServices.Any())
            {
                var users = context.Users.Where(u => u.Role == "User").ToList();
                var services = context.Services.ToList();
                if (users.Any() && services.Any())
                {
                    context.UserServices.Add(new UserService { UserId = users[0].UserId, ServiceId = services[0].ServiceId, IsActive = true });
                    context.UserServices.Add(new UserService { UserId = users[0].UserId, ServiceId = services[1].ServiceId, IsActive = true });
                    context.SaveChanges();
                }
            }
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
