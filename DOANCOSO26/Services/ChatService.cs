using DOANCOSO26.Hubs;
using Microsoft.AspNetCore.SignalR;

public class ChatService
{
    private readonly IHubContext<ChatHub> _chatHubContext;

    public ChatService(IHubContext<ChatHub> chatHubContext)
    {
        _chatHubContext = chatHubContext;
    }

    public async Task SendMessageToAdmins(string message)
    {
        await _chatHubContext.Clients.Group("AdminGroup").SendAsync("ReceiveMessage", message);
    }

    public async Task SendMessageToCustomer(string customerId, string message)
    {
        await _chatHubContext.Clients.Client(customerId).SendAsync("ReceiveMessage", message);
    }
}
