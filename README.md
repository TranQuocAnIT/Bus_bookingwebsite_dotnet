Hệ thống website bán vé xe khách liên tỉnh được xây dựng bằng C# .NET (ASP.NET Core MVC) và SQL Server, giúp khách hàng đặt vé trực tuyến dễ dàng. Hệ thống hỗ trợ quản lý tuyến xe, đặt vé, thanh toán và theo dõi lịch trình xe chạy.
2. Chức năng chính
👤 Đối với khách hàng
✅ Đăng ký, đăng nhập (Email, SĐT, Google, Facebook)
✅ Tìm kiếm tuyến xe (Chọn điểm đi, điểm đến, ngày khởi hành)
✅ Xem chi tiết chuyến xe (Giờ khởi hành, số ghế trống, giá vé)
✅ Chọn ghế và đặt vé (Giao diện chọn ghế trực quan)
✅ Thanh toán trực tuyến (VNPay, Momo, thẻ ngân hàng)
✅ Tra cứu vé đã đặt (Xem mã vé, trạng thái vé)
✅ Hủy/đổi vé (Theo chính sách hoàn vé)

🚌 Đối với nhà xe (quản trị viên, nhân viên bán vé)
✅ Quản lý tuyến xe (Thêm, sửa, xóa tuyến)
✅ Quản lý lịch trình xe chạy (Thời gian, tài xế, điểm dừng)
✅ Quản lý đặt vé (Xác nhận thanh toán, cập nhật trạng thái vé)
✅ Báo cáo doanh thu (Thống kê doanh thu theo ngày, tháng, năm)

