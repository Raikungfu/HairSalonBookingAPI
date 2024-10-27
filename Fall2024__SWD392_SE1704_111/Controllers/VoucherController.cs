using BusinessObject;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;
using static BusinessObject.RequestDTO.RequestDTO;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/voucher")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet("listVoucher")]
        public async Task<IActionResult> GetListServices()
        {
            try
            {
                var result = await _voucherService.GetListVoucherAsync();
                return Ok(result);
            }
            catch (Exception) { return BadRequest("Error"); }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("changeVoucherStatus/{id}")]
        public async Task<IActionResult> ChangeStatusAccount([FromRoute] int id)
        {

            // Gọi service để cập nhật
            var response = await _voucherService.ChangeStatusVoucherById(id);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        //[Authorize(Roles = "Manager")]
        [HttpPost("updateVoucher/{id}")]
        public async Task<IActionResult> UpdateVoucherAsync([FromBody] UpdateVoucherDTO request, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            var response = await _voucherService.UpdateVoucherById(id, request);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("CreateVoucher")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDTO createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _voucherService.CreateVoucherAsync(createRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("PagingVoucherList")]
        public async Task<IActionResult> GetVoucherPaging([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            // Gọi service để lấy danh sách người dùng
            var response = await _voucherService.GetAllVoucherPagingAsync(pageNumber, pageSize);

            // Trả về phản hồi
            if (response == null)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        [HttpGet("GetVoucherById/{voucherId}")]
        public async Task<IActionResult> GetVoucherById([FromRoute] int voucherId)
        {
            
            var response = await _voucherService.GetVoucherByIdAsync(voucherId);

            // Trả về phản hồi
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }
    }
}
