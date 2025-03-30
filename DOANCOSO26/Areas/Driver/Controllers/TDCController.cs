//using DOANCOSO26.Models;
//using DOANCOSO26.Repository;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace DOANCOSO26.Areas.Driver.Controllers
//{
//    public class TDCController : Controller
//    {
//        private readonly ITDCertificateRepository _TDCRepository;
//        private readonly IBusTripRepository _busTripRepository;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public TDCController(
//            ITDCertificateRepository departureTicketRepository,
//            IBusTripRepository busTripRepository,
//            UserManager<ApplicationUser> userManager)
//        {
//            _TDCRepository = departureTicketRepository;
//            _busTripRepository = busTripRepository;
//            _userManager = userManager;
//        }
//        public async Task<IActionResult> Index()
//        {
 
//            var tdc = await _TDCRepository.GetAllAsync();
//            return View(tdc);
//        }

//        public async Task<IActionResult> Add()
//        {
//            var tdc = await _TDCRepository.GetAllAsync();
//            var bustrip = await _busTripRepository.GetAllAsync();
//            ViewBag.Bustrips = new SelectList(bustrip, "Id", "");
//            return View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Add(Booking booking)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = await _userManager.GetUserAsync(User);
//                booking.UserId = user.Id;
//                booking.UserName = user.FullName;
//                booking.SDT = user.PhoneNumber;
//                booking.Timebooking = DateTime.Now; // Automatically set the booking time

//                var seat = await _seatRepository.GetByIdAsync(booking.SeatId);
//                if (seat == null || seat.SeatStatus != Status.Available)
//                {
//                    ModelState.AddModelError("SeatId", "The selected seat is not available.");
//                    var availableSeats = await _seatRepository.GetAllAsync();
//                    availableSeats = availableSeats.Where(s => s.SeatStatus == Status.Available).ToList();
//                    ViewBag.Seats = new SelectList(availableSeats, "Id", "SeatNumber");
//                    return View(booking);
//                }

//                booking.TotalPrice = seat.Price; // Set the total price to the seat price
//                booking.BusTrip = await _busTripRepository.GetByIdAsync(seat.BusTripId);

//                seat.SeatStatus = Status.Booked;
//                await _seatRepository.UpdateAsync(seat);
//                await _bookingRepository.AddAsync(booking);

//                return RedirectToAction(nameof(Index));
//            }

//            var seatsList = await _seatRepository.GetAllAsync();
//            seatsList = seatsList.Where(seat => seat.SeatStatus == Status.Available).ToList();
//            ViewBag.Seats = new SelectList(seatsList, "Id", "SeatNumber");
//            return View(booking);
//        }

//        public async Task<IActionResult> Display(int id)
//        {
//            var booking = await _bookingRepository.GetByIdAsync(id);
//            if (booking == null)
//            {
//                return NotFound();
//            }
//            return View(booking);
//        }

//        public async Task<IActionResult> Update(int id)
//        {
//            var booking = await _bookingRepository.GetByIdAsync(id);
//            if (booking == null)
//            {
//                return NotFound();
//            }
//            return View(booking);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Update(int id, Booking booking)
//        {
//            if (id != booking.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                var existingBooking = await _bookingRepository.GetByIdAsync(id);
//                if (existingBooking == null)
//                {
//                    return NotFound();
//                }

//                existingBooking.SDT = booking.SDT;
//                existingBooking.Timebooking = booking.Timebooking; // Keep original booking time
//                existingBooking.TotalPrice = booking.TotalPrice;
//                existingBooking.Note = booking.Note;
//                existingBooking.SeatId = booking.SeatId;

//                await _bookingRepository.UpdateAsync(existingBooking);
//                return RedirectToAction(nameof(Index));
//            }
//            return View(booking);
//        }

//        public async Task<IActionResult> Delete(int id)
//        {
//            var booking = await _bookingRepository.GetByIdAsync(id);
//            if (booking == null)
//            {
//                return NotFound();
//            }
//            return View(booking);
//        }

//        [HttpPost, ActionName("Delete")]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            await _bookingRepository.DeleteAsync(id);
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
