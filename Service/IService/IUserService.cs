using BusinessObject.Model;
using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;

namespace Service.IService
{
    public interface IUserService
    {
        Task<ResponseDTO> GetAll();
        Task<ResponseDTO> Login(LoginRequestDTO request);
        Task<ResponseDTO> Register(RegisterRequestDTO request);
        Task<ResponseDTO> ChangeStatusAccountById( int userId);
        Task<ResponseDTO> GetListUsersAsync();
        Task<ResponseDTO> GetUserByNameAsync(string fullName);
        Task<ResponseDTO> GetUserByIdAsync(int userId);
        Task<PagedResult<User>> GetAllUserPagingAsync(int pageNumber, int pageSize);
        Task<ResponseDTO> CreateAccountAsync(CreateAccountDTO request);
        Task<ResponseDTO> UpdateAccountById(int userId, UpdateAccountDTO request);
        Task<PagedResult<User>> GetUserPagingByNameAsync(string fullName, int pageNumber, int pageSize);
        Task<ResponseDTO> GetCurrentUser();
    }
}
