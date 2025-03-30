using DOANCOSO26.Repository;
using System.ComponentModel.DataAnnotations;

namespace DOANCOSO26.Models
{
    public class BusTrip
    {
        
    
        public int Id { get; set; }
        public string DisplayName => $"{Name} - {DepartureDate.ToString("dd/MM/yyyy")}-{DepartureTime.ToString("hh:mm")}";
        [Required(ErrorMessage = "Vui lòng nhập tên chuyến đi")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn xe")]
        public int BusId { get; set; }
        public Bus? Bus { get; set; }
        public int? Capacity { get; set; }
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thời gian xuất phát")]
        [DataType(DataType.Time)]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày xuất phát")]
        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }
        public StatusTrip? TripStatus { get; set; }
        public List<Seat>? Seats { get; set; }
        public int BusRouteId { get; set; }
        public BusRoute? BusRoute { get; set; }
        public List <BusTripImage>? Images { get; set; }
        public string? DriverId { get; set; }
        public ApplicationUser? Driver { get; set; }
        public string? AdminId { get; set; }
        public ApplicationUser? Admin { get; set; }
        public int GetAvailableSeats()
        {
            return Seats?.Count(seat => seat.SeatStatus == Status.Available) ?? 0;
        }
        public int GetBookedSeat()
        {
            return Seats?.Count(seat => seat.SeatStatus == Status.Booked) ?? 0;
        }

    }
}