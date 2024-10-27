using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/servicesStylist")]
    [ApiController]
    public class ServicesStylistController : ControllerBase
    {
        private readonly IServicesStylistServices _stylistService;

        public ServicesStylistController(IServicesStylistServices stylistService)
        {
            _stylistService = stylistService;
        }

        [HttpGet("getStylistsByServiceIdAsync/{serviceId}")]
        public async Task<IActionResult> GetStylistsByServiceId(int serviceId)
        {
            var response = await _stylistService.GetStylistsByServiceIdAsync(serviceId);
            return Ok(response); 
        }
    }
}
