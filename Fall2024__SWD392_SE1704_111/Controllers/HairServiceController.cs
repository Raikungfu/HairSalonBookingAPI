using BusinessObject.ResponseDTO;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;
using static BusinessObject.RequestDTO.RequestDTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/hairservice")]
    [ApiController]
    public class HairServiceController : ControllerBase
    {
        private readonly IHairServiceService _serviceManagementService;
       

        public HairServiceController(IHairServiceService serviceManagementService)
        {
            _serviceManagementService = serviceManagementService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetListServices()
        {
            var result = await _serviceManagementService.GetListServicesAsync();
            return Ok(result);
        }

        [HttpGet("getServices/{id}")]
        public async Task<IActionResult> GetServiceById([FromRoute]int id)
        {
            var services = await _serviceManagementService.GetServiceByIdAsync(id);
            return Ok(services);
        }

        [HttpGet("{serviceName}")]
        public async Task<IActionResult> GetServiceByName([FromRoute] string serviceName)
        {
            // Gọi service để lấy danh sách người dùng
            var response = await _serviceManagementService.GetServiceByNameAsync(serviceName);

            // Trả về phản hồi
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDTO request)
        {
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            // Gọi service để tạo report
            var response = await _serviceManagementService.CreateServiceAsync(request);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        [HttpPost("update/{serviceId}")]
        public async Task<IActionResult> UpdateReport([FromBody] UpdateServiceDTO request, [FromRoute] int serviceId)
        {
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            // Gọi service để tạo report
            var response = await _serviceManagementService.UpdateServiceAsync(request, serviceId);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        [HttpPost("remove/{serviceId}")]
        public async Task<IActionResult> RemoveReport([FromRoute] int serviceId, [FromBody] RemoveServiceDTO request)
        {
            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            // Gọi service để tạo report
            var response = await _serviceManagementService.ChangeServiceStatusAsync(request, serviceId);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }
    }
}

