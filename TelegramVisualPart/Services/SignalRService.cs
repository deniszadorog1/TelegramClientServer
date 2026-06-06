using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection; 
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
using Microsoft.Extensions.Hosting;
using System.Windows;
using System.Net.NetworkInformation;
using System.Formats.Asn1;

namespace TelegramVisualPart.Services
{
    public static class SignalRService
    {
        //SignalR -> ApiService -> Controller -> DbService

        private static HubConnection? _connection;
        public static TelSystem? _system;

        public static event Func<List<Message>, User, Task>? TextMessageReceived;
        public static event Func<List<Message>, User, Task>? MediaMessageReceived;

        public static event Func<User, StaticMessage, Task>? StatMessageReceived;

        public static event Func<List<Message>, User, Task>? SendAllMessagesDel;  

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
        public static event Func<User, int, Task>? SetShareContactMessage;


        public static event Action<User, TextMessage>? ReplyMesAction;
        public static event Action<User, Message>? ForwardMesAction;
        public static event Action<User, Message, bool>? DeleteMessageByIdDel;
        public static event Action<User, Message>? ToPinMessageDel;
        public static event Action<User>? UpdateLittlePhotoVisInChatDel;

        public static event Func<User, Task>? RemoveContactDel;
        public static event Func<User, Task>? AddContactDel;

        public static event Func<User, Task>? UpdateChatsControlsDel;
        public static event Action<User>? SendTypingActionDel;

        public static event Action<List<DateTime>, int>? RemoveManyMessagesDel;

        public static event Action<TelegramLib.MainClasses.User>? UpdateUserImagesDel;

        public static event Action<HashSet<int>>? UpdateChatsAfterSched;
        public static event Action<User>? UpdatePagePhotoDel;

        public static event Func<int, Task>? UpdateCachedDel;

        private static bool _isChatEventsAreSet = false;
        public static bool GetIsChatEventsAreSet() => _isChatEventsAreSet;
        public static void ChangeIsChatEventsAreSet(bool isSet)
        {
            _isChatEventsAreSet = isSet;
        }

        public static void SetSystem(TelSystem system)
        {
            _system = system;
        }

        public static async Task SetBasicSignalRConnection()
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

            string connection = ApiService.GetConnectionString().Trim().TrimEnd('/');

            //MessageBox.Show($"{connection}   ___   {str}");

            /*            _connection = new HubConnectionBuilder()
                            .WithUrl($"{connection}/chatHub", options =>
                            {
                                options.Headers["ngrok-skip-browser-warning"] = "true";
                                options.AccessTokenProvider = () => Task.FromResult(_system.Token);
                            })
                            .WithAutomaticReconnect()
                            .Build();*/

            _connection = new HubConnectionBuilder()
                .WithUrl($"{connection}/chatHub", options =>
                {
                    options.Headers["ngrok-skip-browser-warning"] = "true";
                    options.AccessTokenProvider = () => Task.FromResult(_system.Token);
                })
                .WithAutomaticReconnect()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                })
                .Build();

            //MessageBox.Show("Its done!");


            _connection.On<List<Message>, User>("ReceiveTextMessage", (messages, user) =>
            {
                TextMessageReceived?.Invoke(messages, user);
                return;
            });

            _connection.On<List<Message>, User>("ReceiveMediaMessage", (messages, sender) =>
            {
                MediaMessageReceived?.Invoke(messages, sender);
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

            _connection.On<User, User, int>("AddShareContactMessage",
                (logged, chatter, id) =>
            {
                //chatter is now logged
                //logged is now chatter
                SetShareContactMessage?.Invoke(logged, id);
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

            _connection.On<List<DateTime>, int>("RemoveManyMessagesByDateTimes", (sentTimes, loggedUserId) =>
            {
                RemoveManyMessagesDel?.Invoke(sentTimes, loggedUserId);
            });

            _connection.On<TelegramLib.MainClasses.User>(
                "UpdateUserImages", (user) =>
            {
                UpdateUserImagesDel?.Invoke(user);
            });


            _connection.On<HashSet<int>>("UpdateAfterSchedMessages", (chatIds) =>
            {
                UpdateChatsAfterSched?.Invoke(chatIds);
            });

            _connection.On<TelegramLib.MainClasses.User>("UpdateLittlePhotoVisInChat", (loggedUser) =>
            {
                UpdateLittlePhotoVisInChatDel?.Invoke(loggedUser);
            });

            _connection.On<User>("UpdatePagePhoto", (loggedUser) =>
            {
                UpdatePagePhotoDel?.Invoke(loggedUser);
            });

            _connection.On<int>("UpdateCachedSettings", (id) =>
            {
                UpdateCachedDel?.Invoke(id);
            });

            _connection.On<List<Message>, User>("SendAllMessages", (messages, user) =>
            {
                SendAllMessagesDel?.Invoke(messages, user);
            });

            await _connection.StartAsync();
        }

        public static async Task SendTextMessage(User sender, List<Message> message, User chatter)
        {
/*            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("TestConnection");
            }*/

            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendTextMessage", sender, message, chatter);
        }

        public static async Task SendMediaMessage(User sender, List<Message> message, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendMediaMessage", sender, message, chatter);

            /*            if (_connection.State == HubConnectionState.Connected)
                            await _connection.InvokeAsync("SendMediaMessage", sender, message, chatter);*/
        }

        public static async Task AddShareContactMessage(User logged, User chatter,
                int id)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("AddShareContactMessage", logged, chatter, id);
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
            if (_connection is null) return;
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateOnlineStatus", toUpdate);
        }

        public static async Task UpdateLittlePhotoVisInChat(User loggedUser)
        {
            if (_connection is null) return;

            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("UpdateLittlePhotoVisInChat", loggedUser);
            }
        }

        public static async Task UpdatePagePhoto(User loggedUser)
        {
            if (_connection is null) return;

            if(_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("UpdatePagePhoto", loggedUser);
            }       
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

        public static async Task RemoveManyMessagesByDateTimes(
            List<DateTime> sentTimes, int loggedUserId, int chatterId)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("RemoveManyMessagesByDateTimes",
                    sentTimes, loggedUserId, chatterId);
        }

        public static async Task UpdateUserImages(TelegramLib.MainClasses.User user)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateUserImages", user);
        }

        public static async Task UpdateCachedSettings(int id)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateCachedSettings", id);
        }

        public static async Task SendAllMessages(List<Message> messages, User sender, User chatter)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendAllMessages", messages, sender, chatter);
        }
    }
}
