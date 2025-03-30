using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DOANCOSO26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SeatController : Controller
    {
        private readonly IBusTripRepository _bustripRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IBusRepository _busRepository;
        public SeatController(IBusTripRepository bustripRepository, ISeatRepository seatRepository , IBusRepository busRepository)
        {
            _bustripRepository = bustripRepository;
            _busRepository = busRepository;
            _seatRepository = seatRepository;
        }
        public async Task<IActionResult> Index()
        {
            var seat = await _seatRepository.GetAllAsync();
            return View(seat);
        }
        // Hiển thị form thêm sản phẩm mới
     
        public async Task<IActionResult> Add()
        {
            var bustrip = await _bustripRepository.GetAllAsync();
            ViewBag.BusTrips = new SelectList(bustrip, "Id", "DisplayName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Seat seat)
        {
            if (ModelState.IsValid)
            {
                // Set the initial seat details
                seat.SeatStatus = Status.Available;
                seat.SeatNumber = "A1"; // Assuming this is the first seat added manually through the form
                await _seatRepository.AddAsync(seat);

                var currentBusTrip = await _bustripRepository.GetByIdAsync(seat.BusTripId);

                if (currentBusTrip != null && currentBusTrip.Capacity > 1)
                {
                    var newSeats = new List<Seat>();
                    for (int i = 2; i <= currentBusTrip.Capacity; i++)
                    {
                        string seatPrefix = (i <= 15) ? "A" : "B";
                        int seatNumber = (i <= 15) ? i : (i - 15);
                        var newSeat = new Seat
                        {
                            BusTripId = seat.BusTripId,
                            SeatNumber = seatPrefix + seatNumber,
                            Price = seat.Price,
                            SeatStatus = Status.Available
                        };

                        newSeats.Add(newSeat);
                    }

                    foreach (var newSeat in newSeats)
                    {
                        await _seatRepository.AddAsync(newSeat);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            var bustrip = await _bustripRepository.GetAllAsync();
            ViewBag.BusTrips = new SelectList(bustrip, "Id", "Name");

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            ViewBag.Errors = errors;

            return View(seat);
        }





        // Hiển thị thông tin chi tiết loại
        public async Task<IActionResult> Display(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null)
            {
                return NotFound();
            }
            return View(seat);
        }

        public async Task<IActionResult> Update(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null)
            {
                return NotFound();
            }
            var bustrip = await _bustripRepository.GetAllAsync();
            ViewBag.BusTrips = new SelectList(bustrip, "Id", "Name");
            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
            return View(seat);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Seat seat)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != seat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingSeat = await _seatRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync


                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                existingSeat.SeatNumber = seat.SeatNumber;
                // Cập nhật các thông tin khác của sản phẩm
                existingSeat.Price = seat.Price;

                existingSeat.BusTripId = seat.BusTripId;
                existingSeat.SeatStatus = seat.SeatStatus;
                await _seatRepository.UpdateAsync(existingSeat);
                return RedirectToAction(nameof(Index));
            }
            var bustrip = await _bustripRepository.GetAllAsync();
            ViewBag.BusTrips = new SelectList(bustrip, "Id", "Name");
            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
            return View(seat);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null)
            {
                return NotFound();
            }
            return View(seat);
        }


        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _seatRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
