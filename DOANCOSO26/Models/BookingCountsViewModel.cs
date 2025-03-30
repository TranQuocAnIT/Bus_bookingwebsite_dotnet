namespace DOANCOSO26.Models
{
    public class BookingCountsViewModel
    {
        public int TodayBookings { get; set; }
        public int TotalBookings { get; set; }
        public double TotalPaidBookingsPrice { get; set; }
        public double TotalCost { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public double TotalProfit => TotalPaidBookingsPrice - TotalCost;
    }
}
