namespace DOANCOSO26.Models
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }

    public class VnPayRequestModel
    {
        public int BookingId { get; set; }

        public string FullName { get; set; }
        public string PhoneNum { get; set; }
        public DateTime Timebooking { get; set; }
        public double TotalPrice { get; set; }
    }
}
