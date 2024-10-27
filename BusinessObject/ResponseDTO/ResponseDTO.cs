using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.ReportEnum;

namespace BusinessObject.ResponseDTO
{
    public class ResponseDTO
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ResponseDTO(int status, string? message, object? data = null)
        {
            Status = status;
            Message = message;
            Data = data;
        }


    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class VnPayResponseModel
    {
        public string VnPayResponseCode { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public string BookingId { get; set; }
    }

    public class PaymentType
    {
        public static string VNPAY = "VnPay";
    }

    public class ServicesDTO
    {
        public int ServiceId { get; set; }
        public string? ImageLink { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public TimeSpan? EstimateTime { get; set; }
    }

    public class StylistResponseDTO
    {
        public int StylistId { get; set; }
        public string StylistName { get; set; }
    }

    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }
    }

    public class ScheduleUserDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Phone { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UserProfileDTO
    {
        public int UserProfileId { get; set; }
        public int UserId { get; set; }
        public string? ImageLink { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }

    public class UserProfileUpdatedDTO
    {
        public int UserProfileId { get; set; }
        public int UserId { get; set; }
        public string? ImageLink { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? Phone { get; set; }
    }

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Phone { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
    }

    public class ViewBookingDTO
    {
        public int BookingId { get; set; }
        public double TotalPrice { get; set; }
        public User Customer { get; set; } = null!;
        public Voucher? Voucher { get; set; }
        public BookingStatus Status { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
    }

    public class ViewManageBookingDTO
    {
        public int BookingId { get; set; }
        public double TotalPrice { get; set; }
        public ViewUserDTO Customer { get; set; } = null!;
        public Voucher? Voucher { get; set; }
        public BookingStatus Status { get; set; }
        public ICollection<ViewPaymentDTO> Payments { get; set; }
    }

    public class ViewUserDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public UserStatus? Status { get; set; }
        public UserRole? Role { get; set; }
    }

    public class ViewPaymentDTO
    {
        public int PaymentId { get; set; }
        public double? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? Status { get; set; }
        public int? BookingId { get; set; }
        public int? PaymentTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
    }


    public class UserListDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public UserStatus? Status { get; set; }
        public UserRole? Role { get; set; }
    }


    public class VoucherDTO
    {
        public int VoucherId { get; set; }
        public double? DiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }

    }

    public class ReportDTO
    {
        public string? ReportName { get; set; }
        public string? ReportLink { get; set; }
        public ReportStatusEnum? Status { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public class BookingDetailResponseDTO
        {
            public int BookingDetailID { get; set; }
            public int BookingID { get; set; }
            public int StylistID { get; set; }
            public int ServiceID { get; set; }
            public DateTime? CreateDate { get; set; }
            public string CreateBy { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string UpdateBy { get; set; }
        }
    }
}

