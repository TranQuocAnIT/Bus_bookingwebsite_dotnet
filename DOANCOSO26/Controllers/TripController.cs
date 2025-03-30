using DOANCOSO26.Data;
using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DOANCOSO26.Controllers
{
    [Authorize(Roles = "Customer, Driver")]

    public class TripController : Controller
    {

        private readonly IBusTripRepository _bustripRepository;
        private readonly IBusRepository _busRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ApplicationDbContext _context;
        private readonly ISeatRepository _seatRepository;
        private readonly IStopRepository _stopRepository;
        private readonly IBusRouteRepository _busRouteRepository;
        private readonly ITripReportRepository _tripReportRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDriverregisRepository _driverregisRepository;
        public TripController(
            IBusTripRepository bustripRepository,
            IBusRepository busRepository,
            ApplicationDbContext context,
            ISeatRepository seatRepository,
            IStopRepository stopRepository,
            IBusRouteRepository busRouteRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITripReportRepository tripReportRepository,
            IBookingRepository bookingRepository,
            IDriverregisRepository driverregisRepository)
        {
            _bustripRepository = bustripRepository;
            _busRepository = busRepository;
            _seatRepository = seatRepository;
            _context = context;
            _stopRepository = stopRepository;
            _busRouteRepository = busRouteRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _tripReportRepository = tripReportRepository;
            _bookingRepository = bookingRepository;
            _driverregisRepository = driverregisRepository;
        }


        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(string startLocation, string endLocation, DateTime? departureDate, int? numSeats ,string? company)
        {

            // Lọc chuyến đi dựa trên các điều kiện tìm kiếm
            var buss = await _busRepository.GetAllAsync();
            var rout = await _busRouteRepository.GetAllAsync();
            var busTrips = await _bustripRepository.GetAllAsync();
            foreach (var trip in busTrips)
            {
                trip.Seats = _seatRepository.GetSeatsByBusTripId(trip.Id).ToList();
            }
            return View(busTrips);
        }
        public async Task<IActionResult> IndexCompany(string startLocation, string endLocation, DateTime? departureDate, int? numSeats, string? company)
        {

            // Lọc chuyến đi dựa trên các điều kiện tìm kiếm
            var buss = await _busRepository.GetAllAsync();
            var rout = await _busRouteRepository.GetAllAsync();
            var busTrips = await _bustripRepository.GetAllAsync();
            foreach (var trip in busTrips)
            {
                trip.Seats = _seatRepository.GetSeatsByBusTripId(trip.Id).ToList();
            }
            return View(busTrips);
        }
        public async Task<IActionResult> schedule()
        {
            var routes = await _busRouteRepository.GetAllAsync();
            var groupedRoutes = routes.GroupBy(r => r.Start)
                                      .ToDictionary(g => g.Key, g => g.ToList());
            return View(groupedRoutes);
        }
        public async Task<IActionResult> Search(string Start, string End, DateTime? DepartureDate, int? SeatsRequired ,string? Company)
        {
            ViewBag.StartLocations = await _busRouteRepository.GetAllStartLocationsAsync();
            ViewBag.EndLocations = await _busRouteRepository.GetAllEndLocationsAsync();
            ViewBag.BusCompanys = await _busRepository.GetAllBusCompanyAsync();
            var trips = await _bustripRepository.GetAllAsync();
            trips = trips.Where(t => t.GetAvailableSeats() > 0 && t.TripStatus == StatusTrip.NotYetDeparted).ToList();

            if (!string.IsNullOrEmpty(Start))
            {
                trips = trips.Where(t => t.BusRoute.Start.Contains(Start, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(End))
            {
                trips = trips.Where(t => t.BusRoute.End.Contains(End, StringComparison.OrdinalIgnoreCase)).ToList();
            }
           
            if (!string.IsNullOrEmpty(Company))
            {
                trips = trips.Where(t => t.Bus.Company.Contains(Company, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (DepartureDate.HasValue)
            {
                trips = trips.Where(t => t.DepartureDate.Date == DepartureDate.Value.Date).ToList();
            }

            if (SeatsRequired.HasValue)
            {
                trips = trips.Where(t => t.GetAvailableSeats() >= SeatsRequired.Value).ToList();
            }

            // Sorting trips by DepartureDate in ascending order
            trips = trips.OrderBy(t => t.DepartureTime).ToList();

            foreach (var trip in trips)
            {
                trip.Seats = _seatRepository.GetSeatsByBusTripId(trip.Id).ToList();
            }

            ViewBag.SearchResults = trips;
            return View("SearchAndResult");
        }

        public async Task<IActionResult> SearchAndResult()
        {
            ViewBag.StartLocations = await _busRouteRepository.GetAllStartLocationsAsync();
            ViewBag.EndLocations = await _busRouteRepository.GetAllEndLocationsAsync();
            ViewBag.BusCompanys = await _busRepository.GetAllBusCompanyAsync();
            return View();
        }

        public async Task<IActionResult> Exit(int id)
        {
            var bustrip = await _bustripRepository.GetByIdAsync(id);
            bustrip.TripStatus = StatusTrip.Running;
            if (bustrip == null)
            {
                return NotFound();
            }
            ViewBag.lsImage = _context.BusTripImages.Where(x => x.BusTripId == id).ToList();
            return View(bustrip);
        }
        public async Task<IActionResult> Add()
        {
            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
            return View();
        }

       
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(BusTrip bustrip, IFormFile imageUrl, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    bustrip.ImageUrl = await SaveImage(imageUrl);
                }
                if (images != null)
                {
                    bustrip.Images = new List<BusTripImage>();
                    foreach (var item in images)
                    {
                        BusTripImage image = new BusTripImage()
                        {
                            BusTripId = bustrip.Id,
                            Url = await SaveImage(item)
                        };
                        bustrip.Images.Add(image);
                    }
                }
                await _bustripRepository.AddAsync(bustrip);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
            return View(bustrip);
        }

        // Viết thêm hàm SaveImage (tham khảo bào 02)
        private async Task<string> SaveImage(IFormFile imageUrl)
        {
            var savePath = Path.Combine("wwwroot/images", imageUrl.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await imageUrl.CopyToAsync(fileStream);
            }
            return "/images/" + imageUrl.FileName;
        }
        public async Task<IActionResult> Display(int id)
        {
            var bustrip = await _bustripRepository.GetByIdAsync(id);
            if (bustrip == null)
            {
                return NotFound();
            }
            var busTrips = await _bustripRepository.GetAllAsync();
            foreach (var trip in busTrips)
            {
                trip.Seats = _seatRepository.GetSeatsByBusTripId(trip.Id).ToList();
            }
          
            var stops = await _stopRepository.GetStopsByBusRouteId(bustrip.BusRouteId);

            var sortedStops = stops.OrderBy(stop => stop.Stt).ToList();

            ViewBag.Stops = sortedStops;
            bustrip.Seats = _seatRepository.GetSeatsByBusTripId(bustrip.Id).ToList();

            ViewBag.lsImage = _context.BusTripImages.Where(x => x.BusTripId == id).ToList();

            return View(bustrip);
        }
        public async Task<IActionResult> DisplayDriver(int id)
        {
            var bustrip = await _bustripRepository.GetByIdAsync(id);
            if (bustrip == null)
            {
                return NotFound();
            }
            var busTrips = await _bustripRepository.GetAllAsync();
            foreach (var trip in busTrips)
            {
                trip.Seats = _seatRepository.GetSeatsByBusTripId(trip.Id).ToList();
            }
            // Fetch the stops associated with the BusRouteId of this BusTrip
            var stops = await _stopRepository.GetStopsByBusRouteId(bustrip.BusRouteId);

            // Sắp xếp danh sách trạm dừng theo thuộc tính Stt
            var sortedStops = stops.OrderBy(stop => stop.Stt).ToList();

            // Populate Seats for the BusTrip
            bustrip.Seats = _seatRepository.GetSeatsByBusTripId(bustrip.Id).ToList();

            // Pass the sorted stops to the view
            ViewBag.Stops = sortedStops;

            ViewBag.lsImage = _context.BusTripImages.Where(x => x.BusTripId == id).ToList();

            return View(bustrip);
        }
        // Hiển thị thông tin chi tiết sản phẩm

        public async Task<IActionResult> Update(int id)
        {
            var bustrip = await _bustripRepository.GetByIdAsync(id);
            if (bustrip == null)
            {
                return NotFound();
            }
            var buses = await _bustripRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(buses, "Id", "Name", bustrip.Id);
            return View(bustrip);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, BusTrip bustrip, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != bustrip.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingbustrip = await _bustripRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync


                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    bustrip.ImageUrl = existingbustrip.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    bustrip.ImageUrl = await SaveImage(imageUrl);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingbustrip.Name = bustrip.Name;

                existingbustrip.DepartureDate = bustrip.DepartureDate;
                existingbustrip.DepartureTime = bustrip.DepartureTime;
                existingbustrip.BusId = bustrip.BusId;
                existingbustrip.BusRouteId = bustrip.BusRouteId;
                existingbustrip.TripStatus = bustrip.TripStatus;
                bustrip.ImageUrl = existingbustrip.ImageUrl;


                await _bustripRepository.UpdateAsync(existingbustrip);
                return RedirectToAction(nameof(Index));
            }
            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
            return View(bustrip);
        }


        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var bustrip = await _bustripRepository.GetByIdAsync(id);
            if (bustrip == null)
            {
                return NotFound();
            }
            return View(bustrip);
        }


        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bustripRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MyProfile()
        {

            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }
        public async Task<IActionResult> ViewMySchedule()
        {
            var bustrip = await _bustripRepository.GetAllAsync();
            var user = await _userManager.GetUserAsync(User);


            var userBustrips = await _bustripRepository.GetAllByUserIdAsync(user.Id);
            return View(userBustrips);
        }



        [HttpGet]
        public async Task<IActionResult> CreateReport(int id)
        {
            var busTrip = await _bustripRepository.GetByIdAsync(id);
            var tripReport = new TripReport { BusTrip = busTrip };
            var bookings = await _bookingRepository.GetBookingsByTripIdAsync(id);
            var seatInfoByBookingId = new Dictionary<int, Seat>();
            foreach (var booking in bookings)
            {
                var seat = await _seatRepository.GetSeatByBookingIdAsync(booking.Id);
                seatInfoByBookingId.Add(booking.Id, seat);
            }
            ViewBag.Bookings = bookings;
            ViewBag.SeatInfoByBookingId = seatInfoByBookingId;
            return View(tripReport);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(int id, [Bind("Gascost,Repaircosts,Anothercost,DriverId,Note")] TripReport tripReport)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var busTrip = await _bustripRepository.GetByIdAsync(id);
                    if (busTrip == null)
                    {
                        return NotFound();
                    }

                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    tripReport.BusTripId = busTrip.Id;
                    tripReport.BusTrip = busTrip;
                    tripReport.CreateTime = DateTime.Now;
                    tripReport.DriverName = user.UserName;
                    tripReport.DriverId = user.Id;

                    busTrip.TripStatus = StatusTrip.Departed;
                    await _bustripRepository.UpdateAsync(busTrip);

                    await _tripReportRepository.AddAsync(tripReport);


                    var bookings = await _bookingRepository.GetBookingsByTripIdAsync(id);
                    var seatInfoByBookingId = new Dictionary<int, Seat>();
        
                    foreach (var booking in bookings)
                    {
                        var seat = await _seatRepository.GetSeatByBookingIdAsync(booking.Id);
                        seatInfoByBookingId.Add(booking.Id, seat);

                        string statusOnBus = Request.Form[$"Bookings[{booking.Id}].Arrived"];
                        if (string.IsNullOrEmpty(statusOnBus))
                        {
                            statusOnBus = Request.Form[$"Bookings[{booking.Id}].GotintheBus"];
                        }

                        booking.StatusOnBus = Enum.TryParse(statusOnBus, out StatusOnBus statusResult) ? statusResult : StatusOnBus.NotintheBus;

                        await _bookingRepository.UpdateAsync(booking);
                    }
                    ViewBag.Bookings = bookings;
                    ViewBag.SeatInfoByBookingId = seatInfoByBookingId;

                    return RedirectToAction("DisplayTripReport", new { id = tripReport.Id });
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu báo cáo. Vui lòng thử lại.");
                }
            }

            return View(tripReport);
        }




        public async Task<IActionResult> DisplayTripReport(int id)
        {
            var tripReport = await _tripReportRepository.GetByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (tripReport == null)
            {
                return NotFound();
            }

            var busTripId = tripReport.BusTripId; // Lấy BusTripId từ TripReport

            // Lấy danh sách booking và thông tin về ghế ngồi
            var bookings = await _bookingRepository.GetBookingsByTripIdAsync(busTripId);
            var seatInfoByBookingId = new Dictionary<int, Seat>();
            foreach (var booking in bookings)
            {
                var seat = await _seatRepository.GetSeatByBookingIdAsync(booking.Id);
                seatInfoByBookingId.Add(booking.Id, seat);
            }

            ViewBag.Bookings = bookings;
            ViewBag.SeatInfoByBookingId = seatInfoByBookingId;

            return View(tripReport);
        }
        

      
        public async Task<IActionResult> AddDriver()
        {
            var buses = await _busRepository.GetAllAsync();
            var user = await _userManager.GetUserAsync(User);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDriver(Driverregis driverregis, IFormFile CCCD1imageUrl, IFormFile CCCD2imageUrl , IFormFile GPLX1imageUrl, IFormFile GPLX2imageUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                driverregis.DriverId = user.Id;
                driverregis.ApproveStatus = ApproveStatus.Notyetapproved;

              
                    driverregis.GPLX1ImageUrl = await SaveImage(GPLX1imageUrl);
                    driverregis.GPLX2ImageUrl = await SaveImage(GPLX2imageUrl);
                    driverregis.CCCD1ImageUrl = await SaveImage(CCCD1imageUrl);
                    driverregis.CCCD2ImageUrl = await SaveImage(CCCD2imageUrl);
              

          

                await _driverregisRepository.AddAsync(driverregis);

                // Chuyển hướng đến action DisplayDriverRegistration với ID của phiếu đăng ký mới được thêm
                return RedirectToAction(nameof(DisplayDriverRegistration), new { id = driverregis.Id });
            }

            // Nếu ModelState không hợp lệ, hiển thị lại form với dữ liệu đã nhập
            return View(driverregis);
        }

        // Action để hiển thị chi tiết phiếu đăng ký tài xế sau khi thêm thành công
        public async Task<IActionResult> DisplayDriverRegistration(int id)
        {
            var driverRegis = await _driverregisRepository.GetByIdAsync(id);
            if (driverRegis == null)
            {
                return NotFound();
            }

            return View(driverRegis);
        }
        public async Task<IActionResult> IndexDriverRegis()
        {
            var user = await _userManager.GetUserAsync(User);
            var driverRegisList = await _driverregisRepository.GetByUserIdAsync(user.Id);
            return View(driverRegisList);
        }
    }
}
