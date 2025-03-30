namespace DOANCOSO26.Models
{
    public class TripReport
    {
        public int Id { get; set; }
        public int BusTripId { get; set; }
        public BusTrip? BusTrip { get; set; }
        public string? DriverName { get; set; }
        public DateTime? CreateTime { get; set; }
        public double? Gascost { get; set; }
        public  double? Repaircosts { get; set; }
        public double? Anothercost { get; set; }
        public string? DriverId { get; set; }
        public ApplicationUser? Driver { get; set; }
        public string Note { get; set; }
    }
}
