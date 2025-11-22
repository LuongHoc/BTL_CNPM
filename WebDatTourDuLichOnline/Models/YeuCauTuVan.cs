using System;

namespace WebDatTourDuLichOnline.Models
{
    public class YeuCauTuVan
    {
        public int YeuCauTuVanId { get; set; }

        // Liên quan đến đơn / tour / khách
        public int? DonDatTourId { get; set; }
        public int? TourId { get; set; }
        public int? KhachHangId { get; set; }

        // Thông tin yêu cầu
        public string Kenh { get; set; }          // "DatTour", "NhanVienTaoDon", "ChiTietTour"
        public string NoiDung { get; set; }       // Nội dung cần tư vấn
        public DateTime ThoiGianTao { get; set; } // Thời gian tạo
        public string TrangThai { get; set; }     // "Moi", "DangLienHe", "DaXong"
        public string? GhiChuNhanVien { get; set; }

        // Điều hướng (nav properties)
        public DonDatTour? DonDatTour { get; set; }
        public Tour? Tour { get; set; }
        public KhachHang? KhachHang { get; set; }
    }
}
