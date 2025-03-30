using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DOANCOSO26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IBusTripRepository _busTripRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBusRepository _busRepository;
        public BookingController(IBookingRepository bookingRepository, ISeatRepository seatRepository, IBusTripRepository busTripRepository, UserManager<ApplicationUser> userManager, IBusRepository busRepository)
        {
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
            _busTripRepository = busTripRepository;
            _userManager = userManager;
            _busRepository = busRepository;
        }

        public async Task<IActionResult> Index()
        {
            var seat  = await _seatRepository.GetAllAsync();
            var bookings = await _bookingRepository.GetAllAsync();
            return View(bookings);
        }

        public async Task<IActionResult> Add()
        {
            var availableSeats = await _seatRepository.GetAllAsync();
            availableSeats = availableSeats.Where(seat => seat.SeatStatus == Status.Available).ToList();
            ViewBag.Seats = new SelectList(availableSeats, "Id", "SeatNumber");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                booking.UserId = user.Id;
                booking.UserName = user.FullName;
                booking.SDT = user.PhoneNumber;
                booking.Timebooking = DateTime.Now; // Automatically set the booking time

                var seat = await _seatRepository.GetByIdAsync(booking.SeatId);
                if (seat == null || seat.SeatStatus != Status.Available)
                {
                    ModelState.AddModelError("SeatId", "The selected seat is not available.");
                    var availableSeats = await _seatRepository.GetAllAsync();
                    availableSeats = availableSeats.Where(s => s.SeatStatus == Status.Available).ToList();
                    ViewBag.Seats = new SelectList(availableSeats, "Id", "SeatNumber");
                    return View(booking);
                }

                booking.TotalPrice = seat.Price; // Set the total price to the seat price
                booking.BusTrip = await _busTripRepository.GetByIdAsync(seat.BusTripId);

                seat.SeatStatus = Status.Booked;
                await _seatRepository.UpdateAsync(seat);
                await _bookingRepository.AddAsync(booking);

                return RedirectToAction(nameof(Index));
            }

            var seatsList = await _seatRepository.GetAllAsync();
            seatsList = seatsList.Where(seat => seat.SeatStatus == Status.Available).ToList();
            ViewBag.Seats = new SelectList(seatsList, "Id", "SeatNumber");
            return View(booking);
        }

        public async Task<IActionResult> Display(int id)
        {

            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses);
            var seat = await _seatRepository.GetAllAsync();
            ViewBag.Seats = new SelectList(seat);
            var butrip = await _busTripRepository.GetAllAsync();
            ViewBag.Bustrips = new SelectList(butrip);
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);

        }

        public async Task<IActionResult> Update(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            booking.StatusBooking = StatusBooking.Paid;
            await _bookingRepository.UpdateAsync(booking);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingBooking = await _bookingRepository.GetByIdAsync(id);
                if (existingBooking == null)
                {
                    return NotFound();
                }

                existingBooking.SDT = booking.SDT;
                existingBooking.Timebooking = booking.Timebooking; // Keep original booking time
                existingBooking.TotalPrice = booking.TotalPrice;
                existingBooking.Note = booking.Note;
                existingBooking.SeatId = booking.SeatId;

                await _bookingRepository.UpdateAsync(existingBooking);
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookingRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
