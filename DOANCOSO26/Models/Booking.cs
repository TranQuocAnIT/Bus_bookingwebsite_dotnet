using Microsoft.IdentityModel.Protocols.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOANCOSO26.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? InvoiceCode { get; set; }
        public string? UserName { get; set; }
        [Phone]
        [Display(Name = "Phone Number")]
        public string? SDT { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public DateTime Timebooking { get; set; }
        public ApplicationUser? User { get; set; }
        public double TotalPrice { get; set; }
        public string? Note { get; set; }
        public int SeatId { get; set; }
        public Seat? Seat { get; set; }
        public int? TripId { get; set; }
        public BusTrip? BusTrip { get; set; }
        public StatusBooking? StatusBooking  { get; set; }
        public int? PickupStopId { get; set; }
        public Stop? PickupStop { get; set; }
        public int? DropOffStopId { get; set; }
        public Stop? DropOffStop { get; set; }
        public StatusOnBus StatusOnBus { get; set; }
        public Goods? Goods { get; set; }
    }
}
