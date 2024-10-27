using AutoMapper;
using BusinessObject.Model;
using BusinessObject;
using BusinessObject.RequestDTO;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ReportEnum;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace Service.Service
{
    [AllowAnonymous]
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJWTService _jWTService;
        public ReportService(IUnitOfWork unitOfWork, IConfiguration config, IMapper mapper, IHttpContextAccessor httpContextAccessor, IJWTService jWTService)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _jWTService = jWTService;
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // Không cho phép chênh lệch thời gian
            };

            return tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        }

        public async Task<ResponseDTO> GetListReportAsync()
        {
            try
            {
                var report = await _unitOfWork.ReportRepository.GetAllAsync();

                if (report == null || !report.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Empty List");
                }

                var result = _mapper.Map<List<ReportDTO>>(report);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception e)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, e.Message);
            }
        }

        public async Task<ResponseDTO> CreateReportAsync(CreateReportDTO request)
        {
            try
            {
                // Lấy token từ header Authorization
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Token is missing.");
                }

                // Giải mã token để lấy thông tin người dùng
                var claimsPrincipal = ValidateToken(token);

                // Lấy UserId từ claims
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "User not found in token.");
                }

                int userId = int.Parse(userIdClaim.Value);

                if (await _unitOfWork.BookingRepository.GetByIdAsync((int)request.BookingId) == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Booking not found");
                }

                // Lấy người dùng hiện tại
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User not found !");
                }

                // Sử dụng AutoMapper 
                var report = _mapper.Map<Report>(request);

                report.StylistId = userId;
                report.CreateBy = user.UserName;
                report.CreateDate = DateTime.Now;
                report.UpdateBy = user.UserName;
                report.UpdateDate = DateTime.Now;

                // Lưu các thay đổi vào cơ sở dữ liệu
                var result = await _unitOfWork.ReportRepository.CreateReportAsync(report);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message, ex);
            }
        }

        public async Task<ResponseDTO> UpdateReportAsync(UpdateReportDTO request, int reportId)
        {
            try
            {
                // Lấy token từ header Authorization
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Token is missing.");
                }

                // Giải mã token để lấy thông tin người dùng
                var claimsPrincipal = ValidateToken(token);

                // Lấy UserId từ claims
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "User not found in token.");
                }

                int userId = int.Parse(userIdClaim.Value);

                var report = await _unitOfWork.ReportRepository.GetReportById(reportId);
                if (report == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "report not found");
                }

                // Lấy người dùng hiện tại
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User not found !");
                }

                _mapper.Map(request, report);

                report.StylistId = userId;
                report.CreateDate = DateTime.Now;
                report.UpdateDate = DateTime.Now;
                report.UpdateBy = user.UserName;

                // Lưu các thay đổi vào cơ sở dữ liệu
                var result = _unitOfWork.ReportRepository.UpdateReportAsync(report);

                if (result == null)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, "Update Report Failed");
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Update Report Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> ChangeReportStatusAsync(RemoveReportDTO request, int reportId)
        {
            try
            {
                // Lấy token từ header Authorization
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Token is missing.");
                }

                // Giải mã token để lấy thông tin người dùng
                var claimsPrincipal = ValidateToken(token);

                // Lấy UserId từ claims
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "User not found in token.");
                }

                int userId = int.Parse(userIdClaim.Value);

                var report = await _unitOfWork.ReportRepository.GetReportById(reportId);
                if (report == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "report not found");
                }

                // Lấy người dùng hiện tại
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User not found !");
                }

                // Sử dụng AutoMapper 
                _mapper.Map(request, report);
                report.UpdateDate = DateTime.Now;
                report.UpdateBy = user.UserName;

                // Lưu các thay đổi vào cơ sở dữ liệu
                var result = _unitOfWork.ReportRepository.UpdateReportAsync(report);

                if (result == null)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, "Update Report status Failed");
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Update Report status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}

