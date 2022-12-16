using Microsoft.AspNetCore.SignalR;
using MockDoor.Shared.Models.General;

namespace MockDoor.Services.Hubs
{
    public class RequestHub : Hub
    {
        public async Task SendRequest(DateTime time, HttpRequestDto message)
        {
            await Clients.All.SendAsync("ReceiveMessage", time, message);
        }
    }
}
