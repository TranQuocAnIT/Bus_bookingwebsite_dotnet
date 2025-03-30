using DOANCOSO26.Models;

namespace DOANCOSO26.Services
{
    public interface IVnPaySevices
    {
        string CreatePaymentUrl(HttpContext context ,VnPayRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
