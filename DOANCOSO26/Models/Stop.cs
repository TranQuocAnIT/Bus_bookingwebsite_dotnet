namespace DOANCOSO26.Models
{
    public class Stop
    {
        public int Id { get; set; }
        public int? Stt { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int BusRouteId { get; set; }
        public BusRoute? BusRoute { get; set; }
        public string DisplayName => $"{Name} - {Location}";

    }
}
