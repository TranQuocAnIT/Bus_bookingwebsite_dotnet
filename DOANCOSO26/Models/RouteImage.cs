namespace DOANCOSO26.Models
{
    public class RouteImage 
    {
        public int Id { get; set; }
        public string Url { get; set; } // Đường dẫn hình ảnh
        public int BusRouteId { get; set; }
        public BusRoute? BusRoute { get; set; }
    }
}
