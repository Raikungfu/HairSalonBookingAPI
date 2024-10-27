using AutoMapper;
using BusinessObject;
using BusinessObject.Model;
using BusinessObject.RequestDTO;
using BusinessObject.ResponseDTO;
using Microsoft.Extensions.Configuration;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.VoucherEnum;

namespace Service.Service
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJWTService _jWTService;
        private readonly IUserService _userService;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IJWTService jWTService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jWTService = jWTService;
            _userService = userService;
        }

        public async Task<ResponseDTO> ChangeBookingStatus(RequestDTO.ChangebookingStatusDTO request, int bookingId)
        {
            try
            {
                // Lấy người dùng hiện tại
                var booking = await _unitOfWork.BookingRepository.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Booking not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO vào user

                booking.Status = request.Status;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.BookingRepository.UpdateAsync(booking);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }       

        public async Task<ResponseDTO> CreateBooking(BookingRequestDTO bookingRequest)
        {           
            try
            {
                int customerId;
                // Lấy người dùng hiện tại
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    // Kiểm tra xem UserName đã tồn tại hay chưa
                    var existingUser = await _unitOfWork.UserRepository.GetUserByUserNameAsync(bookingRequest.UserName);
                    if(existingUser != null)
                    {
                        customerId = existingUser.UserId;
                    }
                    else
                    {
                        //Nếu người dùng chưa đăng nhập
                        RegisterRequestDTO registerRequestDTO = new RegisterRequestDTO();
                        registerRequestDTO.userName = bookingRequest.UserName;
                        registerRequestDTO.phone = bookingRequest.Phone;
                        registerRequestDTO.password = bookingRequest.Password;
                        var registUser = _mapper.Map<User>(registerRequestDTO);

                        //Tạo  mới người dùng với 2 field UserName và Password

                        var createAccountResponse = await _userService.Register(registerRequestDTO);
                        if (createAccountResponse.Status != 1)
                        {
                            return new ResponseDTO(Const.FAIL_CREATE_CODE, "Create Account Fail");
                        }
                        customerId = registUser.UserId; // Sử dụng UserId của người dùng mới
                    }
                    
                }
                else
                {
                    // Nếu người dùng đã đăng nhập, sử dụng thông tin của họ
                    customerId = user.UserId;
                }
                double? totalPrice = 0;

                // Kiểm tra ServiceId
                if (bookingRequest.ServiceId == null || !bookingRequest.ServiceId.Any())
                {
                    return new ResponseDTO(400, Const.FAIL_READ_MSG);
                }

                foreach (var serviceId in bookingRequest.ServiceId)
                {
                    var service = await _unitOfWork.HairServiceRepository.GetByIdAsync(serviceId);
                    if (service == null)
                    {
                        return new ResponseDTO(400, $"Service with ID {serviceId} does not exist.");
                    }
                    totalPrice += service.Price;
                }

                // Kiểm tra Voucher nếu có VoucherId
                if (bookingRequest.VoucherId.HasValue)
                {
                    var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(bookingRequest.VoucherId.Value);
                    if (voucher == null)
                    {
                        return new ResponseDTO(400, $"Voucher with ID {bookingRequest.VoucherId} does not exist.");
                    }

                    // Kiểm tra tính hợp lệ của voucher (ngày, trạng thái, v.v.)
                    if (voucher.Status != VoucherStatusEnum.Active /*|| voucher.StartDate > DateTime.Now || voucher.EndDate < DateTime.Now*/)
                    {
                        return new ResponseDTO(400, "Voucher is not valid.");
                    }

                    // Giảm giá từ tổng giá nếu có DiscountAmount
                    if (voucher.DiscountAmount.HasValue)
                    {
                        totalPrice -= voucher.DiscountAmount.Value;
                        if (totalPrice < 0) totalPrice = 0; // Đảm bảo tổng giá không âm
                    }
                }

                // Kiểm tra StylistId
                if (bookingRequest.StylistId == null || !bookingRequest.StylistId.Any())
                {
                    return new ResponseDTO(400, Const.FAIL_READ_MSG);
                }

                foreach (var stylistId in bookingRequest.StylistId)
                {
                    var stylist = await _unitOfWork.UserRepository.GetByIdAsync(stylistId);
                    if (stylist == null || stylist.Role != UserRole.Stylist)
                    {
                        return new ResponseDTO(400, $"Stylist with ID {stylistId} does not exist or is not a stylist.");
                    }
                }

                // kiểm tra schedule có tồn tại không
                var schedule = await _unitOfWork.UserRepository.GetByIdAsync(bookingRequest.ScheduleId);
                if (schedule == null)
                {
                    return new ResponseDTO(400, $"Stylist with ID {bookingRequest.ScheduleId} does not exist.");
                }

                // Tạo một đối tượng Booking mới từ DTO
                var booking = _mapper.Map<Booking>(bookingRequest);
                booking.CustomerId = customerId;
                booking.CreateDate = DateTime.Now;
                booking.Status = BookingStatus.InQueue;
                booking.TotalPrice = totalPrice;
                // Thêm BookingDetails từ ServiceId và StylistId
                booking.BookingDetails = new List<BookingDetail>();
                for (int i = 0; i < bookingRequest.ServiceId.Count; i++)
                {
                    booking.BookingDetails.Add(new BookingDetail
                    {
                        ServiceId = bookingRequest.ServiceId[i],
                        StylistId = bookingRequest.StylistId[i],
                        CreateDate = DateTime.Now,
                        //CreateBy = bookingRequest.CreatedBy
                    });
                }

                // Lưu Booking vào database thông qua UnitOfWork
                var checkUpdate = await _unitOfWork.BookingRepository.CreateBookingAsync(booking);
                if (checkUpdate <= 0)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
                }
                var checkoutRequest = new CheckoutRequestDTO
                {
                    TotalPrice = booking.TotalPrice,
                    CreateDate = DateTime.Now,
                    Description = $"{bookingRequest.UserName} {bookingRequest.Phone}",
                    FullName = bookingRequest.UserName,
                    BookingId = booking.BookingId
                };
                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, checkoutRequest);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, ex);
            }
            
        }
        public async Task<IEnumerable<ViewManageBookingDTO>> GetAllBookingsAsync(int page = 1, int pageSize = 10)
        {
            var bookings = _unitOfWork.BookingRepository.GetAllWithTwoInclude("Payments", "Customer");
            var pagedBookings = bookings.Skip((page - 1) * pageSize).Take(pageSize);
            return _mapper.Map<IEnumerable<ViewManageBookingDTO>>(pagedBookings);
        }

        public async Task<IEnumerable<ViewManageBookingStylistDTO>> GetAllBookingsForStylistAsync(int id)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingByStylistIdAsync(id);
            return _mapper.Map<IEnumerable<ViewManageBookingStylistDTO>>(bookings);
        }

        public async Task<ResponseDTO> GetBookingHistoryOfCurrentUser()
        {
            try
            {
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "User not found.");
                }

                // Lấy danh sách booking của user hiện tại từ repository
                var bookings = await _unitOfWork.BookingRepository.GetBookingHistoryByCustomerIdAsync(user.UserId);

                if (bookings == null || bookings.Count == 0)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No booking history found.");
                }

                // Ánh xạ kết quả từ Booking sang BookingHistoryDTO
                var bookingHistoryDto = _mapper.Map<List<BookingHistoryDTO>>(bookings);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, "Booking history retrieved successfully.", bookingHistoryDto);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

    }
}
