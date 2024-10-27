using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/users")]
    [ApiController]
    public class ScheduleUserController : ControllerBase
    {
        private readonly IScheduleUserService _scheduleUserService;

        public ScheduleUserController(IScheduleUserService scheduleUserService)
        {
            _scheduleUserService = scheduleUserService;
        }

        [HttpGet("getAllStylistSchedules")]
        public async Task<IActionResult> GetAllStylistSchedulesAsync()
        {
            var result = await _scheduleUserService.GetListScheduleUserAsync();
            return Ok(result);
        }
    }
}
