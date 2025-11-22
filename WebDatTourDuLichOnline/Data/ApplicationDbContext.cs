using Microsoft.EntityFrameworkCore;
using WebDatTourDuLichOnline.Models;

namespace WebDatTourDuLichOnline.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<LoaiTour> LoaiTours { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<DonDatTour> DonDatTours { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<YeuCauTuVan> YeuCauTuVans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map tên bảng giống SQL
            modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
            modelBuilder.Entity<KhachHang>().ToTable("KhachHang");
            modelBuilder.Entity<LoaiTour>().ToTable("LoaiTour");
            modelBuilder.Entity<Tour>().ToTable("Tour");
            modelBuilder.Entity<DonDatTour>().ToTable("DonDatTour");
            modelBuilder.Entity<DanhGia>().ToTable("DanhGia");
            modelBuilder.Entity<ThanhToan>().ToTable("ThanhToan");
            modelBuilder.Entity<YeuCauTuVan>().ToTable("YeuCauTuVan");
        }
    }
}
