using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebDatTourDuLichOnline.Data;
using WebDatTourDuLichOnline.Models;

namespace WebDatTourDuLichOnline.Controllers
{
    [Authorize(Roles = "KhachHang")]
    public class ThanhToanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThanhToanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ThanhToan/ThanhToanDon/5
        [HttpGet]
        public async Task<IActionResult> ThanhToanDon(int id)
        {
            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            var don = await _context.DonDatTours
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(d => d.DonDatTourId == id
                                          && d.KhachHangId == khachHang.KhachHangId);

            if (don == null)
                return NotFound();

            if (don.TrangThaiThanhToan == "DaThanhToan")
                return RedirectToAction("DonCuaToi", "TaiKhoan");

            return View(don);
        }

        // POST: Xác nhận thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToanDon(int id, string phuongThuc)
        {
            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            var don = await _context.DonDatTours
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(d => d.DonDatTourId == id
                                          && d.KhachHangId == khachHang.KhachHangId);

            if (don == null)
                return NotFound();

            if (don.TrangThaiThanhToan == "DaThanhToan")
                return RedirectToAction("DonCuaToi", "TaiKhoan");

            // Giả lập thanh toán thành công
            don.TrangThaiThanhToan = "DaThanhToan";

            // Có thể cho rằng sau khi thanh toán thì đơn được xác nhận
            if (don.TrangThaiDon == "ChoXacNhan")
            {
                don.TrangThaiDon = "DaXacNhan";
            }

            await _context.SaveChangesAsync();

            ViewBag.PhuongThuc = phuongThuc;
            return View("ThanhToanThanhCong", don);
        }
    }
}
