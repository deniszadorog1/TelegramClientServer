using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.DTOsHelper;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using User = TelegramLib.MainClasses.User;


namespace TelegramClientServer.SignalRHubs
{
    [Authorize]
    public class MainHub : Hub
    {
/*        public async Task TestConnection()
        {
            var myId = Context.UserIdentifier;
            System.Diagnostics.Debug.WriteLine($"Мой ID в системе SignalR: {myId}");

            await Clients.Caller.SendAsync("ReceiveTextMessage", new User { Name = "System" }, new TextMessage { Text = "Связь есть!" });

            await Clients.All.SendAsync("ReceiveTextMessage", new User { Name = "System" }, new TextMessage { Text = "Видят все!" });
        }*/


        public async Task SendTextMessage(User user, List<Message> messages, User chatter)
        {
            /*            var envelope = new MessageEnvelope
                        {
                            Sender = user,
                            Content = messages,
                            ReceiverId = chatter.Id.ToString()
                        };

                        await MessageBus.PublishAsync(envelope);*/


            await Clients.User(chatter.Id.ToString()).SendAsync("ReceiveTextMessage", messages, user);

        }

        public async Task SendMediaMessage(User user, List<Message> messages, User chatter)
        {
            var senderId = Context.UserIdentifier;

            await Clients.User(chatter.Id.ToString()).SendAsync("ReceiveMediaMessage", messages, user);
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

        public async Task UpdateUserImages(TelegramLib.MainClasses.User user)
        {
            await Clients.All.SendAsync("UpdateUserImages", user);
        }

        public async Task RemoveContact(User logged, User removed)
        {
            await Clients.User(removed.Id.ToString()).SendAsync("RemoveContact", logged, removed);
        }

        public async Task UpdateOnlineStatus(User toUpdate)
        {
            await Clients.All.SendAsync("UpdateOnlineStatus", toUpdate);
        }
        public async Task UpdateLittlePhotoVisInChat(User loggedUser)
        {
            await Clients.All.SendAsync("UpdateLittlePhotoVisInChat", loggedUser);
        }

        public async Task UpdatePagePhoto(User loggedUser)
        {
            await Clients.All.SendAsync("UpdatePagePhoto", loggedUser);
        }

        public async Task UpdateContactBio(User user)
        {
            await Clients.All.SendAsync("UpdateContactBio", user);
        }

        public async Task AddUserImage(User addedImage)
        {
            await Clients.All.SendAsync("AddUserImage", addedImage);
        }

        public async Task EditMessage(int clientId, User logged, EditDTO dto)
        {
            await Clients.User(clientId.ToString()).SendAsync("EditMessage", logged, dto);
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

        public async Task UpdateForwardStatus(User user)
        {
            await Clients.All.SendAsync("UpdateForwardStatus", user);
        }

        public async Task DeleteChat(User loggedUser, User chatter, int clientId)
        {
            await Clients.User(clientId.ToString()).SendAsync("DeleteChat", loggedUser, chatter);
        }

        public async Task UpdateReadStatus(User loggedUser, int clientId)
        {
            await Clients.User(clientId.ToString()).SendAsync("UpdateReadStatus", loggedUser);
        }

        public async Task UpdateChatsControls(User logged, User chatter)
        {
            await Clients.User(chatter.Id.ToString()).SendAsync("UpdateChatsControls", logged);
        }

        public async Task AddShareContactMessage(User logged, 
            User chatter, int id)
        {
            await Clients.User(chatter.Id.ToString())
                .SendAsync("AddShareContactMessage", logged, chatter, id);
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
            TelegramLib.MainClasses.Messages.Message mes, bool isUpdateVis)
        {
            await Clients.User(chatter.Id.ToString())
                .SendAsync("DeleteMessage", logged, mes, isUpdateVis);
        }

        public async Task SendTypingAction(User logged, User chatter)
        {
            await Clients.User(chatter.Id.ToString())
                .SendAsync("SendTypingAction", logged);
        }

        public async Task RemoveManyMessagesByDateTimes(List<DateTime> sentTimes, int loggedUserId, int chatterId)
        {
            await Clients.User(chatterId.ToString()).
                SendAsync("RemoveManyMessagesByDateTimes", sentTimes, loggedUserId);
        }

        public async Task UpdateCachedSettings(int id)
        {
            await Clients.All.SendAsync("UpdateCachedSettings", id);
        }

        public async Task SendAllMessages(List<Message> messages, User sender, User chatter)
        {
            await Clients.User(chatter.Id.ToString()).
                SendAsync("SendAllMessages", messages, sender);
        }
    }

    public class MessageDispatcher : BackgroundService
    {
        private readonly IHubContext<MainHub> _hubContext;

        public MessageDispatcher(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var envelope in MessageBus.SubscribeAsync(stoppingToken))
            {
                await _hubContext.Clients.User(envelope.ReceiverId)
                    .SendAsync("ReceiveTextMessage", envelope.Sender, envelope.Content, stoppingToken);
            }
        }
    }

}
