using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDatTourDuLichOnline.Data;
using WebDatTourDuLichOnline.Models;

namespace WebDatTourDuLichOnline.Controllers
{
    [Authorize(Roles = "NhanVien")]
    public class NhanVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public NhanVienController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ----------------------
        // 1) DASHBOARD NHÂN VIÊN
        // ----------------------
        public async Task<IActionResult> Index()
        {
            var homNay = DateTime.Today;

            ViewBag.TongChoXacNhan = await _context.DonDatTours
                .CountAsync(d => d.TrangThaiDon == "ChoXacNhan");

            ViewBag.TongDaXacNhan = await _context.DonDatTours
                .CountAsync(d => d.TrangThaiDon == "DaXacNhan");

            ViewBag.TongDaHuy = await _context.DonDatTours
                .CountAsync(d => d.TrangThaiDon == "DaHuy");

            ViewBag.TongDonHomNay = await _context.DonDatTours
                .CountAsync(d => d.NgayDat.Date == homNay);

            return View();
        }

        // ----------------------------------------------
        // 2) QUẢN LÝ ĐƠN: lọc nhanh + tìm theo mã/khách
        // ----------------------------------------------
        public async Task<IActionResult> DanhSachDon(string trangThai, string keyword)
        {
            var query = _context.DonDatTours
                .Include(d => d.KhachHang)
                .Include(d => d.Tour)
                .AsQueryable();

            // Lọc theo trạng thái nếu khác TatCa
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "TatCa")
                query = query.Where(d => d.TrangThaiDon == trangThai);

