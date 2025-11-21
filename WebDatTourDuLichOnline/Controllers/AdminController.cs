using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebDatTourDuLichOnline.Data;
using WebDatTourDuLichOnline.Models;

namespace WebDatTourDuLichOnline.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ========================= DASHBOARD =========================
        public async Task<IActionResult> Index()
        {
            ViewBag.SoTour = await _context.Tours.CountAsync();
            ViewBag.SoDon = await _context.DonDatTours.CountAsync();
            ViewBag.SoDonCho = await _context.DonDatTours.CountAsync(d => d.TrangThaiDon == "ChoXacNhan");
            ViewBag.DoanhThu = await _context.DonDatTours
                                   .Where(d => d.TrangThaiDon == "DaHoanTat")
                                   .SumAsync(d => (decimal?)d.TongTien) ?? 0m;
            return View();
        }

        // ========================= TOUR =========================

        // Danh sách tour
        public async Task<IActionResult> DanhSachTour()
        {
            var tours = await _context.Tours
                .Include(t => t.LoaiTour)
                .OrderBy(t => t.NgayKhoiHanh)
                .ToListAsync();
            return View(tours);
        }

        // GET: Thêm tour
        [HttpGet]
        public async Task<IActionResult> ThemTour()
        {
            ViewBag.LoaiTours = new SelectList(
                await _context.LoaiTours.AsNoTracking().ToListAsync(),
                "LoaiTourId", "TenLoai");

            return View(new Tour
            {
                TrangThai = true,                       // mặc định đang bán
                NgayKhoiHanh = DateTime.Today.AddDays(7)
            });
        }

        // POST: Thêm tour (kèm upload ảnh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemTour(Tour model, IFormFile? hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LoaiTours = new SelectList(
                    await _context.LoaiTours.AsNoTracking().ToListAsync(),
                    "LoaiTourId", "TenLoai");
                return View(model);
            }

            // ảnh
            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                model.HinhAnh = await SaveImageAsync(hinhAnh);
            }

            // đồng bộ còn lại
            if (model.SoChoConLai <= 0 || model.SoChoConLai > model.SoCho)
                model.SoChoConLai = model.SoCho;

            _context.Tours.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachTour));
        }

        // GET: Sửa tour
        [HttpGet]
        public async Task<IActionResult> SuaTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            ViewBag.LoaiTours = new SelectList(
                await _context.LoaiTours.AsNoTracking().ToListAsync(),
                "LoaiTourId", "TenLoai", tour.LoaiTourId);
            return View(tour);
        }

        // POST: Sửa tour (có thể đổi ảnh)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaTour(Tour model, IFormFile? hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LoaiTours = new SelectList(
                    await _context.LoaiTours.AsNoTracking().ToListAsync(),
                    "LoaiTourId", "TenLoai", model.LoaiTourId);
                return View(model);
            }

            var tour = await _context.Tours.FindAsync(model.TourId);
            if (tour == null) return NotFound();

            tour.MaTour = model.MaTour;
            tour.TenTour = model.TenTour;
            tour.LoaiTourId = model.LoaiTourId;
            tour.DiemKhoiHanh = model.DiemKhoiHanh;
            tour.DiemDen = model.DiemDen;
            tour.ThoiGian = model.ThoiGian;
            tour.NgayKhoiHanh = model.NgayKhoiHanh;
            tour.GiaNguoiLon = model.GiaNguoiLon;
            tour.GiaTreEm = model.GiaTreEm;
            tour.SoCho = model.SoCho;
            tour.SoChoConLai = Math.Clamp(model.SoChoConLai, 0, model.SoCho);
            tour.MoTaNgan = model.MoTaNgan;
            tour.MoTaChiTiet = model.MoTaChiTiet;
            tour.DichVuBaoGom = model.DichVuBaoGom;
            tour.DichVuKhongBaoGom = model.DichVuKhongBaoGom;
            tour.ChinhSachHuy = model.ChinhSachHuy;
            tour.TrangThai = model.TrangThai;

            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                var newFile = await SaveImageAsync(hinhAnh, tour.HinhAnh);
                tour.HinhAnh = newFile;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachTour));
        }

        // Ẩn/Hiện tour
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAnHien(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            tour.TrangThai = !tour.TrangThai;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachTour));
        }

        // (Tùy chọn) Xóa cứng tour + ảnh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaTour(int id)
        {
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(tour.HinhAnh))
                DeleteImage(tour.HinhAnh);

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachTour));
        }

        // ========================= ĐƠN ĐẶT TOUR =========================

        public async Task<IActionResult> DanhSachDon(string? trangThaiDon)
        {
            var q = _context.DonDatTours
                .Include(d => d.Tour)
                .Include(d => d.KhachHang)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThaiDon))
                q = q.Where(d => d.TrangThaiDon == trangThaiDon);

            ViewBag.TrangThaiDon = trangThaiDon;
            return View(await q.OrderByDescending(d => d.NgayDat).ToListAsync());
        }

        // Cập nhật trạng thái đơn + thanh toán (hoàn chỗ nếu hủy)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThaiDon(int id, string trangThaiDon, string trangThaiThanhToan)
        {
            var don = await _context.DonDatTours
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(d => d.DonDatTourId == id);
            if (don == null) return NotFound();

            if (trangThaiDon == "DaHuy" && don.TrangThaiDon != "DaHuy" && don.Tour != null)
            {
                var so = don.SoNguoiLon + don.SoTreEm;
                don.Tour.SoChoConLai = Math.Clamp(don.Tour.SoChoConLai + so, 0, don.Tour.SoCho);
            }

            don.TrangThaiDon = trangThaiDon;
            don.TrangThaiThanhToan = trangThaiThanhToan;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachDon));
        }

        // ========================= BÁO CÁO =========================

        [HttpGet]
        public async Task<IActionResult> BaoCaoDoanhThu(DateTime? tuNgay, DateTime? denNgay)
        {
            tuNgay ??= DateTime.Today.AddDays(-30);
            denNgay ??= DateTime.Today;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");

            var data = await _context.DonDatTours
                .Include(d => d.Tour)
                .Where(d => d.TrangThaiThanhToan == "DaThanhToan" &&
                            d.NgayDat.Date >= tuNgay.Value.Date &&
                            d.NgayDat.Date <= denNgay.Value.Date)
                .GroupBy(d => new { d.TourId, d.Tour.MaTour, d.Tour.TenTour })
                .Select(g => new BaoCaoDoanhThuViewModel
                {
                    TourId = g.Key.TourId,
                    MaTour = g.Key.MaTour,
                    TenTour = g.Key.TenTour,
                    SoDon = g.Count(),
                    TongSoKhach = g.Sum(x => x.SoNguoiLon + x.SoTreEm),
                    TongDoanhThu = g.Sum(x => x.TongTien)
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .ToListAsync();

            ViewBag.TongDoanhThu = data.Sum(x => x.TongDoanhThu);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> BaoCaoTyLeHuy(DateTime? tuNgay, DateTime? denNgay)
        {
            tuNgay ??= DateTime.Today.AddDays(-30);
            denNgay ??= DateTime.Today;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");

            var data = await _context.DonDatTours
                .Include(d => d.Tour)
                .Where(d => d.NgayDat.Date >= tuNgay.Value.Date &&
                            d.NgayDat.Date <= denNgay.Value.Date)
                .GroupBy(d => new { d.TourId, d.Tour.MaTour, d.Tour.TenTour })
                .Select(g => new TyLeHuyViewModel
                {
                    TourId = g.Key.TourId,
                    MaTour = g.Key.MaTour,
                    TenTour = g.Key.TenTour,
                    TongDon = g.Count(),
                    DonDaHuy = g.Count(x => x.TrangThaiDon == "DaHuy")
                })
                .OrderByDescending(x => x.DonDaHuy)
                .ToListAsync();

            foreach (var x in data)
                x.TyLeHuy = x.TongDon == 0 ? 0 : Math.Round(x.DonDaHuy * 100.0 / x.TongDon, 2);

            ViewBag.TongDon = data.Sum(x => x.TongDon);
            ViewBag.TongDonHuy = data.Sum(x => x.DonDaHuy);
            return View(data);
        }

        // ========================= NHÂN VIÊN =========================

        public async Task<IActionResult> DanhSachNhanVien()
        {
            var list = await _context.NhanViens
                .Include(n => n.TaiKhoan)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult ThemNhanVien() => View(new NhanVienViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemNhanVien(NhanVienViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var tk = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhauHash = PasswordHelper.Hash(model.MatKhau),
                VaiTro = "NhanVien"
            };

            var nv = new NhanVien
            {
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                DiaChi = model.DiaChi,
                TaiKhoan = tk
            };

            _context.NhanViens.Add(nv);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachNhanVien));
        }

        [HttpGet]
        public async Task<IActionResult> SuaNhanVien(int id)
        {
            var nv = await _context.NhanViens.Include(n => n.TaiKhoan)
                         .FirstOrDefaultAsync(n => n.NhanVienId == id);
            if (nv == null) return NotFound();

            return View(new NhanVienViewModel
            {
                NhanVienId = nv.NhanVienId,
                HoTen = nv.HoTen,
                Email = nv.Email,
                SoDienThoai = nv.SoDienThoai,
                DiaChi = nv.DiaChi,
                TenDangNhap = nv.TaiKhoan?.TenDangNhap
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaNhanVien(NhanVienViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var nv = await _context.NhanViens.Include(n => n.TaiKhoan)
                         .FirstOrDefaultAsync(n => n.NhanVienId == model.NhanVienId);
            if (nv == null) return NotFound();

            nv.HoTen = model.HoTen;
            nv.Email = model.Email;
            nv.SoDienThoai = model.SoDienThoai;
            nv.DiaChi = model.DiaChi;

            if (nv.TaiKhoan != null)
            {
                nv.TaiKhoan.TenDangNhap = model.TenDangNhap.Trim();
                if (!string.IsNullOrWhiteSpace(model.MatKhau))
                    nv.TaiKhoan.MatKhauHash = PasswordHelper.Hash(model.MatKhau);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachNhanVien));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaNhanVien(int id)
        {
            var nv = await _context.NhanViens.FirstOrDefaultAsync(n => n.NhanVienId == id);
            if (nv == null) return NotFound();

            var tk = await _context.TaiKhoans.FindAsync(nv.TaiKhoanId);

            _context.NhanViens.Remove(nv);
            if (tk != null) _context.TaiKhoans.Remove(tk);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachNhanVien));
        }

        // ========================= Helpers =========================

        private async Task<string> SaveImageAsync(IFormFile file, string? oldFile = null)
        {
            // wwwroot/images/tours
            var folder = Path.Combine(_env.WebRootPath, "images", "tours");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            if (!string.IsNullOrWhiteSpace(oldFile))
                DeleteImage(oldFile);

            return fileName; // chỉ lưu tên file
        }

        private void DeleteImage(string fileName)
        {
            var fullPath = Path.Combine(_env.WebRootPath, "images", "tours", fileName);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
