using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using DOANCOSO26.Services;
using DOANCOSO26.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging.Abstractions;
using System.Drawing;

namespace DOANCOSO26.Controllers
{

    [Authorize(Roles = "Customer")]
    public class ReservationController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IBusTripRepository _busTripRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBusRepository _busRepository;
        private readonly InvoiceCodeGenerator _invoiceCodeGenerator;
        private readonly IBusRouteRepository _busRouteRepository;
        private readonly IStopRepository _stopRepository;
        private readonly IVnPaySevices _vnPaySevices;

        public ReservationController(IBookingRepository bookingRepository, ISeatRepository seatRepository, IBusTripRepository busTripRepository, UserManager<ApplicationUser> userManager, IBusRepository busRepository, InvoiceCodeGenerator invoiceCodeGenerator, IBusRouteRepository busRouteRepository, IStopRepository stopRepository, IVnPaySevices vnPaySevices)
        {
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
            _busTripRepository = busTripRepository;
            _userManager = userManager;
            _busRepository = busRepository;
            _invoiceCodeGenerator = invoiceCodeGenerator;
            _busRouteRepository = busRouteRepository;
            _stopRepository = stopRepository;
            _vnPaySevices = vnPaySevices;
        }

        public async Task<IActionResult> Add(int? tripId, int? seatId)
        {
            if (!tripId.HasValue || !seatId.HasValue)
            {
                return BadRequest("TripId and SeatId are required");
            }

            var availableSeats = await _seatRepository.GetAllAsync();
          

            var stops = await _stopRepository.GetStopsByBusTripIdAsync(tripId.Value);
            stops = stops.OrderBy(s => s.Stt).ToList();

            var selectedSeat = availableSeats.FirstOrDefault(seat => seat.Id == seatId);

            if (selectedSeat == null)
            {
                return BadRequest("Selected seat is not available");
            }

            // Pass the selected seat to the view
            ViewBag.SelectedSeat = selectedSeat;
          
            ViewBag.TripId = tripId;
            ViewBag.Stops = new SelectList(stops, "Id", "DisplayName");

            return View();
        }






        [HttpPost]
        public async Task<IActionResult> Add(Booking booking, string payment = "COD")
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                booking.UserId = user.Id;
                booking.Timebooking = DateTime.Now;
                booking.StatusBooking = StatusBooking.UnPaid;
                var seat = await _seatRepository.GetByIdAsync(booking.SeatId);

               

                booking.TotalPrice = seat.Price;
                booking.Seat = seat;
                booking.StatusOnBus = StatusOnBus.NotintheBus;
                seat.SeatStatus = Status.Booked;
                await _seatRepository.UpdateAsync(seat);

                booking.InvoiceCode = _invoiceCodeGenerator.GenerateInvoiceCode();

                var createdBooking = await _bookingRepository.AddAsync(booking);

                if (createdBooking == null || createdBooking.Id == 0)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo đặt vé. Vui lòng thử lại.");
                    return await ReloadAddView(seat.BusTripId);
                }

                if (payment == "Thanh Toán VNPay")
                {
                    var vnPayModel = new VnPayRequestModel
                    {
                        BookingId = new Random().Next(1000, 10000),
                        TotalPrice = createdBooking.TotalPrice,
                        FullName = createdBooking.UserName,
                        PhoneNum = createdBooking.SDT,
                        Timebooking = DateTime.Now
                    };
                    createdBooking.StatusBooking = StatusBooking.Paid;
                    await _bookingRepository.UpdateAsync(createdBooking);
                    return Redirect(_vnPaySevices.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                return RedirectToAction("Display", new { id = createdBooking.Id });
            }

            var selectedSeat = await _seatRepository.GetByIdAsync(booking.SeatId);
            return await ReloadAddView(selectedSeat?.BusTripId ?? 0);
        }

        private async Task<IActionResult> ReloadAddView(int tripId)
        {
            var availableSeats = await _seatRepository.GetAllAsync();
            availableSeats = availableSeats.Where(seat => seat.BusTripId == tripId && seat.SeatStatus == Status.Available).ToList();

            var stops = await _stopRepository.GetStopsByBusTripIdAsync(tripId);

            ViewBag.Seats = new SelectList(availableSeats, "Id", "SeatNumber");
            ViewBag.TripId = tripId;
            ViewBag.Stops = new SelectList(stops, "Id", "DisplayName");
            return View("Add");
        }
        [Authorize]
        public async Task<IActionResult> VnPayment()
        {

            var response = _vnPaySevices.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VnPay:{response.VnPayResponseCode} ";
                return RedirectToAction("PaymentFail");
            }
            TempData["Message"] = $"Lỗi thanh toán VnPay:{response.VnPayResponseCode} ";

            return RedirectToAction("DisPlay");
        }
        public IActionResult PaymentFail()
        {
            return View();
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
            var booking = await _bookingRepository.GetByIdAsync(id);
            booking.StatusBooking = StatusBooking.Cancelled;
            await _bookingRepository.UpdateAsync(booking);
            return RedirectToAction(nameof(ViewMyBookings));
        }

        [HttpGet]
        public IActionResult SearchBooking()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchBooking(string invoiceCode)
        {
            // Tìm kiếm Booking theo InvoiceCode
            var booking = await _bookingRepository.GetByInvoiceCodeAsync(invoiceCode);

            if (booking != null)
            {
                // Nếu tìm thấy Booking, chuyển hướng đến Action Display với ID của Booking
                return RedirectToAction("Display", new { id = booking.Id });
            }

            // Nếu không tìm thấy Booking, trả về View SearchBooking với thông báo lỗi
            ModelState.AddModelError("", $"Không tìm thấy vé với mã {invoiceCode}");
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetBusTripAndBusInfo(int seatId)
        {

            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null)
            {
                return Json(new { start = "", end = "", busnumber = "", company = "", departureTime = "", departtureDate = "" });
            }

            var busTrip = await _busTripRepository.GetByIdAsync(seat.BusTripId);
            var bus = await _busRepository.GetByIdAsync(busTrip.BusId);
            var route = await _busRouteRepository.GetByIdAsync(busTrip.BusRouteId);
            string seatnumber = seat.SeatNumber.ToString();
            string price = seat.Price.ToString();
            string start = route.Start;
            string end = route.End;
            string busnumber = bus.BusNumber;
            string company = bus.Company;
            string departureTime = busTrip.DepartureTime.ToString("HH:mm");
            string departtureDate = busTrip.DepartureDate.ToString("dd:MM;yyyy");
            string pickuptime = busTrip.DepartureTime.AddMinutes(-30).ToString("HH:mm");

            return Json(new { start, end, busnumber, company, departureTime, departtureDate, seatnumber, price, pickuptime });
        }
        [HttpGet]
        public async Task<IActionResult> ViewMyBookings()
        {

            var seat = await _seatRepository.GetAllAsync();

            var bookings = await _bookingRepository.GetAllAsync();
            var user = await _userManager.GetUserAsync(User);
            var route = await _busRouteRepository.GetAllAsync();
            var trips = await _busTripRepository.GetAllAsync();
            if (user == null)
            {
                return NotFound();
            }

            var userBookings = await _bookingRepository.GetAllByUserIdAsync(user.Id);
            var sortedUserBookings = userBookings
       .Where(x => x.StatusBooking == StatusBooking.UnPaid || x.StatusBooking == StatusBooking.Paid || x.StatusBooking == StatusBooking.Cancelled)
       .OrderByDescending(x => x.Timebooking)
       .ToList();

            return View(sortedUserBookings);

        }
    }
}
