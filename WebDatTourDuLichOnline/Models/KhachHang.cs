using System.Collections.Generic;

namespace WebDatTourDuLichOnline.Models
{
    public class KhachHang
    {
        public int KhachHangId { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string? DiaChi { get; set; }

        public int TaiKhoanId { get; set; }
        public TaiKhoan TaiKhoan { get; set; } = null!;

        public ICollection<DonDatTour>? DonDatTours { get; set; }
        public ICollection<DanhGia>? DanhGias { get; set; }
    }
}
