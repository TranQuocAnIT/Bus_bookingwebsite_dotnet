namespace DOANCOSO26.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }  // ID của người gửi (khách hàng hoặc admin)
        public ApplicationUser Sender { get; set; }  // Người gửi
        public string Content { get; set; }  // Nội dung tin nhắn
        public DateTime Timestamp { get; set; }  // Thời gian gửi tin nhắn
    }
}
