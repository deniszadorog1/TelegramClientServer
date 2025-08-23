using Microsoft.AspNetCore.SignalR;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using User = TelegramLib.MainClasses.User;

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

        public async Task UpdateContact(User updatedContact)
        {
            await Clients.All.SendAsync("UpdateContact", updatedContact);
        }

        public async Task UpdateOnlineStatus(User toUpdate)
        {
            await Clients.All.SendAsync("UpdateOnlineStatus", toUpdate);
        }

        public async Task AddUserImage(User addedImage)
        {
            await Clients.All.SendAsync("AddUserImage", addedImage);
        }

        public async Task ClearChat(int clientId, User chatter)
        {
            await Clients.User(clientId.ToString()).SendAsync("ClearChat", chatter);
        }

        public async Task SetContactPhoneNumberVisibility(bool isVis, 
            TelegramLib.MainClasses.User user)
        {
            await Clients.All.SendAsync("SetContactPhoneNumberVisibility", isVis, user);
        }

        public async Task SetContactLastSeenVisState(User user)
        {
            await Clients.All.SendAsync("SetContactLastSeenVisState", user);
        }
    }
}
