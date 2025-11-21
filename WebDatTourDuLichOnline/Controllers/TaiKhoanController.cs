using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebDatTourDuLichOnline.Data;
using WebDatTourDuLichOnline.Models;

namespace WebDatTourDuLichOnline.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /TaiKhoan/DangKy
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        // POST: /TaiKhoan/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // kiểm tra trùng tên đăng nhập
            var existed = await _context.TaiKhoans
                .AnyAsync(t => t.TenDangNhap == model.TenDangNhap);
            if (existed)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            // tạo tài khoản
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                MatKhauHash = PasswordHelper.Hash(model.MatKhau),
                VaiTro = "KhachHang",
                TrangThai = true
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // tạo khách hàng gắn với tài khoản
            var kh = new KhachHang
            {
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                DiaChi = model.DiaChi,
                TaiKhoanId = taiKhoan.TaiKhoanId
            };

            _context.KhachHangs.Add(kh);
            await _context.SaveChangesAsync();

            // Sau khi đăng ký xong -> chuyển tới trang đăng nhập
            return RedirectToAction("DangNhap");
        }

        // GET: /TaiKhoan/DangNhap
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /TaiKhoan/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var passwordHash = PasswordHelper.Hash(model.MatKhau);

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t =>
                    t.TenDangNhap == model.TenDangNhap &&
                    t.MatKhauHash == passwordHash &&
                    t.TrangThai == true);

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            // Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, taiKhoan.TaiKhoanId.ToString()),
                new Claim(ClaimTypes.Name, taiKhoan.TenDangNhap),
                new Claim(ClaimTypes.Role, taiKhoan.VaiTro)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.GhiNho
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /TaiKhoan/DangXuat
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        // Đơn đặt tour của tài khoản hiện tại
        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> DonCuaToi()
        {
            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
            {
                return RedirectToAction("DangNhap", new { returnUrl = Url.Action("DonCuaToi") });
            }

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
            {
                return Content("Không tìm thấy thông tin khách hàng.");
            }

            var dons = await _context.DonDatTours
                .Include(d => d.Tour)
                .Where(d => d.KhachHangId == khachHang.KhachHangId)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View(dons);
        }

        // Hủy đơn (khách được hủy khi đơn đang chờ xác nhận)
        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDon(int id)
        {
            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
            {
                return RedirectToAction("DangNhap", new { returnUrl = Url.Action("DonCuaToi") });
            }

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
            {
                return Content("Không tìm thấy thông tin khách hàng.");
            }

            var don = await _context.DonDatTours
                .Include(d => d.Tour)
                .FirstOrDefaultAsync(d => d.DonDatTourId == id && d.KhachHangId == khachHang.KhachHangId);

            if (don == null)
            {
                return NotFound();
            }

            if (don.TrangThaiDon == "ChoXacNhan")
            {
                // hoàn lại chỗ
                var soNguoi = don.SoNguoiLon + don.SoTreEm;
                if (don.Tour != null)
                {
                    don.Tour.SoChoConLai += soNguoi;
                }
                don.TrangThaiDon = "DaHuy";

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DonCuaToi");
        }
        // =================== HỒ SƠ CÁ NHÂN ===================
        [Authorize(Roles = "KhachHang")]
        [HttpGet]
        public async Task<IActionResult> HoSo()
        {
            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
            {
                return RedirectToAction("DangNhap", new { returnUrl = Url.Action("HoSo") });
            }

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
            {
                return Content("Không tìm thấy thông tin khách hàng.");
            }

            var model = new HoSoViewModel
            {
                HoTen = khachHang.HoTen,
                Email = khachHang.Email,
                SoDienThoai = khachHang.SoDienThoai,
                DiaChi = khachHang.DiaChi
            };

            return View(model);
        }

        [Authorize(Roles = "KhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoSo(HoSoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
            {
                return RedirectToAction("DangNhap", new { returnUrl = Url.Action("HoSo") });
            }

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(k => k.TaiKhoanId == taiKhoanId);

            if (khachHang == null)
            {
                return Content("Không tìm thấy thông tin khách hàng.");
            }

            khachHang.HoTen = model.HoTen;
            khachHang.Email = model.Email;
            khachHang.SoDienThoai = model.SoDienThoai;
            khachHang.DiaChi = model.DiaChi;

            await _context.SaveChangesAsync();

            ViewBag.ThongBao = "Cập nhật hồ sơ thành công.";
            return View(model);
        }
        // =================== ĐỔI MẬT KHẨU ===================
        [Authorize]
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoanIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (taiKhoanIdClaim == null)
            {
                return RedirectToAction("DangNhap", new { returnUrl = Url.Action("DoiMatKhau") });
            }

            int taiKhoanId = int.Parse(taiKhoanIdClaim.Value);

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TaiKhoanId == taiKhoanId);

            if (taiKhoan == null)
            {
                return Content("Không tìm thấy tài khoản.");
            }

            var hashCu = PasswordHelper.Hash(model.MatKhauCu);
            if (taiKhoan.MatKhauHash != hashCu)
            {
                ModelState.AddModelError("MatKhauCu", "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            taiKhoan.MatKhauHash = PasswordHelper.Hash(model.MatKhauMoi);
            await _context.SaveChangesAsync();

            ViewBag.ThongBao = "Đổi mật khẩu thành công.";
            return View(new DoiMatKhauViewModel());
        }

        // Khi không đủ quyền
        public IActionResult KhongDuQuyen()
        {
            return View();
        }
    }
}
