using BusinessObject.ResponseDTO;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using static BusinessObject.RequestDTO.RequestDTO;
using Service.IService;
using Service.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize(Roles = "Stylist")]
        [HttpPost("changeBookingStatus/{id}")]
        public async Task<IActionResult> ChangeBookingStatus([FromRoute] int id, [FromBody] ChangebookingStatusDTO request)
        {
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            // Gọi service để cập nhật thông tin người dùng
            var response = await _bookingService.ChangeBookingStatus(request, id);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        [HttpPost("createBooking")]
        public async Task<IActionResult> Booking([FromBody] BookingRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            var response = await _bookingService.CreateBooking(request);

            if (response.Status != Const.SUCCESS_CREATE_CODE)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetBookingHistoryOfCurrentUser()
        {
            try
            {
                var response = await _bookingService.GetBookingHistoryOfCurrentUser();

                if (response.Status == Const.SUCCESS_READ_CODE)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings(int page = 1, int pageSize = 10)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(page, pageSize);
            return Ok(bookings);
        }

        [Authorize(Roles = "Stylist")]
        [HttpGet("current-stylist")]
        public async Task<IActionResult> GetAllBookingsForCurrentStylist()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(id == null || !int.TryParse(id, out int stylistId))
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            var bookings = await _bookingService.GetAllBookingsForStylistAsync(stylistId);
            return Ok(bookings);
        }
    }
}
