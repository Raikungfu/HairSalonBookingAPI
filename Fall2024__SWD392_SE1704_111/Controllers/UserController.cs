using BusinessObject;
using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;
using System.Dynamic;
using System.Globalization;
using System.Security.Claims;
using static BusinessObject.RequestDTO.RequestDTO;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var response = await _userService.Login(request);

                if (response.Status == Const.FAIL_READ_CODE)
                {
                    return Unauthorized(response.Message);
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.Register(registerRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await _userService.GetCurrentUser();

            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        string ConvertToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
                return input;

            return char.ToLower(input[0], CultureInfo.InvariantCulture) + input.Substring(1);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost("changeStatus/{id}")]
        public async Task<IActionResult> ChangeStatusAccount([FromRoute] int id)
        {

            // Gọi service để cập nhật thông tin người dùng
            var response = await _userService.ChangeStatusAccountById(id);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }

        [HttpGet("usersList")]
        public async Task<IActionResult> GetListUser()
        {
            var result = await _userService.GetListUsersAsync();
            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (result.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(result); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }
            return Ok(result);
        }

        [HttpGet("searchAccountByName/{fullName}")]
        public async Task<IActionResult> GetUserByName([FromRoute] string fullName)
        {
            // Gọi service để lấy danh sách người dùng
            var response = await _userService.GetUserByNameAsync(fullName);

            // Trả về phản hồi
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        [HttpGet("GetUserById/{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] int userId)
        {
            // Gọi service để lấy danh sách người dùng
            var response = await _userService.GetUserByIdAsync(userId);

            // Trả về phản hồi
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet("PagingUserList")]
        public async Task<IActionResult> GetUserPaging([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            // Gọi service để lấy danh sách người dùng
            var response = await _userService.GetAllUserPagingAsync(pageNumber,pageSize);

            // Trả về phản hồi
            if (response == null)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        [HttpGet("GetUserByUsernamePaging/{userName}")]
        public async Task<IActionResult> GetUserByUsernamePaging([FromRoute] string userName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            // Gọi service để lấy danh sách người dùng
            var response = await _userService.GetUserPagingByNameAsync(userName, pageNumber, pageSize);

            // Trả về phản hồi
            if (response == null)
            {
                return BadRequest(response); // Trả về mã lỗi nếu không thành công
            }

            return Ok(response); // Trả về mã 200 nếu thành công
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.CreateAccountAsync(createRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updateAccount/{id}")]
        public async Task<IActionResult> UpdateAccountAsync([FromBody] UpdateAccountDTO request, [FromRoute] int id)
        {

            // Kiểm tra xem request có hợp lệ không
            if (request == null)
            {
                return BadRequest(new ResponseDTO(Const.FAIL_READ_CODE, "Invalid request."));
            }

            var response = await _userService.UpdateAccountById(id, request);

            // Kiểm tra kết quả và trả về phản hồi phù hợp
            if (response.Status != Const.SUCCESS_READ_CODE)
            {
                return BadRequest(response); // Trả về mã lỗi 400 với thông báo lỗi từ ResponseDTO
            }

            return Ok(response); // Trả về mã 200 nếu cập nhật thành công với thông tin trong ResponseDTO
        }
    }
}
