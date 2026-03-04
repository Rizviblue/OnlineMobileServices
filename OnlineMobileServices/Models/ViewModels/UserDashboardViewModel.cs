namespace OnlineMobileServices.Models.ViewModels
{
    public class UserDashboardViewModel
    {
        public User User { get; set; } = null!;
        public List<Recharge> RecentRecharges { get; set; } = new();
        public List<PostPaidBill> RecentBills { get; set; } = new();
        public List<UserService> ActiveServices { get; set; } = new();
        public int TotalRecharges { get; set; }
        public int TotalBillsPaid { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