3. Công nghệ sử dụng
![image](https://github.com/user-attachments/assets/993c77a0-b3de-4a84-90a4-13078eb16470)
Backend: ASP.NET Core MVC, C#

Frontend: HTML, CSS, JavaScript, Bootstrap

Database: SQL Server

Realtime: SignalR (Cập nhật trạng thái ghế tức thời)

Thanh toán: Tích hợp VNPay, Momo API

Xác thực: ASP.NET Identity


Trang chủ là nơi cung cấp cái nhìn tổng quan giúp người dùng dễ dàng tương tác với hệ thống. Tại đây, người dùng có thể xem thông tin về nhà xe, vé xe và sử dụng công cụ tìm kiếm nhanh chóng, thuận tiện.
 ![image](https://github.com/user-attachments/assets/3a1c700b-8d36-42d4-9a3e-c760e4ae6292)

Hình 3 1 giao diện trang chủ (1)
 ![image](https://github.com/user-attachments/assets/3ff52c31-098c-4f5d-9719-0acc1f41213e)

Hình 3 2 giao diện trang chủ (2)
 ![image](https://github.com/user-attachments/assets/9c6e54ba-54bd-496e-8f1d-b7ca0592c6fd)

Hình 3 3 giao diện trang chủ (3)
3.3.2 Đăng kí  và đăng nhâp
Trang đăng ký giúp người dùng, đặc biệt là khách vãng lai, tạo tài khoản để trở thành khách hàng và đăng nhập vào hệ thống. Người dùng chỉ cần điền thông tin cá nhân và nhấn nút "Đăng ký"
 ![image](https://github.com/user-attachments/assets/7346dbba-d765-42d6-b42d-6830a822e177)

Hình 3 4 Trang đăng kí
 ![image](https://github.com/user-attachments/assets/4e1d6538-ee14-40da-8645-67cff3aa57d7)

Hình 3 5 trang đăng nhập
3.2.3 Trang tìm kiếm và kết quả 
 ![image](https://github.com/user-attachments/assets/058d43c4-f008-49f9-adcc-a307fef7b03b)

Hình 3 6 Trang tìm kiếm và kết quả
Trang đặt vé là một chức năng quan trọng trong hệ thống, cung cấp đầy đủ thông tin về các chuyến xe. Người dùng có thể lựa chọn vị trí ghế ngồi và thực hiện đặt vé một cách thuận tiện.
3.2.4 Trang thông tin chuyến xe và chọn ghế 
Trang giao diện chọn ghế và thông tin xe cung cấp cho người dùng các lựa chọn ghế ngồi trong chuyến xe, đồng thời hiển thị thông tin chi tiết về chuyến đi như giờ khởi hành, điểm đến, và các thông tin liên quan khác. Người dùng có thể dễ dàng chọn ghế và kiểm tra các thông tin trước khi xác nhận việc đặt vé.
 ![image](https://github.com/user-attachments/assets/d368158d-f220-49fe-9f90-bba6488600e9)

Hình 3 7 Trang thông tin chuyến xe và chọn ghế
 ![image](https://github.com/user-attachments/assets/7e114889-39f3-4887-830b-1a29ccc7320a)

Hình 3 8 Popup sơ đồ chọn ghế
Sơ đồ ghế là giao diện hiển thị các vị trí ghế có sẵn trên chuyến xe, giúp người dùng dễ dàng lựa chọn ghế ngồi. Thông thường, sơ đồ ghế sẽ được thiết kế theo dạng mô phỏng không gian trong xe, với các ghế được đánh dấu trạng thái: có sẵn, đã được đặt, hoặc không thể chọn. 

3.2.5 trang xác nhận thông tin 
Trang xác nhận thông tin giúp người dùng kiểm tra lại tất cả các thông tin đặt vé của mình để đảm bảo tính chính xác trước khi tiến hành thanh toán. Nếu cần, người dùng có thể quay lại để chỉnh sửa thông tin cho đúng.
 ![image](https://github.com/user-attachments/assets/181afd56-88a9-4c07-8221-b6575da690ae)

Hình 3 9 Trang xác nhận thông tin
 ![image](https://github.com/user-attachments/assets/0f25cf27-5d60-4373-aef3-91aeff795c98)

 
Hình 3 10  Giao diện đơn đặt hàng thành công
Giao diện vé xe hiển thị thông tin chi tiết về chuyến đi, bao gồm tên nhà xe, giờ khởi hành, điểm đi và điểm đến, cùng với số ghế đã chọn. Người dùng cũng có thể thấy giá vé và các thông tin liên quan như mã vé hoặc mã QR để tiện kiểm tra khi lên xe. Giao diện này giúp việc kiểm tra và quản lý vé trở nên dễ dàng và nhanh chóng.
3.2.6 Trang giao diện chi tiết vé
 ![image](https://github.com/user-attachments/assets/2ad83d7c-cec3-4ed5-b7a2-d25d0738d33e)

Hình 3 11 Chi tiết vé
3.2.7 một số trang giao diện khác 
Trang tuyến đường cung cấp thông tin chi tiết về các tuyến xe, bao gồm các điểm xuất phát, điểm đến và các trạm dừng trên hành trình. Người dùng có thể dễ dàng tra cứu các tuyến đường có sẵn, thời gian di chuyển dự kiến và các lựa chọn vé phù hợp.
 ![image](https://github.com/user-attachments/assets/971ad24f-66e1-4ead-abcc-5fbd44e52211)

Hình 3 12 trang tuyến đường
 ![image](https://github.com/user-attachments/assets/9c13a61b-ee8a-4567-bfd9-1b57dabb5588)

Hình 3 13 Trang lịch sử đặt vé
Trang lịch sử đặt vé hiển thị tất cả các vé mà người dùng đã đặt trong quá khứ. Tại đây, người dùng có thể xem chi tiết các chuyến xe đã đặt, bao gồm thông tin về chuyến đi, số vé, tình trạng thanh toán và các tùy chọn như hủy vé hoặc in lại vé.
 ![image](https://github.com/user-attachments/assets/73d96f1a-df3e-429b-9af5-202aa8bb2424)

Hình 3 14 trang tra cưu vé
 
