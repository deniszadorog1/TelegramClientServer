using Microsoft.AspNetCore.SignalR;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;

namespace TelegramClientServer.SignalRHubs
{
    public class MainHub : Hub
    {
        public async Task SendTextMessage(UserChat user, TextMessage message)
        {

            await Clients.All.SendAsync("ReceiveTextMessage", user, message);
        }

        public async Task SendMediaMessage(UserChat user, MediaAction message)
        {

            await Clients.All.SendAsync("ReceiveMediaMessage", user, message);
        }

    }
}
