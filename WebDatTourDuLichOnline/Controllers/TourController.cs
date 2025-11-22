using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebDatTourDuLichOnline.Data;
using WebDatTourDuLichOnline.Models;

namespace WebDatTourDuLichOnline.Controllers
{
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TourController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== CHI TIẾT TOUR (GET) ====================
        public async Task<IActionResult> ChiTiet(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.LoaiTour)
                .FirstOrDefaultAsync(t => t.TourId == id && t.TrangThai == true);

            if (tour == null)
            {
                return NotFound();
            }

            // Lấy danh sách đánh giá cho tour
            var danhGias = await _context.DanhGias
                .Include(d => d.KhachHang)
                .Where(d => d.TourId == id)
                .OrderByDescending(d => d.NgayDanhGia)
                .ToListAsync();

            ViewBag.DanhGias = danhGias;
            ViewBag.SoDanhGia = danhGias.Count;
            ViewBag.DiemTrungBinh = danhGias.Count > 0
                ? danhGias.Average(d => d.SoSao)
                : 0;

            // Lấy vài nhân viên tư vấn để hiển thị
            var nhanVienTuVan = await _context.NhanViens
                .OrderBy(nv => nv.NhanVienId)
                .Take(3)
                .ToListAsync();

            ViewBag.NhanVienTuVan = nhanVienTuVan;

            return View(tour);
        }

        // ==================== ĐẶT TOUR (GET) ====================
        // Mở form đặt tour cho khách hàng
        [Authorize(Roles = "KhachHang")]
        [HttpGet]
        public async Task<IActionResult> DatTour(int id)
        {
            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.TourId == id && t.TrangThai == true);

            if (tour == null)
            {
                return NotFound();
            }

            var model = new DatTourViewModel
            {
                TourId = tour.TourId,
                TenTour = tour.TenTour,
                DiemKhoiHanh = tour.DiemKhoiHanh,
                DiemDen = tour.DiemDen,
                NgayKhoiHanh = tour.NgayKhoiHanh,
                GiaNguoiLon = tour.GiaNguoiLon,
                GiaTreEm = tour.GiaTreEm ?? 0,
                SoChoConLai = tour.SoChoConLai,
                SoNguoiLon = 1,
                SoTreEm = 0,
                TongTien = tour.GiaNguoiLon
            };

            return View(model);
        }

        // ==================== ĐẶT TOUR (POST) ====================
        // Xử lý submit form đặt tour
        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatTour(DatTourViewModel model)
        {
            // Lấy tour theo ID trong model
            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.TourId == model.TourId && t.TrangThai == true);

            if (tour == null)
                return NotFound();

            // Lấy giá từ DB
            decimal giaNguoiLon = tour.GiaNguoiLon;
            decimal giaTreEm = tour.GiaTreEm ?? 0;

            if (model.SoNguoiLon < 0) model.SoNguoiLon = 0;
            if (model.SoTreEm < 0) model.SoTreEm = 0;

            int soNguoi = model.SoNguoiLon + model.SoTreEm;

            // Kiểm tra số lượng khách
            if (soNguoi <= 0)
            {
                ViewBag.ThongBao = "Vui lòng chọn ít nhất 1 khách.";

                // Nạp lại info tour cho view
                model.TenTour = tour.TenTour;
                model.DiemKhoiHanh = tour.DiemKhoiHanh;
                model.DiemDen = tour.DiemDen;
                model.NgayKhoiHanh = tour.NgayKhoiHanh;
                model.GiaNguoiLon = giaNguoiLon;
                model.GiaTreEm = giaTreEm;
                model.SoChoConLai = tour.SoChoConLai;
                model.TongTien = 0;

                return View(model);
            }

            if (soNguoi > tour.SoChoConLai)
            {
                ViewBag.ThongBao = $"Số chỗ còn lại không đủ. Hiện còn {tour.SoChoConLai} chỗ.";

                model.TenTour = tour.TenTour;
                model.DiemKhoiHanh = tour.DiemKhoiHanh;
                model.DiemDen = tour.DiemDen;
                model.NgayKhoiHanh = tour.NgayKhoiHanh;
                model.GiaNguoiLon = giaNguoiLon;
                model.GiaTreEm = giaTreEm;
                model.SoChoConLai = tour.SoChoConLai;
                model.TongTien = 0;

                return View(model);
            }

            decimal tongTien = giaNguoiLon * model.SoNguoiLon + giaTreEm * model.SoTreEm;

            // Lấy tài khoản hiện tại
            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan",
                    new { returnUrl = Url.Action("DatTour", new { id = model.TourId }) });
            }

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
                return Content("Không tìm thấy thông tin khách hàng.");

            // Tạo đơn đặt tour
            var don = new DonDatTour
            {
                MaDon = "DT" + DateTime.Now.Ticks,
                KhachHangId = khachHang.KhachHangId,
                TourId = tour.TourId,
                NgayDat = DateTime.Now,
                SoNguoiLon = model.SoNguoiLon,
                SoTreEm = model.SoTreEm,
                TongTien = tongTien,
                TrangThaiThanhToan = "ChuaThanhToan",
                TrangThaiDon = "ChoXacNhan",
                GhiChu = model.GhiChu
            };

            _context.DonDatTours.Add(don);
            tour.SoChoConLai -= soNguoi;

            // Lưu đơn để có DonDatTourId
            await _context.SaveChangesAsync();

            // Tạo yêu cầu tư vấn cho nhân viên
            var yeuCau = new YeuCauTuVan
            {
                DonDatTourId = don.DonDatTourId,
                TourId = tour.TourId,
                KhachHangId = khachHang.KhachHangId,
                Kenh = "DatTour",
                NoiDung = "Khách vừa đặt tour, cần nhân viên liên hệ xác nhận & tư vấn thêm.",
                ThoiGianTao = DateTime.Now,
                TrangThai = "Moi"
            };

            _context.YeuCauTuVans.Add(yeuCau);
            await _context.SaveChangesAsync();

            return RedirectToAction("DatTourThanhCong", new { id = don.DonDatTourId });
        }

        // ==================== ĐẶT TOUR THÀNH CÔNG ====================
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> DatTourThanhCong(int id)
        {
            var don = await _context.DonDatTours
                .Include(d => d.Tour)
                .Include(d => d.KhachHang)
                .FirstOrDefaultAsync(d => d.DonDatTourId == id);

            if (don == null)
                return NotFound();

            return View(don);
        }

        // ==================== THÊM ĐÁNH GIÁ TOUR ====================
        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDanhGia(int tourId, int soSao, string noiDung)
        {
            // Lấy tài khoản hiện tại
            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan",
                    new { returnUrl = Url.Action("ChiTiet", "Tour", new { id = tourId }) });
            }

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            // Tìm khách hàng tương ứng
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
            {
                return RedirectToAction("ChiTiet", new { id = tourId });
            }

            // Tạo bản ghi đánh giá
            var danhGia = new DanhGia
            {
                TourId = tourId,
                KhachHangId = khachHang.KhachHangId,
                SoSao = soSao,
                NoiDung = noiDung,
                NgayDanhGia = DateTime.Now
            };

            _context.DanhGias.Add(danhGia);
            await _context.SaveChangesAsync();

            return RedirectToAction("ChiTiet", new { id = tourId });
        }
    }
}
