using DOANCOSO26.Models;
using DOANCOSO26.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DOANCOSO26.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StopController : Controller
    {
        private readonly IBusTripRepository _bustripRepository;
        private readonly IBusRouteRepository _busRouteRepository;
        private readonly IStopRepository _stopRepository;
        public StopController(IBusTripRepository bustripRepository, IStopRepository stopRepository, IBusRouteRepository busRouteRepository)
        {
            _bustripRepository = bustripRepository;
            _stopRepository = stopRepository;
            _busRouteRepository = busRouteRepository;
        }
        public async Task<IActionResult> Index()
        {
            var stop = await _stopRepository.GetAllAsync();
            return View(stop);
        }
        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var routes = await _busRouteRepository.GetAllAsync();
            ViewBag.BusRoutes = new SelectList(routes, "Id", "DisplayName");
            return View();
        }

        // Xử lý thêm loại mới
        [HttpPost]
        public async Task<IActionResult> Add(Stop stop)
        {
            if (ModelState.IsValid)
            {
                await _stopRepository.AddAsync(stop);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var routes = await _busRouteRepository.GetAllAsync();
            ViewBag.BusRoutes = new SelectList(routes, "Id", "DisplayName");
            return View(stop);
        }
        // Hiển thị thông tin chi tiết loại
        public async Task<IActionResult> Display(int id)
        {
            var stop = await _stopRepository.GetByIdAsync(id);
            if (stop == null)
            {
                return NotFound();
            }
            return View(stop);
        }

        public async Task<IActionResult> Update(int id)
        {
            var stop = await _stopRepository.GetByIdAsync(id);
            if (stop == null)
            {
                return NotFound();
            }
            var routes = await _busRouteRepository.GetAllAsync();
            ViewBag.BusRoutes = new SelectList(routes, "Id", "DisplayName");
            return View(stop);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Stop stop)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != stop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingstop = await _stopRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync


                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                existingstop.Name = stop.Name;
                existingstop.Stt = stop.Stt;
                // Cập nhật các thông tin khác của sản phẩm
                existingstop.Location = stop.Location;
                existingstop.BusRouteId = stop.BusRouteId;



                await _stopRepository.UpdateAsync(existingstop);
                return RedirectToAction(nameof(Index));
            }
            var routes = await _busRouteRepository.GetAllAsync();
            ViewBag.BusRoutes = new SelectList(routes, "Id", "DisplayName");
            return View(stop);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var stop = await _stopRepository.GetByIdAsync(id);
            if (stop == null)
            {
                return NotFound();
            }
            return View(stop);
        }


        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _stopRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
