using System;
using System.Collections.Generic;

namespace WebDatTourDuLichOnline.Models
{
    public class DonDatTour
    {
        public int DonDatTourId { get; set; }
        public string MaDon { get; set; } = null!;
        public int KhachHangId { get; set; }
        public int TourId { get; set; }
        public DateTime NgayDat { get; set; } = DateTime.Now;
        public int SoNguoiLon { get; set; }
        public int SoTreEm { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan";
        public string TrangThaiDon { get; set; } = "ChoXacNhan";
        public string? GhiChu { get; set; }

        public KhachHang KhachHang { get; set; } = null!;
        public Tour Tour { get; set; } = null!;
        public ICollection<ThanhToan>? ThanhToans { get; set; }
    }
}
