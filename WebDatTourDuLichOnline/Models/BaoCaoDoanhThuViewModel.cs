using System;

namespace WebDatTourDuLichOnline.Models
{
    public class BaoCaoDoanhThuViewModel
    {
        public int TourId { get; set; }
        public string MaTour { get; set; } = null!;
        public string TenTour { get; set; } = null!;

        // Số đơn đã thanh toán trong khoảng thời gian
        public int SoDon { get; set; }

        // Tổng số khách (người lớn + trẻ em)
        public int TongSoKhach { get; set; }

        // Tổng doanh thu (chỉ tính đơn đã thanh toán)
        public decimal TongDoanhThu { get; set; }
    }
}