            // Tìm theo mã đơn hoặc tên khách
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(d =>
                    d.MaDon.Contains(keyword) ||
                    (d.KhachHang != null && d.KhachHang.HoTen.Contains(keyword)));
            }

            query = query.OrderByDescending(d => d.NgayDat);

            ViewBag.TrangThai = trangThai;
            ViewBag.Keyword = keyword;

            return View(await query.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatDon(int id, string trangThaiDon, string trangThaiThanhToan)
        {
            var don = await _context.DonDatTours.FindAsync(id);
            if (don == null) return NotFound();

            don.TrangThaiDon = trangThaiDon;
            don.TrangThaiThanhToan = trangThaiThanhToan;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachDon));
        }

        // -------------------------------------
        // 3) TẠO ĐƠN HỘ KHÁCH (giữ nguyên logic)
        // -------------------------------------
        [HttpGet]
        public async Task<IActionResult> TaoDon()
        {
            ViewBag.Tours = await _context.Tours
                .Where(t => t.TrangThai) // chỉ tour đang bán/hiển thị
                .OrderBy(t => t.NgayKhoiHanh)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoDon(
            string hoTen,
            string soDienThoai,
            string email,
            string diaChi,
            int tourId,
            int soNguoiLon,
            int soTreEm,
            string ghiChu)
        {
            ViewBag.Tours = await _context.Tours
                .Where(t => t.TrangThai)
                .OrderBy(t => t.NgayKhoiHanh)
                .ToListAsync();

            if (soNguoiLon <= 0 && soTreEm <= 0)
            {
                ModelState.AddModelError(string.Empty, "Phải có ít nhất 1 khách (người lớn hoặc trẻ em).");
                return View();
            }

            var tour = await _context.Tours.FindAsync(tourId);
            if (tour == null)
            {
                ModelState.AddModelError(string.Empty, "Tour không tồn tại.");
                return View();
            }

            int tongKhach = soNguoiLon + soTreEm;
            if (tongKhach > tour.SoChoConLai)
            {
                ModelState.AddModelError(string.Empty, "Số chỗ còn lại không đủ cho số khách này.");
                return View();
            }

            // tìm hoặc tạo khách theo SĐT
            var khach = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.SoDienThoai == soDienThoai);

            if (khach == null)
            {
                khach = new KhachHang
                {
                    HoTen = hoTen,
                    Email = email,
                    SoDienThoai = soDienThoai,
                    DiaChi = diaChi
                };
                _context.KhachHangs.Add(khach);
                await _context.SaveChangesAsync();
            }
            else
            {
                khach.HoTen = hoTen;
                khach.Email = email;
                khach.DiaChi = diaChi;
            }

            decimal giaNL = tour.GiaNguoiLon;
            decimal giaTE = tour.GiaTreEm.HasValue ? tour.GiaTreEm.Value : tour.GiaNguoiLon;
            decimal tongTien = giaNL * soNguoiLon + giaTE * soTreEm;

            var don = new DonDatTour
            {
                MaDon = $"DT{DateTime.Now.Ticks}",
                KhachHangId = khach.KhachHangId,
                TourId = tour.TourId,
                NgayDat = DateTime.Now,
                SoNguoiLon = soNguoiLon,
                SoTreEm = soTreEm,
                TongTien = tongTien,
                TrangThaiThanhToan = "ChuaThanhToan",
                TrangThaiDon = "ChoXacNhan",
                GhiChu = ghiChu
            };

            _context.DonDatTours.Add(don);
            tour.SoChoConLai -= tongKhach;

            // Lưu đơn
            await _context.SaveChangesAsync();

            // Tạo yêu cầu tư vấn cho đơn do nhân viên tạo hộ
            var yeuCau = new YeuCauTuVan
            {
                DonDatTourId = don.DonDatTourId,
                TourId = tour.TourId,
                KhachHangId = khach.KhachHangId,
                Kenh = "NhanVienTaoDon",
                NoiDung = string.IsNullOrWhiteSpace(ghiChu)
                    ? "Nhân viên tạo đơn hộ khách, cần liên hệ xác nhận."
                    : ghiChu,
                ThoiGianTao = DateTime.Now,
                TrangThai = "Moi"
            };

            _context.YeuCauTuVans.Add(yeuCau);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Đã tạo đơn {don.MaDon} cho khách {khach.HoTen}.";
            return RedirectToAction(nameof(DanhSachDon));

        }

        // ---------------------------------------
        // 4) DANH SÁCH KHÁCH THEO TOUR (giữ logic)
        // ---------------------------------------
        public async Task<IActionResult> DanhSachKhachTheoTour(int? tourId)
        {
            ViewBag.Tours = await _context.Tours
                .OrderBy(t => t.NgayKhoiHanh)
                .ToListAsync();

            if (!tourId.HasValue)
            {
                ViewBag.TourDaChon = null;
                return View(Enumerable.Empty<DonDatTour>());
            }

            var tour = await _context.Tours.FindAsync(tourId.Value);
            ViewBag.TourDaChon = tour;

            var donTheoTour = await _context.DonDatTours
                .Include(d => d.KhachHang)
                .Where(d => d.TourId == tourId.Value && d.TrangThaiDon != "DaHuy")
                .OrderBy(d => d.NgayDat)
                .ToListAsync();

            return View(donTheoTour);
        }

        // ============================================================
        // 5) QUẢN LÝ TOUR (NHÂN VIÊN): danh sách / thêm / sửa / ẩn-hiện
        // ============================================================

        // 5.1 Danh sách tour + tìm kiếm
        public async Task<IActionResult> DanhSachTour(string q)
        {
            var query = _context.Tours.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(t =>
                    t.MaTour.Contains(q) ||
                    t.TenTour.Contains(q) ||
                    t.DiemKhoiHanh.Contains(q) ||
                    t.DiemDen.Contains(q));
            }

            query = query.OrderByDescending(t => t.NgayKhoiHanh);
            ViewBag.Keyword = q;

            return View(await query.ToListAsync());
        }

        // 5.2 Thêm tour
        [HttpGet]
        public IActionResult ThemTour() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemTour(
            [Bind("MaTour,TenTour,LoaiTour,DiemKhoiHanh,DiemDen,NgayKhoiHanh,GiaNguoiLon,GiaTreEm,SoCho,HinhAnh")] Tour model,
            IFormFile hinhAnh)
        {
            if (!ModelState.IsValid) return View(model);

            // xử lý ảnh (tùy chọn)
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                model.HinhAnh = await SaveImageAsync(hinhAnh, null);
            }

            // khởi tạo các giá trị hệ thống
            model.SoChoConLai = model.SoCho;
            model.TrangThai = true; // đang bán/hiển thị

            _context.Tours.Add(model);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Đã thêm tour {model.TenTour}.";
            return RedirectToAction(nameof(DanhSachTour));
        }

        // 5.3 Sửa tour
        [HttpGet]
        public async Task<IActionResult> SuaTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();
            return View(tour);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTour(
            int id,
            [Bind("TourId,MaTour,TenTour,LoaiTour,DiemKhoiHanh,DiemDen,NgayKhoiHanh,GiaNguoiLon,GiaTreEm,SoCho,HinhAnh,TrangThai")] Tour model,
            IFormFile hinhAnh)
        {
            if (id != model.TourId) return NotFound();
            if (!ModelState.IsValid) return View(model);

            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.TourId == id);
            if (tour == null) return NotFound();

            // cập nhật thông tin cơ bản
            tour.MaTour = model.MaTour;
            tour.TenTour = model.TenTour;
            tour.LoaiTour = model.LoaiTour;
            tour.DiemKhoiHanh = model.DiemKhoiHanh;
            tour.DiemDen = model.DiemDen;
            tour.NgayKhoiHanh = model.NgayKhoiHanh;
            tour.GiaNguoiLon = model.GiaNguoiLon;
            tour.GiaTreEm = model.GiaTreEm;
            tour.TrangThai = model.TrangThai;

            // điều chỉnh SoCho/SoChoConLai an toàn theo số đã đặt
            var daDat = await _context.DonDatTours
                .Where(d => d.TourId == id && d.TrangThaiDon != "DaHuy")
                .SumAsync(d => (int?)d.SoNguoiLon + d.SoTreEm) ?? 0;

            tour.SoCho = model.SoCho;
            tour.SoChoConLai = Math.Max(tour.SoCho - daDat, 0);

            // xử lý ảnh nếu upload mới
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                tour.HinhAnh = await SaveImageAsync(hinhAnh, tour.HinhAnh);
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = $"Đã cập nhật tour {tour.TenTour}.";
            return RedirectToAction(nameof(DanhSachTour));
        }

        // 5.4 Ẩn/Hiện tour (toggle)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAnHien(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            tour.TrangThai = !tour.TrangThai; // true = hiện (đang bán), false = ẩn
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DanhSachTour));
        }
        // =================== YÊU CẦU TƯ VẤN KHÁCH HÀNG ===================

        public async Task<IActionResult> DanhSachTuVan(string trangThai)
        {
            var query = _context.YeuCauTuVans
                .Include(y => y.KhachHang)
                .Include(y => y.Tour)
                .Include(y => y.DonDatTour)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai) && trangThai != "TatCa")
            {
                query = query.Where(y => y.TrangThai == trangThai);
            }

            var list = await query
                .OrderByDescending(y => y.ThoiGianTao)
                .ToListAsync();

            ViewBag.TrangThai = trangThai;
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTuVan(int id, string trangThai, string? ghiChuNhanVien)
        {
            var yc = await _context.YeuCauTuVans.FindAsync(id);
            if (yc == null) return NotFound();

            yc.TrangThai = trangThai;
            yc.GhiChuNhanVien = ghiChuNhanVien;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachTuVan));
        }
        // -----------------
        // HÀM PHỤ: Lưu ảnh
        // -----------------
        private async Task<string> SaveImageAsync(IFormFile file, string oldFile)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images", "tours");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            // tên file duy nhất
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploads, fileName);

            using (var stream = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(stream);
            }

            // xoá file cũ nếu có
            if (!string.IsNullOrEmpty(oldFile))
            {
                var oldPath = Path.Combine(uploads, oldFile);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            return fileName;
        }
    }
}
