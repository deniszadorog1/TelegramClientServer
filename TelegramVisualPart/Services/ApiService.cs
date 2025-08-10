using ControlzEx.Standard;
using MahApps.Metro.Controls;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Linq;
using TelegramLib.Helpers;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.Services;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.Pages.Contacts;

//using static ControlzEx.Standard.NativeMethods;

namespace TelegramVisualPart.Services
{
    public static class ApiService
    {
        private static readonly HttpClient _client;
        private static readonly string? _host =
            Environment.GetEnvironmentVariable("localHost");

        static ApiService()
        {
            DotNetEnv.Env.Load();

            _host = Environment.GetEnvironmentVariable("localHost");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri(_host);
            //_client.BaseAddress = new Uri("https://localhost:7238/");

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<bool> AddNewUser(string login,
            string password, string name, string surname,
            string phoneNumber, DateTime? birthDate)
        {
            var data = new
            {
                Login = login,
                Password = password,
                Name = name,
                Surname = surname,
                PhoneNumber = phoneNumber,
                BirthDate = birthDate
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/StartPage/AddUser", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateUser(TelegramLib.MainClasses.User user)
        {
            var data = new { User = user };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/StartPage/UpdateUser", content);

             return response.IsSuccessStatusCode;
        }

        public static async Task<bool> AddUserSettings(int userId)
        {
            var data = new
            {
                UserId = userId
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/StartPage/AddUserSettings", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> AddUserBasicColor(int userId)
        {
            var data = new
            {
                UserId = userId
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/StartPage/AddUserBasicColor", content);

            return response.IsSuccessStatusCode;
        }

        // Get User
        public static async Task<TelegramLib.MainClasses.User> GetUser(string login, string password)
        {
            var date = new { Login = login, Password = password };

            var json = JsonConvert.SerializeObject(date);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.GetAsync($"api/StartPage/GetUser?login={login}&password={password}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.User? user = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.User>(jsonResponse);
            return user;
        }


        // Get TelSystem
        public static async Task<TelSystem> GetTelSystem(string login, string password)
        {
            var date = new { Login = login, Password = password };

            var json = JsonConvert.SerializeObject(date);
            var contact = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.GetAsync($"api/StartPage/GetTelSystem?login={login}&password={password}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelSystem? res = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelSystem>(jsonResponse);

            return res;
        }



        //SETTINGS
        //Advanced setting
        public static async Task<bool> UpdateAdvanced(TelegramLib.UserSettings.SettingsTypes.AdvancedSettings settings)
        {
            var date = new { Advanced = settings };

            var json = JsonConvert.SerializeObject(date);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateAdvanced", content);

            return response.IsSuccessStatusCode;
        }

        //notifs settings 
        public static async Task<bool> UpdateNotificationSettings(NotificationSettings settings)
        {
            var date = new { NotifsAndSoundSettings = settings };

            var json = JsonConvert.SerializeObject(date);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateNotifsAndSounds", content);

            return response.IsSuccessStatusCode;
        }

        //Chat settings
        public static async Task<bool> UpdateChatSettings(TelegramLib.UserSettings.SettingsTypes.ChatSettings settings)
        {
            var date = new { ChatSet = settings };

            var json = JsonConvert.SerializeObject(date);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateChatSettings", content);

            return response.IsSuccessStatusCode;
        }

        //Priv Settings
        public static async Task<bool> UpdatePrivSettings(PrivAndSecSettings settings)
        {
            var date = new { Settings = settings };

            var json = JsonConvert.SerializeObject(date);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdatePrivacySettings", content);

            return response.IsSuccessStatusCode;
        }

        //Add contact
        public static async Task<bool> AddContact(int userId, UserContactcs contact)
        {
            var data = new { Contact = contact, UserId = userId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Social/AddContact", content);

            return response.IsSuccessStatusCode;
        }

        //Update contact
        public static async Task<bool> UpdateContact(int userId, UserContactcs contact)
        {
            var data = new { Contact = contact, UserId = userId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/UpdateContact", content);

            return response.IsSuccessStatusCode;
        }

        //Add blocked contact
        public static async Task<bool> AddBlockedContact(int userId, int contactId)
        {
            var data = new { UserId = userId, ContactId = contactId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Social/AddBlockedContact", content);

            return response.IsSuccessStatusCode;
        }

        //remove blocked contact
        public static async Task<bool> RemoveBlockedContact(int userId, int contactId)
        {
            var data = new { UserId = userId, ContactId = contactId };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                //RequestUri = new Uri("https://localhost:7238/api/Login/RemoveClosedGames"),
                RequestUri = new Uri($"{_host}api/Social/DeleteBlockedContact"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        //Update User color
        public static async Task<bool> UpdateUserColor(ColorHelper chosenColor)
        {
            var data = new { Contact = chosenColor };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateUserColor", content);

            return response.IsSuccessStatusCode;
        }

        //Add folder
        public static async Task<bool> AddFolder(TelegramLib.MainClasses.FolderObjs.Folder folder, int userId)
        {
            var data = new { Folder = folder, UserId = userId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Social/AddFolder", content);

            return response.IsSuccessStatusCode;
        }

        //Update folder
        public static async Task<bool> UpdateFolder(TelegramLib.MainClasses.FolderObjs.Folder folder, int userId)
        {
            var data = new { Folder = folder, UserId = userId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/UpdateFolder", content);

            return response.IsSuccessStatusCode;
        }

        //Add message
        public static async Task<bool> AddMessage(Message message, UserChat chat)
        {
            var data = new { Chat = chat, ActionMessage = message };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Social/AddMessage", content);

            return response.IsSuccessStatusCode;
        }


        //Add chat
        public static void AddChat()
        {

        }

        //Update chat

        public static async Task<bool> UpdateChat(UserChat chat)
        {
            var data = new { Chat = chat };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/UpdateChat", content);

            return response.IsSuccessStatusCode;
        }

    }
}
