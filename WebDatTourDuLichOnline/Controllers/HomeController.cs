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
        public async Task<IActionResult> Index(string? tuKhoa, string? diemKhoiHanh, string? diemDen, DateTime? ngayKhoiHanh)
        {
            var query = _context.Tours
                .Include(t => t.LoaiTour)
                .Where(t => t.TrangThai == true);

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = query.Where(t =>
                    t.TenTour.Contains(tuKhoa) ||
                    t.DiemDen.Contains(tuKhoa) ||
                    t.DiemKhoiHanh.Contains(tuKhoa));
            }

            if (!string.IsNullOrWhiteSpace(diemKhoiHanh))
            {
                query = query.Where(t => t.DiemKhoiHanh.Contains(diemKhoiHanh));
            }

            if (!string.IsNullOrWhiteSpace(diemDen))
            {
                query = query.Where(t => t.DiemDen.Contains(diemDen));
            }

            if (ngayKhoiHanh.HasValue)
            {
                query = query.Where(t => t.NgayKhoiHanh >= ngayKhoiHanh.Value);
            }

            var tours = await query
                .OrderBy(t => t.NgayKhoiHanh)
                .Take(12)
                .ToListAsync();

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
