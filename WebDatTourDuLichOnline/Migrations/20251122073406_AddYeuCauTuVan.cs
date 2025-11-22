using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebDatTourDuLichOnline.Migrations
{
    /// <inheritdoc />
    public partial class AddYeuCauTuVan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiTour",
                columns: table => new
                {
                    LoaiTourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiTour", x => x.LoaiTourId);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.TaiKhoanId);
                });

            migrationBuilder.CreateTable(
                name: "Tour",
                columns: table => new
                {
                    TourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTour = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenTour = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiTourId = table.Column<int>(type: "int", nullable: false),
                    DiemKhoiHanh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemDen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGian = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKhoiHanh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaNguoiLon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiaTreEm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SoCho = table.Column<int>(type: "int", nullable: false),
                    SoChoConLai = table.Column<int>(type: "int", nullable: false),
                    MoTaNgan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DichVuBaoGom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DichVuKhongBaoGom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChinhSachHuy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tour", x => x.TourId);
                    table.ForeignKey(
                        name: "FK_Tour_LoaiTour_LoaiTourId",
                        column: x => x.LoaiTourId,
                        principalTable: "LoaiTour",
                        principalColumn: "LoaiTourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    KhachHangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.KhachHangId);
                    table.ForeignKey(
                        name: "FK_KhachHang_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    NhanVienId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.NhanVienId);
                    table.ForeignKey(
                        name: "FK_NhanVien_TaiKhoan_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    DanhGiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    KhachHangId = table.Column<int>(type: "int", nullable: false),
                    Diem = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.DanhGiaId);
                    table.ForeignKey(
                        name: "FK_DanhGia_KhachHang_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGia_Tour_TourId",
                        column: x => x.TourId,
                        principalTable: "Tour",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonDatTour",
                columns: table => new
                {
                    DonDatTourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KhachHangId = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNguoiLon = table.Column<int>(type: "int", nullable: false),
                    SoTreEm = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiDon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonDatTour", x => x.DonDatTourId);
                    table.ForeignKey(
                        name: "FK_DonDatTour_KhachHang_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonDatTour_Tour_TourId",
                        column: x => x.TourId,
                        principalTable: "Tour",
                        principalColumn: "TourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    ThanhToanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonDatTourId = table.Column<int>(type: "int", nullable: false),
                    PhuongThuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaGiaoDich = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToan", x => x.ThanhToanId);
                    table.ForeignKey(
                        name: "FK_ThanhToan_DonDatTour_DonDatTourId",
                        column: x => x.DonDatTourId,
                        principalTable: "DonDatTour",
                        principalColumn: "DonDatTourId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauTuVan",
                columns: table => new
                {
                    YeuCauTuVanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonDatTourId = table.Column<int>(type: "int", nullable: true),
                    TourId = table.Column<int>(type: "int", nullable: true),
                    KhachHangId = table.Column<int>(type: "int", nullable: true),
                    Kenh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChuNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauTuVan", x => x.YeuCauTuVanId);
                    table.ForeignKey(
                        name: "FK_YeuCauTuVan_DonDatTour_DonDatTourId",
                        column: x => x.DonDatTourId,
                        principalTable: "DonDatTour",
                        principalColumn: "DonDatTourId");
                    table.ForeignKey(
                        name: "FK_YeuCauTuVan_KhachHang_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangId");
                    table.ForeignKey(
                        name: "FK_YeuCauTuVan_Tour_TourId",
                        column: x => x.TourId,
                        principalTable: "Tour",
                        principalColumn: "TourId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_KhachHangId",
                table: "DanhGia",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_TourId",
                table: "DanhGia",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatTour_KhachHangId",
                table: "DonDatTour",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_DonDatTour_TourId",
                table: "DonDatTour",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_TaiKhoanId",
                table: "KhachHang",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_TaiKhoanId",
                table: "NhanVien",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_DonDatTourId",
                table: "ThanhToan",
                column: "DonDatTourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_LoaiTourId",
                table: "Tour",
                column: "LoaiTourId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauTuVan_DonDatTourId",
                table: "YeuCauTuVan",
                column: "DonDatTourId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauTuVan_KhachHangId",
                table: "YeuCauTuVan",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauTuVan_TourId",
                table: "YeuCauTuVan",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "YeuCauTuVan");

            migrationBuilder.DropTable(
                name: "DonDatTour");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "Tour");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "LoaiTour");
        }
    }
}
