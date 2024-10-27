using AutoMapper;
using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class JWTService : IJWTService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JWTService(IUnitOfWork unitOfWork, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(User account)
        {
            var tokenSecret = _config["Jwt:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(tokenSecret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Role, account.Role!.ToString()!.Trim())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
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

        public async Task<User> GetCurrentUserAsync()
        {
            // Lấy token từ header Authorization
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                User existUser = null;
                return existUser;
            }

            // Giải mã token để lấy thông tin người dùng
            var claimsPrincipal = ValidateToken(token);

            // Lấy UserId từ claims
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User not found in token."); // Hoặc trả về một ResponseDTO nếu cần
            }

            int userId = int.Parse(userIdClaim.Value);

            // Lấy người dùng hiện tại
            var user = await _unitOfWork.UserProfileRepository.GetUserByCurrentId(userId);
            if (user == null)
            {
                throw new Exception("User not found!"); // Hoặc trả về một ResponseDTO nếu cần
            }

            return user;
        }

        public async Task<User> GetCurUserAsync()
        {
            // Lấy token từ header Authorization
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                User existUser = null;
                return existUser;
            }

            // Giải mã token để lấy thông tin người dùng
            var claimsPrincipal = ValidateToken(token);

            // Lấy UserId từ claims
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new Exception("User not found in token."); // Hoặc trả về một ResponseDTO nếu cần
            }

            int userId = int.Parse(userIdClaim.Value);

            // Lấy người dùng hiện tại
            var user = await _unitOfWork.UserRepository.GetUserByCurrentId(userId);
            if (user == null)
            {
                throw new Exception("User not found!"); // Hoặc trả về một ResponseDTO nếu cần
            }

            return user;
        }
    }
}
