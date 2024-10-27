using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using static BusinessObject.RequestDTO.RequestDTO;

namespace Fall2024__SWD392_SE1704_111.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;


        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("checkout")]
        public IActionResult CheckOut([FromBody] VnPaymentRequestModel checkoutRequest)
        {
            if (checkoutRequest.VnPayMethod == VnPayMethod.ATM || checkoutRequest.VnPayMethod == VnPayMethod.CreditCard)
            {
                return Ok(new { paymentUrl = _paymentService.CreatePaymentUrl(HttpContext, checkoutRequest) });
            }
            return BadRequest("Invalid payment method");
        }

        [HttpPost("PaymentCallBack")]
        public IActionResult PaymentCallBack()
        {
            var response = _paymentService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                return BadRequest("Payment failed or was canceled");
            }
            return Ok("Payment succeeded");
        }
    }
}
