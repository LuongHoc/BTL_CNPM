
# 🧭 HỆ THỐNG ĐẶT TOUR DU LỊCH ONLINE

# Bài tập lớn – Môn Công nghệ phần mềm

# 👥 Thành viên nhóm

- Nguyễn Văn Hoan – K225480106023

- Lương Văn Học – K225480106025

- Hoàng Đức Hội – K225480106085

## Lớp: K58KTP – Trường ĐH Kỹ Thuật Công Nghiệp – Khoa Điện tử – CNTT

# 📌 1. Giới thiệu dự án

Hệ thống đặt tour du lịch online được xây dựng nhằm tự động hóa quy trình đặt tour, tra cứu thông tin du lịch và quản lý khách hàng/tour/doanh thu cho doanh nghiệp.
Dự án giúp:

- Khách hàng tìm kiếm – đặt tour – thanh toán trực tuyến.

- Nhân viên hỗ trợ, quản lý đơn đặt.

- Quản trị viên quản lý tour, nhân viên, doanh thu.

- Đây là bài tập lớn môn Công nghệ phần mềm, thực hiện theo quy trình phân tích – thiết kế – cài đặt – kiểm thử phần mềm.


# 🎯 2. Mục tiêu

- Xây dựng hệ thống đặt tour trực tuyến hiện đại, dễ sử dụng.

- Áp dụng UML, mô hình 3 lớp, cơ sở dữ liệu quan hệ.

- Rèn luyện kỹ năng phân tích – thiết kế phần mềm theo hướng đối tượng.

- Cung cấp sản phẩm demo hoàn chỉnh phục vụ trình chiếu.

# 🧩 3. Các chức năng chính
## 🔹 3.1. Khách hàng

- Đăng ký / đăng nhập / quản lý tài khoản

- Tìm kiếm tour theo: địa điểm, giá, ngày khởi hành, loại tour…

- Xem chi tiết tour: lịch trình, giá, số chỗ còn lại, chính sách

- Đặt tour trực tuyến

- Thanh toán (tiền mặt, chuyển khoản, online)

- Nhận thông báo, xem mã đặt chỗ

- Đánh giá & bình luận tour sau khi đi


## 🔹 3.2. Nhân viên

- Quản lý đơn đặt tour

- Tạo đơn giúp khách hàng

- Tư vấn khách hàng (online/offline)

- Sửa thông tin đơn, cập nhật trạng thái



## 🔹 3.3. Quản trị viên

- Quản lý tour (CRUD)

- Quản lý khách hàng, nhân viên

- Quản lý doanh thu – báo cáo, biểu đồ

- Thống kê tour và khách hàng


# 🏗 4. Kiến trúc hệ thống

- Hệ thống áp dụng mô hình 3 lớp (Three-Layer Architecture):

- Presentation Layer – giao diện web HTML/CSS/JS

- Business Logic Layer – xử lý nghiệp vụ: đặt tour, thanh toán, tính giá, phân quyền

- Data Access Layer – truy xuất SQL Server, quản lý dữ liệu tour/khách hàng/đơn đặt


## 🗄 4.1. Cơ sở dữ liệu gồm các bảng:

KhachHang

NhanVien

TaiKhoan

Tour

LoaiTour

DonDatTour

ThanhToan

DanhGia

YeuCauTuVan



# 🖥 5. Giao diện hệ thống
## 🔸 5.1. Giao diện đăng nhập / đăng ký

- Thiết kế đơn giản, nền sáng, có logo TourVietPlus

- Form đăng nhập: Email + mật khẩu

- Form đăng ký: tên, email, SĐT, tài khoản, mật khẩu

<img width="1853" height="919" alt="image" src="https://github.com/user-attachments/assets/50be6f09-3d17-42d3-a534-6299fd2a730f" />

<img width="1844" height="922" alt="image" src="https://github.com/user-attachments/assets/d8b9d61f-d53f-4a54-9529-5d8726f8c2b2" />


## 🔸 5.2. Giao diện trang chủ

- Banner lớn

- Khung tìm kiếm tour

- Hiển thị số lượng tour, đánh giá, ưu đãi

- Danh mục các tour nổi bật/gần đây

<img width="1837" height="925" alt="image" src="https://github.com/user-attachments/assets/b6e400b9-20d3-40ec-b1ed-5a52923ca2ce" />

<img width="1832" height="917" alt="image" src="https://github.com/user-attachments/assets/e5477131-fb57-4034-87c2-9a5b1bb2db4a" />


## 🔸 5.3. Giao diện đặt tour

- Hiển thị thông tin tour: hành trình, giá, số lượng chỗ

- Chọn số lượng khách

- Tự động tính tổng tiền

<img width="1840" height="924" alt="image" src="https://github.com/user-attachments/assets/eb747e7c-63be-4e6b-8eb7-1e2710c31665" />


## 🔸 5.4. Giao diện thanh toán

- Chọn phương thức thanh toán

- Hiển thị tổng đơn hàng

- Hướng dẫn thanh toán

<img width="1816" height="917" alt="image" src="https://github.com/user-attachments/assets/3622daaf-104d-4972-a48b-791c0552ff3b" />


## 🔸 5.5. Giao diện admin – quản lý doanh thu

- Danh sách tour + số đơn + tổng khách + doanh thu

- Biểu đồ doanh thu theo tour

<img width="1829" height="908" alt="image" src="https://github.com/user-attachments/assets/e8b8b7f2-f8cf-4a4a-9caa-81adc84b92ba" />

<img width="1855" height="909" alt="image" src="https://github.com/user-attachments/assets/622b7d9f-57f3-48c1-a0bb-1371e2ae0dc9" />

## 🔸 5.6. Giao diện admin – quản lý tour

<img width="1846" height="925" alt="image" src="https://github.com/user-attachments/assets/df7782e0-00a4-4e30-b1f0-6cf87ac9341d" />

## 🔸 5.7. Giao diện admin – quản lý nhân viên

<img width="1862" height="925" alt="image" src="https://github.com/user-attachments/assets/abec66d7-e373-41c8-8ba7-555dba89ca76" />

# 🧪 6. Kiểm thử hệ thống

- Kiểm thử đăng ký / đăng nhập

- Kiểm thử tìm kiếm và đặt tour

- Kiểm thử thanh toán thành công/thất bại

- Kiểm thử giao diện quản trị

- Kiểm thử API và dữ liệu



# 📦 7. Công nghệ sử dụng

- Backend: ASP.NET Core / C#

- Frontend: HTML, CSS, JavaScript

- Database: SQL Server

- Tools: SSMS, VS Code/Visual Studio, UML diagrams

# 📊 8. Kết luận & Hướng phát triển

Dự án đã hoàn thành đầy đủ quy trình phân tích – thiết kế – cài đặt – kiểm thử. Hệ thống hoạt động ổn định và đáp ứng nhu cầu đặt tour trực tuyến cơ bản.

## 🔮 Hướng phát triển:

- Gợi ý tour theo xu hướng du lịch

- Gợi ý tour liên quan / tương tự

- Thu thập và phân tích dữ liệu khảo sát: độ tuổi, nghề nghiệp, giới tính…

- Tích hợp AI đề xuất tour phù hợp từng khách hàng

- Hỗ trợ thanh toán quốc tế

- Xây dựng app mobile (iOS/Android)

