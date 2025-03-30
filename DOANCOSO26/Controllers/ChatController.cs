using Microsoft.AspNetCore.Mvc;
using DOANCOSO26.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DOANCOSO26.Controllers
{
    [Route("chat")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatController(IHubContext<ChatHub> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }

        // Hiển thị chatbox cho khách hàng
        [HttpGet("customer")]
        public IActionResult CustomerChat()
        {
            return View();
        }

        // Hiển thị các cuộc trò chuyện của Admin
        [HttpGet("admin")]
        public IActionResult AdminChat()
        {
            return View();
        }

        // Gửi tin nhắn từ khách hàng đến tất cả admin
        [HttpPost("sendMessageToAdmins")]
        public async Task<IActionResult> SendMessageToAdmins(string message)
        {
            await _chatHubContext.Clients.Group("AdminGroup").SendAsync("ReceiveMessage", message);
            return Ok();
        }
    }
}
