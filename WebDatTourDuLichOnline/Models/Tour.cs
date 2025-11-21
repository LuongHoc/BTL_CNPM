using System;
using System.Collections.Generic;

namespace WebDatTourDuLichOnline.Models
{
    public class Tour
    {
        public int TourId { get; set; }
        public string MaTour { get; set; } = null!;
        public string TenTour { get; set; } = null!;
        public int LoaiTourId { get; set; }
        public string DiemKhoiHanh { get; set; } = null!;
        public string DiemDen { get; set; } = null!;
        public string? ThoiGian { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
        public decimal GiaNguoiLon { get; set; }
        public decimal? GiaTreEm { get; set; }
        public int SoCho { get; set; }
        public int SoChoConLai { get; set; }
        public string? MoTaNgan { get; set; }
        public string? MoTaChiTiet { get; set; }
        public string? DichVuBaoGom { get; set; }
        public string? DichVuKhongBaoGom { get; set; }
        public string? ChinhSachHuy { get; set; }
        public string? HinhAnh { get; set; }
        public bool TrangThai { get; set; } = true;

        public LoaiTour LoaiTour { get; set; } = null!;
        public ICollection<DonDatTour>? DonDatTours { get; set; }
        public ICollection<DanhGia>? DanhGias { get; set; }
    }
}
