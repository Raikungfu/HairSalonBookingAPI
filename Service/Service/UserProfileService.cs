using AutoMapper;
using BusinessObject;
using BusinessObject.ResponseDTO;
using Service.IService;
using static BusinessObject.RequestDTO.RequestDTO;

namespace Service.Service
{
    public class UserProfileService: IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJWTService _jWTService;
        public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper, IJWTService jWTService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jWTService = jWTService;
        }
        public async Task<ResponseDTO> GetAllUserProfile()
        {
            
            try
            {
                
                var userProfile = await _unitOfWork.UserProfileRepository.getAllUserProfile();

                    var data= _mapper.Map<List<UserProfileDTO>>(userProfile);
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, data);
        
            }
            catch (Exception ex) {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message, new List<UserProfileDTO>());
            }
          
        }

        public async Task<ResponseDTO> GetUserProfileByIdAsync(int id)
        {
            try
            {
                var userProfile = await _unitOfWork.UserProfileRepository.GetByIdAsync(id);

                var result = _mapper.Map<UserProfileDTO>(userProfile);

                if (userProfile == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE,Const.FAIL_READ_MSG,new UserProfileDTO());
                }

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);

            }
            catch (Exception e)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, e.Message, new UserProfileDTO());
            }
        }

        public async Task<ResponseDTO> UpdateUserProfileAsync(UpdateUserProfileDTO request)
        {
            try 
            {
                // Lấy người dùng hiện tại
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User not found !");
                }

                // Lấy hồ sơ người dùng để cập nhật
                var userProfile = user.UserProfile;
                if (userProfile == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "User Profile not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO vào userProfile
                _mapper.Map(request, userProfile);

                userProfile.RegistrationDate = DateTime.Now;
                user.Phone = request.Phone;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.UserProfileRepository.UpdateAsync(userProfile);
                await _unitOfWork.UserRepository.UpdateAsync(user);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG);
            }
            catch(Exception e)
            {
                return new ResponseDTO(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

        }

        public async Task<ResponseDTO> GetCurrentUserProfile()
        {
            try
            {               
                var user = await _jWTService.GetCurrentUserAsync();
                if (user == null)
                {
                    return new ResponseDTO(Const.UNAUTHORIZED_CODE, "User not found.");
                }

                // Lấy hồ sơ người dùng và ánh xạ sang DTO
                var userProfileDto = _mapper.Map<UserProfileDTO>(user.UserProfile);
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
