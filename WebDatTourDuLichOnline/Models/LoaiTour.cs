using System.Collections.Generic;

namespace WebDatTourDuLichOnline.Models
{
    public class LoaiTour
    {
        public int LoaiTourId { get; set; }
        public string TenLoai { get; set; } = null!;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; } = true;

        public ICollection<Tour>? Tours { get; set; }
    }
}
