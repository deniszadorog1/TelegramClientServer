using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;
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

namespace TelegramVisualPart.Services
{
    public static class SignalRService
    {
        //SignalR -> ApiService -> Controller -> DbService

        private static HubConnection _connection;
        private static TelSystem _system;


        public static event Action<User, TextMessage>? TextMessageReceived;
        public static event Action<User, MediaAction>? MediaMessageReceived;
        public static event Action<User>? UpdateContactDel;
        public static event Action<User>? UpdateOnlineStatusDel;
        public static event Action<User>? UpdateUserImage;
        public static event Action<User>? ClearChatDel;

        public static event Action<bool, User>? SetContactPhoneNumberVisibilityDel;
        public static event Action<User>? SetContactLastSeenVisStateDel;
        public static event Action<User>? SetPhoneNumVisByExpsDel;
        public static event Action<User>? UpdateBirthDateDel;
        public static event Action<User>? UpdateContactPhotoDel;
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
            // достаём userId из заголовка
            if (connection.GetHttpContext().Request.Headers.TryGetValue("userId", out var userId))
            {
                return userId;
            }
            return null;
        }

        public static async Task SetSignalRConnection()
        {
            _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7164/chatHub", options =>
            {
                options.Headers.Add("userId", _system.LoggedUser.Id.ToString());
            }) 
            .Build();

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

                _system.Contacts.Add(contact);

                //Add chat in DB
                await ApiService.AddNewChat(_system.LoggedUser.Id, contact.Id);

                UserChat chatToAdd = await ApiService.GetChatByUserAndSenderId(_system.LoggedUser.Id, contact.Id);
                _system.AddChat(chatToAdd);
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

            await _connection.StartAsync();
        }

        public static async Task SendTextMessage(User sender, TextMessage message)
        {
            //save sent message in db here
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendTextMessage", sender, message);
        }

        public static async Task SendMediaMessage(User sender, MediaAction message)
        {
            //save sent message in db here
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendMediaMessage", sender, message);
        }

        public static async Task AddContact(User user, User contact)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("AddContact", user, contact);
        }

        public static async Task UpdateContact(User updatedUser)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("UpdateContact", updatedUser);
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


    }
}
