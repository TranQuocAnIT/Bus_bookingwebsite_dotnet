using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DOANCOSO26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BusRouteController : Controller
    {
        private readonly IBusTripRepository _bustripRepository;
        private readonly IBusRouteRepository _busRouteRepository;
        private readonly IStopRepository _stopRepository;
        private readonly IBookingRepository _bookingRepository;
        public BusRouteController(IBusTripRepository bustripRepository, IStopRepository stopRepository, IBusRouteRepository busRouteRepository, IBookingRepository bookingRepository)
        {
            _bustripRepository = bustripRepository;
            _stopRepository = stopRepository;
            _busRouteRepository = busRouteRepository;
            _bookingRepository = bookingRepository;
        }
        public async Task<IActionResult> Index()
        {
            var route = await _busRouteRepository.GetAllAsync();
            return View(route);
        }
        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var route = await _busRouteRepository.GetAllAsync();
            return View();
        }

        // Xử lý thêm loại mới

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BusRoute route , IFormFile imageUrl, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    route.ImageUrl = await SaveImage(imageUrl);
                }
                if (images != null)
                {
                    route.Images = new List<RouteImage>();
                    foreach (var item in images)
                    {
                        RouteImage image = new RouteImage()
                        {
                            BusRouteId = route.Id,
                            Url = await SaveImage(item)
                        };
                        route.Images.Add(image);
                    }
                }
                await _busRouteRepository.AddAsync(route);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            return View(route);
        }
        private async Task<string> SaveImage(IFormFile imageUrl)
        {
            var savePath = Path.Combine("wwwroot/images", imageUrl.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await imageUrl.CopyToAsync(fileStream);
            }
            return "/images/" + imageUrl.FileName;
        }

        // Hiển thị thông tin chi tiết loại
        public async Task<IActionResult> Display(int id)
        {
            var BusRoute = await _busRouteRepository.GetByIdAsync(id);
            if (BusRoute == null)
            {
                return NotFound();
            }
            return View(BusRoute);
        }

        public async Task<IActionResult> Update(int id)
        {
            var BusRoute = await _busRouteRepository.GetByIdAsync(id);
            if (BusRoute == null)
            {
                return NotFound();
            }
            return View(BusRoute);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, BusRoute busroute, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != busroute.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingroute = await _busRouteRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync

                if (imageUrl == null)
                {
                    busroute.ImageUrl = existingroute.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    busroute.ImageUrl = await SaveImage(imageUrl);
                }
                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                existingroute.End = busroute.End;
                existingroute.Start = busroute.Start;
                existingroute.Id = busroute.Id;
                existingroute.Distance = busroute.Distance;
                existingroute.Time = busroute.Time;
                existingroute.Price = busroute.Price;
                existingroute.ImageUrl = busroute.ImageUrl;




                await _busRouteRepository.UpdateAsync(existingroute);
                return RedirectToAction(nameof(Index));
            }
            return View(busroute);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var BusRoute = await _busRouteRepository.GetByIdAsync(id);
            if (BusRoute == null)
            {
                return NotFound();
            }
            return View(BusRoute);
        }


        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _busRouteRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
