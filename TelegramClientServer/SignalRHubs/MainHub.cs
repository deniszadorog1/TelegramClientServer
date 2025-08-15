using Microsoft.AspNetCore.SignalR;

namespace TelegramClientServer.SignalRHubs
{
    public class MainHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {

            await Clients.All.SendAsync("ReciveMessage", user, message);
        }

    }
}
