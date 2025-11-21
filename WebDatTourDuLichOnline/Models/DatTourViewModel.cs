using System;
using System.ComponentModel.DataAnnotations;

namespace WebDatTourDuLichOnline.Models
{
    public class DatTourViewModel
    {
        public int TourId { get; set; }

        // Thông tin tour (chỉ hiển thị)
        public string TenTour { get; set; } = null!;
        public string DiemKhoiHanh { get; set; } = null!;
        public string DiemDen { get; set; } = null!;
        public DateTime NgayKhoiHanh { get; set; }
        public decimal GiaNguoiLon { get; set; }
        public decimal GiaTreEm { get; set; }
        public int SoChoConLai { get; set; }

        // Người dùng nhập
        [Display(Name = "Số người lớn")]
        public int SoNguoiLon { get; set; }

        [Display(Name = "Số trẻ em")]
        public int SoTreEm { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }
    }
}
