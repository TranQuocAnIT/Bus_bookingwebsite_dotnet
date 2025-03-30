namespace DOANCOSO26.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }  // ID của khách hàng
        public ApplicationUser Customer { get; set; }  // Khách hàng tham gia phòng chat
        public List<ApplicationUser> Admins { get; set; }  // Danh sách Admin trong phòng chat
        public List<Message> Messages { get; set; }  // Các tin nhắn trong phòng chat
    }
}
