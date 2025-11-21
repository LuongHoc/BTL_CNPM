using System.ComponentModel.DataAnnotations;

namespace WebDatTourDuLichOnline.Models
{
    public class NhanVienViewModel
    {
        public int? NhanVienId { get; set; }

        [Required]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Required]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }   // thêm: bắt buộc khi tạo mới, optional khi sửa

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }
    }
}
