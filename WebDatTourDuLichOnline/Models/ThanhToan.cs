using System;

namespace WebDatTourDuLichOnline.Models
{
    public class ThanhToan
    {
        public int ThanhToanId { get; set; }
        public int DonDatTourId { get; set; }
        public string PhuongThuc { get; set; } = null!;
        public decimal SoTien { get; set; }
        public DateTime ThoiGianThanhToan { get; set; } = DateTime.Now;
        public string? MaGiaoDich { get; set; }
        public string TrangThai { get; set; } = "ThanhCong";

        public DonDatTour DonDatTour { get; set; } = null!;
    }
}
