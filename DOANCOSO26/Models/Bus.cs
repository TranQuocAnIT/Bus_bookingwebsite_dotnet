namespace DOANCOSO26.Models
{
    public class Bus
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string BusNumber { get; set; }

        public BusType? BusType { get; set; }
        public ICollection<BusTrip>? BusTrips { get; set; } = new List<BusTrip>();

    }
}
