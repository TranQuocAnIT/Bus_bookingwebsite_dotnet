using DOANCOSO26.Helper;
using DOANCOSO26.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace DOANCOSO26.Services
{
    public class VnPaySevices : IVnPaySevices
    {
        private readonly IConfiguration _config;

        public VnPaySevices(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPayRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.TotalPrice * 100).ToString()); // Số tiền thanh toán (đơn vị VND và nhân 100)
            vnpay.AddRequestData("vnp_CreateDate", model.Timebooking.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán vé xe:" + model.BookingId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            var  paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            long vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            long vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode.ToString()
            };
        }
    }
}
