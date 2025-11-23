using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebDatTourDuLichOnline.Data;
using WebDatTourDuLichOnline.Models;

namespace WebDatTourDuLichOnline.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Trang chủ: tìm kiếm + danh sách tour
        // Trang chủ: tìm kiếm + danh sách tour (có phân trang)
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? diemKhoiHanh,
            string? diemDen,
            DateTime? ngayKhoiHanh,
            int page = 1,
            int pageSize = 9)
        {
            var query = _context.Tours
                .Include(t => t.LoaiTour)
                .Where(t => t.TrangThai == true);

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();
                query = query.Where(t =>
                    t.TenTour.Contains(tuKhoa) ||
                    t.DiemDen.Contains(tuKhoa) ||
                    t.DiemKhoiHanh.Contains(tuKhoa));
            }

            // Lọc theo điểm khởi hành
            if (!string.IsNullOrWhiteSpace(diemKhoiHanh))
            {
                diemKhoiHanh = diemKhoiHanh.Trim();
                query = query.Where(t => t.DiemKhoiHanh.Contains(diemKhoiHanh));
            }

            // Lọc theo điểm đến
            if (!string.IsNullOrWhiteSpace(diemDen))
            {
                diemDen = diemDen.Trim();
                query = query.Where(t => t.DiemDen.Contains(diemDen));
            }

            // Lọc theo ngày khởi hành (>= ngày chọn)
            if (ngayKhoiHanh.HasValue)
            {
                var d = ngayKhoiHanh.Value.Date;
                query = query.Where(t => t.NgayKhoiHanh.Date >= d);
            }

            // Tính tổng số tour & số trang
            int totalTours = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalTours / (double)pageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            // Lấy dữ liệu cho trang hiện tại
            var tours = await query
                .OrderBy(t => t.NgayKhoiHanh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Gửi info phân trang cho View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            // Gửi lại filter (dùng cho tạo link)
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.DiemKhoiHanh = diemKhoiHanh;
            ViewBag.DiemDen = diemDen;
            ViewBag.NgayKhoiHanh = ngayKhoiHanh?.ToString("yyyy-MM-dd");

            return View(tours);
        }
        public IActionResult GioiThieu()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
