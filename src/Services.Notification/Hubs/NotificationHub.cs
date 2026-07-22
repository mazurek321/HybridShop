using Microsoft.AspNetCore.SignalR;

namespace HybridShop.Services.Notification.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}