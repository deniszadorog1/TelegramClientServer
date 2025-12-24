using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.UserControls;
using Newtonsoft.Json.Bson;
using TelegramLib.Models;

using UserChat = TelegramLib.MainClasses.UserChat;
using User = TelegramLib.MainClasses.User;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using TelegramVisualPart.UserControls.SettingsControls;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Windows;
using TelegramLib.MainClasses.DTOsHelper;
using Microsoft.Xaml.Behaviors.Core;
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.Services
{
    public static class SignalRService
    {
        //SignalR -> ApiService -> Controller -> DbService

        private static HubConnection? _connection;
        private static TelSystem? _system;


        public static event Action<User, TextMessage>? TextMessageReceived;
        public static event Action<User, MediaAction>? MediaMessageReceived;
        public static event Func<User, StaticMessage, Task>? StatMessageReceived;

        public static event Action<User>? UpdateContactDel;
        public static event Action<User>? UpdateOnlineStatusDel;
        public static event Action<User>? UpdateUserImage;
        public static event Action<User>? ClearChatDel;

        public static event Action<bool, User>? SetContactPhoneNumberVisibilityDel;
        public static event Action<User>? SetContactLastSeenVisStateDel;
        public static event Action<User>? SetPhoneNumVisByExpsDel;
        public static event Action<User>? UpdateBirthDateDel;
        public static event Action<User>? UpdateContactPhotoDel;
        public static event Action<User>? UpdateContactBioDel;
        public static event Action<User>? UpdateForwardStatusDel;

        public static event Func<User, Message, Task>? EditMessageDel;

        public static event Action<User>? DeleteChat;

        public static event Action<User>? UpdateReadStatus;
        public static event Func<User, UserContactcs, Task>? SetShareContactMessage;


        public static event Action<User, TextMessage>? ReplyMesAction;
        public static event Action<User, Message>? ForwardMesAction;
        public static event Action<User, Message, bool>? DeleteMessageByIdDel;
        public static event Action<User, Message>? ToPinMessageDel;

        public static event Func<User, Task>? RemoveContactDel;
        public static event Func<User, Task>? AddContactDel;

        public static event Func<User, Task>? UpdateChatsControlsDel;
        public static event Action<User>? SendTypingActionDel;

        public static void SetSystem(TelSystem system)
        {
            _system = system;
        }

        public static async Task SetBasicSignalRConnetion()
        {
            await SetSignalRConnection();
        }

        public static string GetUserId(HubConnectionContext connection)
        {
            if (connection.GetHttpContext().Request.Headers.TryGetValue("userId", out var userId))
            {
                return userId;
            }
            return null;
        }

        public static async Task DisconnectAsync()
        {
            if (_connection is not null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
            ClearAllEvents();
        }

        private static void ClearAllEvents()
        {
            TextMessageReceived = null;
            MediaMessageReceived = null;
            StatMessageReceived = null;

            UpdateContactDel = null;
            UpdateOnlineStatusDel = null;
            UpdateUserImage = null;
            ClearChatDel = null;
            SetContactPhoneNumberVisibilityDel = null;
            SetContactLastSeenVisStateDel = null;
            SetPhoneNumVisByExpsDel = null;
            UpdateBirthDateDel = null;
            UpdateContactPhotoDel = null;
            DeleteChat = null;
            DeleteMessageByIdDel = null;
            EditMessageDel = null;

            UpdateReadStatus = null;
            SetShareContactMessage = null;
            RemoveContactDel = null;
        }

        public static async Task SetSignalRConnection()
        {
            if (_connection is not null) await DisconnectAsync();

            _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7164/chatHub", options =>
            {
                options.Headers.Add("userId", _system.LoggedUser.Id.ToString());
            }).Build();

            _connection.On<User, TextMessage>("ReceiveTextMessage", (user, message) =>
            {
                //Need to be: senderUSERid, receiverUSERid, message
                //sender already added message to hiss chatDB
                //there receiver should add message to RECEIVERSdb

                //sender converts into contact id -> find chat in db by this params -> add message to chat

                TextMessageReceived?.Invoke(user, message);

                return;
            });

            _connection.On<User, MediaAction>("ReceiveMediaMessage", (sender, message) =>
            {
                MediaMessageReceived?.Invoke(sender, message);

                return;
            });

            //Add contacts
            _connection.On<User, User>("AddContact", async (user, toAdd) =>
            {
                //Add conatct in system
                UserContactcs contact = new UserContactcs(-1, toAdd.Name, toAdd.Surname, toAdd.Login, toAdd.BirthDay,
                    toAdd.BIO, toAdd.PhoneNumber, toAdd.LastSeenOnline, true, toAdd.UserImages, null, true);

                contact.ContactUserId = toAdd.Id;
                //add cotact in db
                await ApiService.AddContact(_system.LoggedUser.Id, contact);
                contact = await ApiService.GetLastUserContact(_system.LoggedUser.Id);

                if (!_system.IsContactExistByUserId(contact.ContactUserId)) _system.Contacts.Add(contact);

                //Add chat in DB
                await ApiService.AddNewChat(_system.LoggedUser.Id, contact.ContactUserId);

                UserChat? chatToAdd = await ApiService.GetChatByUserAndSenderId(_system.LoggedUser.Id, contact.ContactUserId);
                _system.AddChat(chatToAdd);

                AddContactDel?.Invoke(toAdd);
                return;
            });

            //Update contacts (BIO, username, etc)  //MAYBE
            _connection.On<User>("UpdateContact", (updatedContact) =>
            {
                UpdateContactDel?.Invoke(updatedContact);
                return;
            });

            //update online status
            _connection.On<User>("UpdateOnlineStatus", (updatedContact) =>
            {
                UpdateOnlineStatusDel?.Invoke(updatedContact);
                return;
            });

            _connection.On<User>("UpdateContactBio", (updatedContact) =>
            {
                UpdateContactBioDel?.Invoke(updatedContact);
                return;
            });

            _connection.On<User>("AddUserImage", (addedImage) =>
            {
                //Add it to contacts system
                UserContactcs? contact = _system.Contacts.FirstOrDefault(x => x.ContactUserId == addedImage.Id);
                if (contact is null) return;

                //Set
                contact.UserImages = addedImage.UserImages;

                //Update in opened chat
                UpdateUserImage?.Invoke(addedImage);

                //update in opened 
                //update

                //Update in view(If need)
            });

            _connection.On<User>("ClearChat", (chatter) =>
            {
                ClearChatDel?.Invoke(chatter);
            });

            _connection.On<bool, User>("SetContactPhoneNumberVisibility", (isVisivle, updatedUser) =>
            {
                SetContactPhoneNumberVisibilityDel?.Invoke(isVisivle, updatedUser);
            });

            _connection.On<User>("SetContactLastSeenVisState", (user) =>
            {
                SetContactLastSeenVisStateDel?.Invoke(user);
            });

            _connection.On<User>("SetPhoneNumVisByExps", (user) =>
            {
                SetPhoneNumVisByExpsDel?.Invoke(user);
            });

            _connection.On<User>("UpdateBirthDate", (user) =>
            {
                UpdateBirthDateDel?.Invoke(user);
            });

            _connection.On<User>("UpdateContactPhoto", (user) =>
            {
                UpdateContactPhotoDel?.Invoke(user);
            });

            _connection.On<User>("UpdateForwardStatus", (user) =>
            {
                UpdateForwardStatusDel?.Invoke(user);
            });

            _connection.On<User, User>("DeleteChat", (loggedUser, chatter) =>
            {
                //logged user is now chatter 
                //Send logged user become chatter in new one
                DeleteChat?.Invoke(loggedUser);
            });

            _connection.On<User>("UpdateReadStatus", (loggedUser) =>
            {
                UpdateReadStatus?.Invoke(loggedUser);
            });

            _connection.On<User, User, UserContactcs>("AddShareContactMessage", 
                (logged, chatter, contactToSend) =>
            {
                //chatter is now logged
                //logged is now chatter
                SetShareContactMessage?.Invoke(logged, contactToSend);
            });


            _connection.On<User, TextMessage>("ReplyMessage", (logged, message) =>
            {
                //to reply
                ReplyMesAction?.Invoke(logged, message);
            });

            _connection.On<User, Message>("PinMessage", (logged, message) =>
            {
                //to pin
                ToPinMessageDel?.Invoke(logged, message);
            });

            _connection.On<User, Message, bool>("DeleteMessage", (logged, mes, isUpdateVis) =>
            {
                DeleteMessageByIdDel?.Invoke(logged, mes, isUpdateVis);
            });

            _connection.On<User, Message>("ForwardMessage", (logged, mes) =>
            {
                ForwardMesAction?.Invoke(logged, mes);
            });

            _connection.On<User, StaticMessage>("AddStatMessage", (logged, mes) =>
            {
                StatMessageReceived?.Invoke(logged, mes);
            });

            _connection.On<User, EditDTO>("EditMessage", (logged, dto) =>
            {
                //Turn into message

                Message mes = dto.TextMes is not null ? dto.TextMes : dto.MediaMes;
                if (mes is null) return;
                
                EditMessageDel?.Invoke(logged, mes);
            });

            _connection.On<User, User>("RemoveContact", (logged, removed) =>
            {
                RemoveContactDel?.Invoke(logged);
            });

            _connection.On<User>("UpdateChatsControls", (toUpdate) =>
            {
                UpdateChatsControlsDel?.Invoke(toUpdate);
                //To update chat controls if last message was removed or changed(Text).
            });

            _connection.On<User>("SendTypingAction", (logged) =>
            {
                //Set typing action;

                SendTypingActionDel?.Invoke(logged);

            });
            await _connection.StartAsync();
        }

        public static async Task SendTextMessage(User sender, TextMessage message, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendTextMessage", sender, message, chatter);
        }

        public static async Task SendMediaMessage(User sender, MediaAction message, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendMediaMessage", sender, message, chatter);
        }

        public static async Task AddStatMessage(User sender, StaticMessage message, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendStatMessage", sender, message, chatter);
        }

        public static async Task AddContact(User user, User contact)
        {
            await VisHelper.UpdateStatesWithSignalR(_system);


            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("AddContact", user, contact);
        }

        public static async Task UpdateContact(User updatedUser)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateContact", updatedUser);
        }

        public static async Task DeleteContact(User loggedUser, User removed)
        {
            await VisHelper.UpdateStatesWithSignalR(_system);

            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("RemoveContact", loggedUser, removed);
        }

        public static async Task UpdateOnlineStatus(User toUpdate)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateOnlineStatus", toUpdate);
        }

        public static async Task AddUserImage(User addedImage)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("AddUserImage", addedImage);
        }

        public static async Task ClearChat(int clientId, User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("ClearChat", clientId, user);
        }

        public static async Task SetUserPhonenumberVisibility(bool IsVisisble, 
            TelegramLib.MainClasses.User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SetContactPhoneNumberVisibility", IsVisisble, user);
        } 

        public static async Task SetContactLastSeenVisState(User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SetContactLastSeenVisState", user);
        }
        
        public static async Task SetPhoneNumVisByExps(User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SetPhoneNumVisByExps", user);
        }

        public static async Task UpdateBirtDate(User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateBirthDate", user);
        }

        public static async Task UpdateContactPhotoVis(User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateContactPhoto", user);
        }

        public static async Task UpdateContactBioVis(User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateContactBio", user);
        }

        public static async Task UpdateContactForwardStatus(User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateForwardStatus", user);
        }

        public static async Task DeleteChatMethod(User loggedUser, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("DeleteChat", loggedUser, chatter, chatter.Id);
        }

        public static async Task UpdateReadStatusMethod(User loggedUser, User chatter)
        {
            if (_connection?.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateReadStatus", loggedUser, chatter.Id);
        }

        public static async Task AddShareContactMessage(User logged, User chatter,
            UserContactcs contactToSend)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("AddShareContactMessage", logged, chatter, contactToSend);
        }

        public static async Task DeleteMessageById(User logged, User chatter,
            TelegramLib.MainClasses.Messages.Message mes, bool isUpdateVis = true)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("DeleteMessage", logged, chatter, mes, isUpdateVis);
        }

        public static async Task PinMessage(User logged, User chatter,
            TelegramLib.MainClasses.Messages.Message toPin)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("PinMessage", logged, chatter, toPin);
        } 


        public static async Task ForwardMessage(User logged, User chatter,
            TelegramLib.MainClasses.Messages.Message toForward)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("ForwardMessage", logged, chatter, toForward);
        }
        
        public static async Task SendReplyMessage(User logged, User chatter,
            TelegramLib.MainClasses.Messages.TextMessage replyMes)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("ReplyMessage", logged, chatter, replyMes);
        }

        public static async Task EditMessage(User logged, User chatter, 
            TelegramLib.MainClasses.Messages.Message toEdit)
        {
            EditDTO dto = GetEditDTO(toEdit);

            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("EditMessage", chatter.Id, logged, dto);
        }

        private static EditDTO GetEditDTO(TelegramLib.MainClasses.Messages.Message mes)
        {
            EditDTO res = new EditDTO();

            if (mes is TextMessage text) res.TextMes = text;
            else if (mes is MediaAction media) res.MediaMes = media;

            return res;
        }

        public static async Task UpdateChatsControls(User logged, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateChatsControls", logged, chatter);
        }

        public static async Task SendTypingAction(User logged, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendTypingAction", logged, chatter);
        }
    }
}
