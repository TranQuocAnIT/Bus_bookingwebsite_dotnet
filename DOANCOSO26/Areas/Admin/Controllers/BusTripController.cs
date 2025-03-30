using DOANCOSO26.Data;
using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace DOANCOSO26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class BusTripController : Controller
    {
       
            private readonly IBusTripRepository _bustripRepository;
            private readonly IBusRepository _busRepository;
            private readonly ApplicationDbContext _context;
        private readonly ISeatRepository _seatRepository;
        private readonly IStopRepository _stopRepository;
        private readonly IBusRouteRepository _busRouteRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly  IBookingRepository _bookingRepository;
        private readonly ITripReportRepository _TripReportRepository;
        private readonly IDriverregisRepository _driverregisRepository;
        public BusTripController(IBusTripRepository bustripRepository, IBusRepository busRepository, ApplicationDbContext context, ISeatRepository seatRepository, IStopRepository stopRepository, IBusRouteRepository busRouteRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager , IBookingRepository bookingRepository, ITripReportRepository tripReportRepository,    IDriverregisRepository driverregisRepository )
            {
                _bustripRepository = bustripRepository;
                _busRepository = busRepository;
            _seatRepository = seatRepository;
            _context = context;
            _stopRepository = stopRepository;
            _busRouteRepository = busRouteRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _bookingRepository = bookingRepository;
            _TripReportRepository = tripReportRepository;
            _driverregisRepository = driverregisRepository;
        }


            // Hiển thị danh sách sản phẩm
            public async Task<IActionResult> Index()
            {
            var rout = await _busRouteRepository.GetAllAsync();
            var busTrips = await _bustripRepository.GetAllAsync();
            foreach (var trip in busTrips)
            {
                trip.Seats = _seatRepository.GetSeatsByBusTripId(trip.Id).ToList();
            }
            
            return View(busTrips);
        }
            // Hiển thị form thêm sản phẩm mới
            public async Task<IActionResult> Add()
            {
            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
            var BusRoute= await _busRouteRepository.GetAllAsync();
            ViewBag.BusRoutes = new SelectList(BusRoute, "Id", "DisplayName");
            var drivers = await _userManager.GetUsersInRoleAsync(Roles.Role_Driver);
            ViewBag.Drivers = new SelectList(drivers, "Id", "FullName");
            return View();
            }

        public async Task<IActionResult> IndexDriver()
        {
            
            var drivers = await _userManager.GetUsersInRoleAsync(Roles.Role_Driver);
            return View(drivers);
        }
        public async Task<IActionResult> IndexAdmin()
        {

            var admin = await _userManager.GetUsersInRoleAsync(Roles.Role_Admin);
            return View(admin);
        }
        public async Task<IActionResult> IndexCustomer()
        {

            var user = await _userManager.GetUsersInRoleAsync(Roles.Role_Customer);
            return View(user);
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
                    var user = await _userManager.GetUserAsync(User);
                    bustrip.AdminId = user.Id;
                    bustrip.TripStatus = StatusTrip.NotYetDeparted;
                    await _bustripRepository.AddAsync(bustrip);
                    return RedirectToAction(nameof(Index));
                }
                // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
                var buses = await _busRepository.GetAllAsync();
                ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
                var BusRoute= await _busRouteRepository.GetAllAsync();
                ViewBag.BusRoutes = new SelectList(BusRoute, "Id", "DisplayName");
                var drivers = await _userManager.GetUsersInRoleAsync(Roles.Role_Driver);
                ViewBag.Drivers = new SelectList(drivers, "Id", "FullName");
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


        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _bustripRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.lsImage = _context.BusTripImages.Where(x => x.BusTripId == id).ToList();
            return View(product);
        }

        public async Task<IActionResult> Exit(int id)
        {
            var bustrip = await _bustripRepository.GetByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (bustrip == null)
            {
                return NotFound();
            }
            var bookings = await _bookingRepository.GetBookingsByTripIdAsync(id);
            var seatInfoByBookingId = new Dictionary<int, Seat>();
            foreach (var booking in bookings)
            {
                var seat = await _seatRepository.GetSeatByBookingIdAsync(booking.Id);
                seatInfoByBookingId.Add(booking.Id, seat);
            }

            var stops = await _stopRepository.GetStopsByBusRouteId(bustrip.BusRouteId);

            var sortedStops = stops.OrderBy(stop => stop.Stt).ToList();

            ViewBag.Stops = sortedStops;
            ViewBag.Bookings = bookings;
            ViewBag.SeatInfoByBookingId = seatInfoByBookingId;
            ViewBag.lsImage = _context.BusTripImages.Where(x => x.BusTripId == id).ToList();
            bustrip.TripStatus = StatusTrip.Running;
            await _bustripRepository.UpdateAsync(bustrip);
            return View(bustrip);
        }
        public async Task<IActionResult> TripReport(int id)
        {
            var tripReport = await _TripReportRepository.GetByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            if (tripReport == null)
            {
                return NotFound();
            }

            var busTripId = tripReport.BusTripId; 

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
        public async Task<IActionResult> TripReportIndex()
        {
            var tripreport  = await _TripReportRepository.GetAllAsync();
            return View(tripreport);
        }
            public async Task<IActionResult> Update(int id)
        {
            var bustrip = await _bustripRepository.GetByIdAsync(id);
         
            if (bustrip == null)
            {
                return NotFound();
            }
            var buses = await _busRepository.GetAllAsync();
            ViewBag.Buses = new SelectList(buses, "Id", "BusNumber");
            var BusRoute= await _busRouteRepository.GetAllAsync();
            ViewBag.BusRoutes = new SelectList(BusRoute, "Id", "DisplayName");
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
                existingbustrip.Capacity = bustrip.Capacity;
                existingbustrip.ImageUrl = bustrip.ImageUrl;
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
            var BusRoute= await _busRouteRepository.GetAllAsync();
            ViewBag.BusRoutes= new SelectList(BusRoute, "Id", "Start", "End");
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
        public IActionResult SearchAndResult()
        {
            return View();
        }
       


        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bustripRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
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
           
            var driverRegisList = await _driverregisRepository.GetAllAsync();
            return View(driverRegisList);
        }
        public async Task<IActionResult> UpdateRoleDriver(int id)
        {
            // Tìm kiếm phiếu đăng ký tài xế
            var driverRegis = await _driverregisRepository.GetByIdAsync(id);
            if (driverRegis == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái thành "Đã phê duyệt"
            driverRegis.ApproveStatus = ApproveStatus.Approved;
            await _driverregisRepository.UpdateAsync(driverRegis);

            // Tìm kiếm người dùng
            var user = await _userManager.FindByIdAsync(driverRegis.DriverId);
            if (user == null)
            {
                return NotFound();
            }

            // Thay đổi vai trò của người dùng từ "Customer" thành "Driver"
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("Customer"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
            }
            if (!currentRoles.Contains("Driver"))
            {
                await _userManager.AddToRoleAsync(user, "Driver");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
