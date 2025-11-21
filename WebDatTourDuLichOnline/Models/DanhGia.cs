using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDatTourDuLichOnline.Models
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        public int DanhGiaId { get; set; }

        public int TourId { get; set; }

        public int KhachHangId { get; set; }

        // Dùng property SoSao nhưng map vào cột DIEM trong database
        [Range(1, 5)]
        [Column("Diem")]
        public int SoSao { get; set; }

        [Required]
        [StringLength(1000)]
        public string NoiDung { get; set; } = null!;

        public DateTime NgayDanhGia { get; set; }

        // Navigation
        public Tour? Tour { get; set; }
        public KhachHang? KhachHang { get; set; }
    }
}
