using AutoMapper;
using BusinessObject;
using BusinessObject.Model;
using BusinessObject.Paging;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.IService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.UserProfileDTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJWTService _jWTService;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IJWTService jWTService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jWTService = jWTService;
        }
        public async Task<ResponseDTO> GetAll()
        {
            try
            {
                var listUser = await _unitOfWork.UserRepository.GetAllAsync();

                if (listUser == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No users found.");
                }
                else
                {
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, listUser);
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> Login(LoginRequestDTO request)
        {
            try
            {
                var account = _unitOfWork.UserRepository.GetAll()
                                .FirstOrDefault(x => x.UserName!.ToLower() == request.userName.ToLower()
                                && x.Password == request.password);

                if (account == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Invalid credentials.");
                }

                if (account.Status != UserStatus.Active)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Your account is not active. Please contact support.");
                }
                //var loginResponse = _mapper.Map<LoginResponse>(account);
                var jwt = _jWTService.GenerateToken(account);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, jwt);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }



        public async Task<ResponseDTO> Register(RegisterRequestDTO request)
        {
            try
            {
                if (await _unitOfWork.UserRepository.ExistsByNameAsync(request.userName))
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "The username is already taken. Please choose a different username.");
                }

                //AutoMapper from RegisterRequestDTO => User
                var user = _mapper.Map<User>(request);

                user.CreateDate = DateTime.UtcNow;
                user.Status = UserStatus.Active;
                user.Role = UserRole.Customer;

                await _unitOfWork.UserRepository.CreateAsync(user);
                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "User registered successfully", request);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }



        public async Task<ResponseDTO> ChangeStatusAccountById(int userId)
        {
            try
            {
                // Lấy người dùng hiện tại
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO vào user

                user.Status = user.Status == UserStatus.Active ? UserStatus.Banned : UserStatus.Active;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.UserRepository.UpdateAsync(user);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetListUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetAllAsync();

                if (users == null || !users.Any())
                {
                    return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Empty List");
                }

                var result = _mapper.Map<List<UserListDTO>>(users);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception e)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, e.Message);
            }
        }

        public async Task<ResponseDTO> GetUserByNameAsync(string fullName)
        {
            try
            {
                // Gọi repository để lấy danh sách người dùng theo tên
                var users = await _unitOfWork.UserRepository.GetUserByNameAsync(fullName);

                // Kiểm tra nếu danh sách rỗng
                if (users == null || !users.Any())
                {
                    return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "No users found with the given name.");
                }

                // Sử dụng AutoMapper để ánh xạ các entity sang DTO
                var result = _mapper.Map<List<UserListDTO>>(users);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu xảy ra
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetUserByIdAsync(int userId)
        {
            try
            {
                // Gọi repository để lấy danh sách người dùng theo tên
                var users = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);

                // Kiểm tra nếu danh sách rỗng
                if (users == null)
                {
                    return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "No users found with the ID");
                }

                // Sử dụng AutoMapper để ánh xạ các entity sang DTO
                var result = _mapper.Map<UserListDTO>(users);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu xảy ra
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<PagedResult<User>> GetAllUserPagingAsync(int pageNumber, int pageSize)
        {
            try
            {
                // Lấy người dùng hiện tại
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new Exception();
                }

                var userList = _unitOfWork.UserRepository.GetUsersExcludingCurrentUserAndRoleAsync(user.UserId, user.Role);
                if (userList == null)
                {
                    throw new Exception();
                }
                var userQuery = userList.AsQueryable();

                return await Paging.GetPagedResultAsync(userQuery, pageNumber, pageSize);
            }
            catch (Exception)
            {
                return new PagedResult<User>();
            }
        }

        public async Task<PagedResult<User>> GetUserPagingByNameAsync(string userName, int pageNumber, int pageSize)
        {
            try
            {
                // Lấy người dùng hiện tại
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new Exception();
                }

                // Gọi repository để lấy danh sách người dùng theo tên
                var users =  _unitOfWork.UserRepository.GetUsersByNameExcludingCurrentUserAndRoleAsync(user.UserId, user.Role, userName);

                // Kiểm tra nếu danh sách rỗng
                if (users == null)
                {
                    throw new Exception();
                }

                var usersQuery = users.AsQueryable();

                return await Paging.GetPagedResultAsync(usersQuery, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu xảy ra
                return new PagedResult<User>();
            }
        }

        public async Task<ResponseDTO> CreateAccountAsync(CreateAccountDTO request)
        {
            try
            {
                if (await _unitOfWork.UserRepository.ExistsByNameAsync(request.userName))
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "The username is already taken. Please choose a different username.");
                }

                //AutoMapper from RegisterRequestDTO => User
                var user = _mapper.Map<User>(request);

                user.CreateDate = DateTime.UtcNow;
                user.Status = UserStatus.Active;
                user.Role = request.RoleId;

                await _unitOfWork.UserRepository.CreateAsync(user);
                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "User registered successfully", request);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateAccountById(int userId, UpdateAccountDTO request)
        {
            try
            {
                // Lấy người dùng hiện tại
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO vào user
                _mapper.Map(request, user);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.UserRepository.UpdateAsync(user);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetCurrentUser()
        {
            try
            {               
                var user = await _jWTService.GetCurUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.UNAUTHORIZED_CODE, "User not found.");
                }

                // Lấy hồ sơ người dùng và ánh xạ sang DTO
                var userProfileDto = _mapper.Map<ViewUserDTO>(user);
                userProfileDto.Phone = user.Phone;

                return new ResponseDTO(Const.SUCCESS_READ_CODE, "User profile retrieved successfully.", userProfileDto);
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                return new ResponseDTO(Const.UNAUTHORIZED_CODE, "Token has expired.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
