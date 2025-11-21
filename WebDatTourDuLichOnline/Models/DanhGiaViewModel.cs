using System.ComponentModel.DataAnnotations;

namespace WebDatTourDuLichOnline.Models
{
    public class DanhGiaViewModel
    {
        public int TourId { get; set; }

        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5.")]
        [Display(Name = "Số sao")]
        public int SoSao { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá.")]
        [Display(Name = "Nội dung đánh giá")]
        public string NoiDung { get; set; } = null!;
    }
}
