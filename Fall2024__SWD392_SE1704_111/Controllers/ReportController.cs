using BusinessObject.ResponseDTO;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;
using static BusinessObject.RequestDTO.RequestDTO;
using Microsoft.AspNetCore.Authorization;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            this._reportService = reportService;
        }

        //[Authorize(Roles = "Stylist")]
        [HttpPost("createReport")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDTO request)
        {
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            // Gọi service để tạo report
            var response = await _reportService.CreateReportAsync(request);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        //[Authorize(Roles = "Stylist")]
        [HttpPost("updateReport/{reportId}")]
        public async Task<IActionResult> UpdateReport([FromBody] UpdateReportDTO request, [FromRoute] int reportId)
        {
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            // Gọi service để tạo report
            var response = await _reportService.UpdateReportAsync(request, reportId);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        //[Authorize(Roles = "Stylist")]
        [HttpPost("changeReportStatus/{reportId}")]
        public async Task<IActionResult> RemoveReport([FromRoute] int reportId, [FromBody] RemoveReportDTO request)
        {
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            // Gọi service để tạo report
            var response = await _reportService.ChangeReportStatusAsync(request, reportId);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        //[Authorize(Roles = "Stylist, Manager")]
        [HttpGet("reportList")]
        public async Task<IActionResult> GetListUser()
        {
            var result = await _reportService.GetListReportAsync();
            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }
            return Ok(result);
        }
    }
}
