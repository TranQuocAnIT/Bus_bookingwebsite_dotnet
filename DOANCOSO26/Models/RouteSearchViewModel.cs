namespace DOANCOSO26.Models
{
    public class RouteSearchViewModel
    {
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string EndLatitude { get; set; }
        public string EndLongitude { get; set; }
        public List<BusRoute> MatchingBusRoutes { get; set; } = new List<BusRoute>();
    }
}
