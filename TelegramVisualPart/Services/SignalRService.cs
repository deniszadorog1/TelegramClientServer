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

namespace TelegramVisualPart.Services
{
    public static class SignalRService
    {
        //SignalR -> ApiService -> Controller -> DbService

        private static HubConnection _connection;

        public static event Action<TelegramLib.MainClasses.UserChat, TextMessage>? TextMessageReceived;

        public static async Task SetBasicSignalRConnetion()
        {
            await SetSignalRConnection();
        }

        public static async Task SetSignalRConnection()
        {
            _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7164/chatHub")
            .Build();

            _connection.On<TelegramLib.MainClasses.UserChat, TextMessage>("ReceiveTextMessage", (chat, message) =>
            {
                //Need to be: senderUSERid, receiverUSERid, message
                //sender already added message to hiss chatDB
                //there receiver should add message to RECEIVERSdb

                //sender converts into contact id -> find chat in db by this params -> add message to chat


                return;
                Console.WriteLine($"{chat}: {message}");
            });

            _connection.On<TelegramLib.MainClasses.UserChat, MediaAction>("ReceiveMediaMessage", (chat, message) =>
            {
                //SignalR -> ApiService -> Controller -> DbService

                return;
                Console.WriteLine($"{chat}: {message}");
            });

            await _connection.StartAsync();
        }

        public static async Task SendTextMessage(TelegramLib.MainClasses.UserChat chat, TextMessage message)
        {
            //save sent message in db here
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendTextMessage", chat, message);
        }

        public static async Task SendMediaMessage(TelegramLib.MainClasses.UserChat chat, MediaAction message)
        {
            //save sent message in db here
            if (_connection.State == HubConnectionState.Connected)
                await _connection.InvokeAsync("SendMediaMessage", chat, message);
        }
    }
}
