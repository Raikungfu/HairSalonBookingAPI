using BusinessObject;
using BusinessObject.RequestDTO;
using BusinessObject.ResponseDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VNPAY_CS_ASPX;
using static BusinessObject.RequestDTO.RequestDTO;

namespace Service.Service
{

    public class OrderInfo
    {
        public long OrderId { get; set; }
        public long Amount { get; set; }
        public string OrderDesc { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }

        public long PaymentTranId { get; set; }
        public string BankCode { get; set; }
        public string PayStatus { get; set; }


    }
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;

        public PaymentService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext httpContext, VnPaymentRequestModel model)
        {
            string vnp_ReturnUrl = _config["VnPay:PaymentBackReturnUrl"];
            string vnp_Url = _config["VnPay:BaseURL"];
            string vnp_TmnCode = _config["VnPay:TmnCode"];
            string vnp_HashSecret = _config["VnPay:HashSecret"];

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (model.TotalPrice * 100).ToString());

            if (model.VnPayMethod == VnPayMethod.ATM)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (model.VnPayMethod == VnPayMethod.CreditCard)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + model.BookingId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", model.BookingId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public VnPayResponseModel PaymentExecute(IQueryCollection query)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            string vnp_HashSecret = _config["VnPay:HashSecret"];
            var response = vnpay.GetResponseData(vnp_HashSecret);
            return new VnPayResponseModel();
        }
    }
}