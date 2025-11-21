using System;
using System.Collections.Generic;

namespace WebDatTourDuLichOnline.Models
{
    public class TaiKhoan
    {
        public int TaiKhoanId { get; set; }
        public string TenDangNhap { get; set; } = null!;
        public string MatKhauHash { get; set; } = null!;
        public string VaiTro { get; set; } = "KhachHang";
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public bool TrangThai { get; set; } = true;

        // Quan he
        public ICollection<KhachHang>? KhachHangs { get; set; }
    }
}
