using Microsoft.AspNetCore.SignalR;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using User = TelegramLib.MainClasses.User;

namespace TelegramClientServer.SignalRHubs
{
    public class MainHub : Hub
    {
        public async Task SendTextMessage(User user, TextMessage message, User chatter)
        {
            await Clients.User(chatter.Id.ToString()).SendAsync("ReceiveTextMessage", user, message);
            //await Clients.All.SendAsync("ReceiveTextMessage", user, message);
        }

        public async Task SendMediaMessage(User user, MediaAction message, User chatter)
        {
            //Send to only one 
            await Clients.User(chatter.Id.ToString()).SendAsync("ReceiveMediaMessage", user, message);

            //await Clients.All.SendAsync("ReceiveMediaMessage", user, message);               
        }

        public async Task SendStatMessage(User user, StaticMessage message, User chatter)
        {
            await Clients.User(chatter.Id.ToString()).SendAsync("AddStatMessage", user, message);
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

        public async Task SetPhoneNumVisByExps(User user)
        {
            await Clients.All.SendAsync("SetPhoneNumVisByExps", user);
        }

        public async Task UpdateBirthDate(User user)
        {
            await Clients.All.SendAsync("UpdateBirthDate", user);
        }

        public async Task UpdateContactPhoto(User user)
        {
            await Clients.All.SendAsync("UpdateContactPhoto", user);
        }

        public async Task DeleteChat(User loggedUser, User chatter, int clientId)
        {
            await Clients.User(clientId.ToString()).SendAsync("DeleteChat", loggedUser, chatter);
        }

        public async Task UpdateReadStatus(User loggedUser, int clientId)
        {
            await Clients.User(clientId.ToString()).SendAsync("UpdateReadStatus", loggedUser);
        }

        public async Task AddShareContactMessage(User logged, 
            User chatter, UserContactcs contactToSend)
        {
            await Clients.User(chatter.Id.ToString())
                .SendAsync("AddShareContactMessage", logged, chatter, contactToSend);
        }


        public async Task ReplyMessage(User logged, User chatter, 
            TelegramLib.MainClasses.Messages.TextMessage text)
        {
            await Clients.User(chatter.Id.ToString())
                .SendAsync("ReplyMessage", logged, text);
        }

        public async Task PinMessage(User logged, User chatter,
            TelegramLib.MainClasses.Messages.Message pinned)
        {
            await Clients.User(chatter.Id.ToString())
                .SendAsync("PinMessage", logged, pinned);
        }


        public async Task ForwardMessage(User logged, User chatter,
            TelegramLib.MainClasses.Messages.Message toForward)
        {
            await Clients.User(chatter.Id.ToString()).SendAsync("ForwardMessage", logged, toForward);
        }

        public async Task DeleteMessage(User logged, User chatter, 
            TelegramLib.MainClasses.Messages.Message mes)
        {
            await Clients.User(chatter.Id.ToString())
                .SendAsync("DeleteMessage", logged, mes);
        }
    }
}
