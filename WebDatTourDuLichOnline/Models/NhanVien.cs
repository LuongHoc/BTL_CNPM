using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebDatTourDuLichOnline.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        public int NhanVienId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        public int TaiKhoanId { get; set; }

        public TaiKhoan TaiKhoan { get; set; }
    }
}
