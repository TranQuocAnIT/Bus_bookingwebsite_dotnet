using DOANCOSO26.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DOANCOSO26.Hubs
{
    public class ChatHub : Hub
    {
        // Gửi tin nhắn đến tất cả các Admin
        public async Task SendMessageToAdmins(string message)
        {
            var userRole = Context.User.IsInRole(Roles.Role_Admin);
            if (userRole)
            {
                // Đảm bảo rằng chỉ có các Admin nhận tin nhắn
                await Clients.Group("AdminGroup").SendAsync("ReceiveMessage", message);
            }
            else
            {
                // Khách hàng chỉ gửi tin nhắn đến các admin
                await Clients.Group("AdminGroup").SendAsync("ReceiveMessage", message);
            }
        }

        // Gửi tin nhắn từ Admin về một khách hàng cụ thể
        public async Task SendMessageToCustomer(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }

        // Tham gia vào nhóm chat (chatroom) với tên nhóm là ID của khách hàng
        public async Task JoinRoom(string customerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, customerId);
        }

        // Admin tham gia nhóm chat để trả lời khách hàng
        public async Task JoinAdminRoom(string customerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
        }
    }
}
