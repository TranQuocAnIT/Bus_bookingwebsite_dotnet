using System.ComponentModel.DataAnnotations;

namespace DOANCOSO26.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string? SeatNumber { get; set; }
       
        public double  Price { get; set; }
        public Status? SeatStatus { get; set; }
        public int BusTripId { get; set; }
        // Navigation property
        public BusTrip? BusTrip { get; set; }
     
        public ICollection<Booking>? Bookings { get; set; }

    }
}
