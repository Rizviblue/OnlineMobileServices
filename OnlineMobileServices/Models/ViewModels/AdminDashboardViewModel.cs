namespace OnlineMobileServices.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRecharges { get; set; }
        public int TotalBills { get; set; }
        public int TotalFeedback { get; set; }
        public decimal TotalRechargeRevenue { get; set; }
        public decimal TotalBillRevenue { get; set; }
        public decimal TotalRevenue => TotalRechargeRevenue + TotalBillRevenue;

        // Monthly revenue for chart (last 6 months)
        public List<string> MonthLabels { get; set; } = new();
        public List<decimal> MonthlyRechargeRevenue { get; set; } = new();
        public List<decimal> MonthlyBillRevenue { get; set; } = new();

        // Recent activity
        public List<Recharge> RecentRecharges { get; set; } = new();
        public List<PostPaidBill> RecentBills { get; set; } = new();
        public List<Feedback> RecentFeedback { get; set; } = new();
    }
}
