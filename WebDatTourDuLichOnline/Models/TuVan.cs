using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDatTourDuLichOnline.Models
{
    public class TuVan
    {
        public int TuVanId { get; set; }

        // Liên quan khách (có thể để trống nếu khách lẻ)
        public int? KhachHangId { get; set; }
        [MaxLength(150)] public string? TenKhach { get; set; }
        [MaxLength(20)] public string SoDienThoai { get; set; } = default!;
        [MaxLength(150)] public string? Email { get; set; }

        // Quan tâm tour
        public int? TourId { get; set; }
        [MaxLength(200)] public string? ChuDe { get; set; }
        public string? NoiDung { get; set; }

        // Thuộc tính nghiệp vụ
        [MaxLength(20)] public string Kenh { get; set; } = "Quay"; // Quay/Zalo/Facebook/Phone/Web
        [MaxLength(20)] public string MucDoUuTien { get; set; } = "TrungBinh"; // Thap/TrungBinh/Cao
        [MaxLength(20)] public string TrangThai { get; set; } = "Moi"; // Moi/DangTuVan/DaHen/DaChotDon/KhongNhuCau

        public DateTime? ThoiGianHenLai { get; set; }

        // Gán cho NV
        public int NhanVienId { get; set; }
        public NhanVien NhanVien { get; set; } = default!;

        // Theo dõi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Tour? Tour { get; set; }
        public KhachHang? KhachHang { get; set; }

        public ICollection<TuVanLog> LichSu { get; set; } = new List<TuVanLog>();
    }

    public class TuVanLog
    {
        public int TuVanLogId { get; set; }
        public int TuVanId { get; set; }
        public TuVan TuVan { get; set; } = default!;

        public string NoiDung { get; set; } = default!;
        public DateTime ThoiGian { get; set; } = DateTime.UtcNow;

        [MaxLength(150)] public string? NhanVienThucHien { get; set; }
    }
}
