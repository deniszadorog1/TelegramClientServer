using Microsoft.AspNetCore.SignalR;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;

namespace TelegramClientServer.SignalRHubs
{
    public class MainHub : Hub
    {
        public async Task SendTextMessage(User user, TextMessage message)
        {
            await Clients.All.SendAsync("ReceiveTextMessage", user, message);
        }

        public async Task SendMediaMessage(UserChat user, MediaAction message)
        {
            await Clients.All.SendAsync("ReceiveMediaMessage", user, message);
        }

        public async Task AddContact(User user, User contact)
        {
            await Clients.User(user.Id.ToString()).SendAsync("AddContact", user, contact);
            //await Clients.All.SendAsync("AddContact", user, contact);
        }

    }
}
