using System;

namespace WebDatTourDuLichOnline.Models
{
    public class TyLeHuyViewModel
    {
        public int TourId { get; set; }
        public string MaTour { get; set; } = null!;
        public string TenTour { get; set; } = null!;

        public int TongDon { get; set; }
        public int DonDaHuy { get; set; }

        // Tỷ lệ hủy (%)
        public double TyLeHuy { get; set; }
    }
}
