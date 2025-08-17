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

namespace TelegramVisualPart.Services
{
    public static class SignalRService
    {
        //SignalR -> ApiService -> Controller -> DbService

        private static HubConnection _connection;
        private static TelSystem _system;


        public static event Action<User, TextMessage>? TextMessageReceived;


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

            _connection.On<TelegramLib.MainClasses.UserChat, MediaAction>("ReceiveMediaMessage", (chat, message) =>
            {



                return;
            });

            //Add contacts
            _connection.On<User, User>("AddContact", async (user, toAdd) =>
            {

                //Add conatct in system
                UserContactcs contact = new UserContactcs(-1, toAdd.Name, toAdd.UserName, toAdd.BirthDay,
                    toAdd.BIO, toAdd.PhoneNumber, toAdd.LastSeenOnline, true, toAdd.UserImages, null);

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
            


            await _connection.StartAsync();
        }

        public static async Task SendTextMessage(User sender, TextMessage message)
        {
            //save sent message in db here
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendTextMessage", sender, message);
        }

        public static async Task SendMediaMessage(TelegramLib.MainClasses.UserChat chat, MediaAction message)
        {
            //save sent message in db here
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendMediaMessage", chat, message);
        }

        public static async Task AddContact(User user, User contact)
        {
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("AddContact", user, contact);
        }
    }
}
