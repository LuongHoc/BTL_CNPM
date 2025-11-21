using System.ComponentModel.DataAnnotations;

namespace WebDatTourDuLichOnline.Models
{
    public class HoSoViewModel
    {
        [Required]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = null!;

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
    }
}
