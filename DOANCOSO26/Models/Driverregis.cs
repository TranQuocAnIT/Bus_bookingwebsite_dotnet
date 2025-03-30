using System;
using System.ComponentModel.DataAnnotations;

namespace DOANCOSO26.Models
{
    public class Driverregis
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string Email { get; set; }

        // Bạn có thể thay đổi kiểu dữ liệu tương ứng với ứng dụng của bạn
        public string DriverId { get; set; }
        public ApplicationUser Driver { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số giấy phép lái xe.")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày cấp giấy phép lái xe.")]
        public DateTime LicenseIssueDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập cơ quan cấp giấy phép lái xe.")]
        public string LicenseIssuingAuthority { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày hết hạn giấy phép lái xe.")]
        [CustomValidation(typeof(Driverregis), "ValidateLicenseExpiryDate")]
        public DateTime LicenseExpiryDate { get; set; }

        // Các thuộc tính khác...

        public string GPLX1ImageUrl { get; set; }
        public List<GPLXImage> GPLX1Image { get; set; }
        public string GPLX2ImageUrl { get; set; }
        public List<GPLX2Image> GPLX2Image { get; set; }
        public string CCCDNumber { get; set; }
        public string CCCD1ImageUrl { get; set; }
        public List<CCCD1Image> CCCD1Image { get; set; }
        public string CCCD2ImageUrl { get; set; }
        public List<CCCD2Image> CCCD2Image { get; set; }
        public ApproveStatus ApproveStatus { get; set; }

        // Phương thức kiểm tra tùy chỉnh
        public static ValidationResult ValidateLicenseExpiryDate(DateTime licenseExpiryDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as Driverregis;
            if (instance != null && licenseExpiryDate <= instance.LicenseIssueDate)
            {
                return new ValidationResult("Ngày hết hạn phải lớn hơn ngày cấp.");
            }
            return ValidationResult.Success;
        }
    }
}
