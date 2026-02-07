using ControlzEx.Standard;
using MahApps.Metro.Controls;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Protocols.OpenIdConnect.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using TelegramLib.Helpers;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.Services;
using TelegramLib.UserSettings;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.UserControls.ContactsControls;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (name is null || name == string.Empty) name = login;

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

        public static async Task<bool> AddShareContactMessage(int sharedUserId, 
            string shareName, int chatId, int senderId, string message)
        {
            var data = new
            {
                SharedUserId = sharedUserId,
                SharedName = shareName,
                ChatId = chatId,
                SenderId = senderId,
                Message = string.Empty
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Social/AddShareContactMessage", content);
            return response.IsSuccessStatusCode;
        }

        public static async Task<List<Message>> GetMessagesByChatId(int chatId)
        {
            var response = 
                await _client.GetAsync($"api/Social/GetMessagesByChatId?chatId={chatId}");
            string jsonResponse = 
                await response.Content.ReadAsStringAsync();

            List<TelegramLib.MainClasses.Messages.Message>? mes =
                JsonConvert.DeserializeObject<List<Message>>(
                jsonResponse,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            return mes;
        }

        public static async Task<bool> GetMessageReadStatus(int mesId)
        {
            var response = await _client.GetAsync($"api/Social/GetReadStatus?mesId={mesId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<bool>(jsonResponse);
        }

        public static async Task<int?> GetLastAddedStatMessageIdByChatId(int chatId)
        {
            var response = await _client.GetAsync($"api/Social/GetLastStatMesIdByChatId?chatId={chatId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int?>(jsonResponse);
        }

        public static async Task<bool> SetReadStatus(int mesId)
        {
            var data = new { MesId = mesId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Social/SetReadStatus", content);
            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateSchedMessageDate(int mesId, DateTime newDate)
        {
            var data = new { MessageId = mesId, NewDate = newDate };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Social/UpdateSchedMessageDate", content);
            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> SetContactMask(UserContactcs contact, int loggedUserId)
        {
            var data = new { Contact = contact, LoggedUserId = loggedUserId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/Social/SetContactMask", content);

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

        public static async Task<bool> ReadMessage(int messageId)
        {
            var data = new
            {
                Id = messageId
            };

            var json = JsonConvert.SerializeObject(data);
            var contact = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/ReadMessage", contact);

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

        public static async Task<bool> AddUserColor(int r, int g, int b, int userId)
        {
            var data = new
            {
                R = r,
                G = g,
                B = b,
                UserId = userId
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Settings/AddUserColor", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<TelegramLib.MainClasses.UserParams.UserImage> GetContactMask(int loggedUserId, int contactUserId)
        {
            var response = await _client.GetAsync($"api/Social/GetContactMask?loggedUserId={loggedUserId}&contactUserId={contactUserId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.UserParams.UserImage? imgParams = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.UserParams.UserImage>(jsonResponse);
            return imgParams;

        }

        // Get User
        public static async Task<TelegramLib.MainClasses.User> GetUser(string login, string password)
        {
            var response = await _client.GetAsync($"api/StartPage/GetUser?login={login}&password={password}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.User? user = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.User>(jsonResponse);
            return user;
        }

        public static async Task<TelegramLib.MainClasses.Messages.Message> GetTextMessageById(int id)
        {
            var response = await _client.GetAsync($"api/Social/GetMessageById?id={id}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.Messages.TextMessage? mes = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.Messages.TextMessage>(jsonResponse);
            return mes;
        }

        public static async Task<TelegramLib.MainClasses.User> GetUserById(int id)
        {
            var response = await _client.GetAsync($"api/Social/GetUserById?userId={id}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.User? user = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.User>(jsonResponse);
            return user;
        }

        public static async Task<int> GetLastSharedMessageIdByChatId(int chatId)
        {
            var response = await _client.GetAsync($"api/Social/GetLastSharedMessageIdByChatId?chatId={chatId}");
            string jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<int>(jsonResponse);
        }

        public static async Task<bool> IsUserOnline(int userId)
        {
            var response = await _client.GetAsync($"api/Social/IsUserOnline?userId={userId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return false;

            bool isOnline = JsonConvert.DeserializeObject<bool>(jsonResponse);
            return isOnline;
        }

        public static async Task<bool> IsUserRegistrationParamsAreExist(string login, string phoneNumber)
        {
            var response = await _client.GetAsync($"api/StartPage/IsRegistrationParamsAreExist?login={login}&phoneNumber={phoneNumber}");

            string jsonResponse = await response.Content.ReadAsStringAsync();

            bool res = JsonConvert.DeserializeObject<bool>(jsonResponse);
            return res;
        }

        public static async Task<bool> IsContactContainsInContacts(UserContactcs contact, UserContactcs toCheck)
        {
            var data = new
            {
                Contact = contact,
                ToCheck = toCheck
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"api/Social/IsContactContactsInContacts", content);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            bool res = JsonConvert.DeserializeObject<bool>(jsonResponse);
            return res;
        }

        public static async Task<bool> UpdateUserLanguage(int userId, TelegramLib.Enums.Settings.Language.LanguageType type)
        {
            var data = new
            {
                UserId = userId,
                Type = type
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"api/Settings/UpdateUserLanguage", content);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            bool res = JsonConvert.DeserializeObject<bool>(jsonResponse);
            return res;
        }

        public static async Task<TelegramLib.MainClasses.Messages.Message> GetLastChatMessage(int chatId)
        {
            var response = await _client.GetAsync($"api/Social/GetLastChatMessage?chatId={chatId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse))
                return null;

            var msg = JsonConvert.DeserializeObject<TelegramLib.MainClasses.Messages.Message>(
                jsonResponse,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            return msg;
        }

        public static async Task<int> GetChatBgIdByName(string name)
        {
            name = System.IO.Path.GetFileName(name);
            var response = await _client.GetAsync($"api/Social/GetChatBgIdByName?name={name}");
            string jsonResponse = await response.Content.ReadAsStringAsync();

            int res = JsonConvert.DeserializeObject<int>(jsonResponse);
            return res;
        }

        //Get User By phone number
        public static async Task<TelegramLib.MainClasses.User> GetUserByPhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace("+", "");

            var response = await _client.GetAsync($"api/Social/GetUserByPhoneNumber?phoneNumber={phoneNumber}");



            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.User? user = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.User>(jsonResponse);
            return user;
        }

        //Get Users phoneNumber
        public static async Task<string> GetUsersPhoneNumber()
        {
            var response = await _client.GetAsync($"api/Social/GetUsersPhoneNumber");

            string jsonResponse = await response.Content.ReadAsStringAsync();

            string res = JsonConvert.DeserializeObject<string>(jsonResponse);
            return res;
        }

        // Get TelSystem
        public static async Task<TelSystem> GetTelSystem(string login, string password)
        {
            var response = await _client.GetAsync($"api/StartPage/GetTelSystem?login={login}&password={password}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelSystem? res = jsonResponse is null ? null : JsonConvert.DeserializeObject<TelSystem>(jsonResponse, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return res;
        }

        public static async Task<bool> AddWallpaper(string imgName)
        {
            var data = new { ImgName = imgName };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/AddWallpaper", content);

            return response.IsSuccessStatusCode;
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

        public static async Task DeleteUserImage(
            TelegramLib.MainClasses.UserParams.UserImage img, int userId)
        {
            var data = new { UserImg = img, UserId = userId };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/DeleteUserImage"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
        }

        public static async Task ClearChat(UserChat chat)
        {
            var data = new { Chat = chat };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                //RequestUri = new Uri("https://localhost:7164/api/Login/RemoveClosedGames"),
                RequestUri = new Uri($"{_host}api/Social/ClearChat"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
        }

        public static async Task RemoveContact(UserContactcs contact, TelegramLib.MainClasses.User loggedUser)
        {
            var data = new { Contact = contact, LoggedUser = loggedUser };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/RemoveContact"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
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

        public static async Task<TelegramLib.MainClasses.Messages.Message> AddAndGetSchedMessage(UserChat chat, Message mes)
        {
            var data = new { Chat = chat, ActionMessage = mes };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Social/AddAndGetSchedMessage", content);

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.Messages.TextMessage? res = 
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.Messages.TextMessage>(jsonResponse);
            return res;
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

        public static async Task<UserContactcs> GetLastUserContact(int userId)
        {
            var response = await _client.GetAsync($"api/Social/GetLastUserContact?userId={userId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            TelegramLib.MainClasses.UserContactcs? contact = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.UserContactcs>(jsonResponse);
            return contact;
        }

        public static async Task<MainSettings> GetSettingsByUserId(int userId)
        {
            var response = await _client.GetAsync($"api/Settings/GetSettingsByUserId?userId={userId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            MainSettings? res = JsonConvert.DeserializeObject<MainSettings>(jsonResponse);
            return res;
        }

        public static async Task<bool?> GetLastSeenVisState(int userId)
        {
            var response = await _client.GetAsync($"api/Settings/GetLastSeenVisState?userId={userId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            bool? res = JsonConvert.DeserializeObject<bool?>(jsonResponse);
            return res;
        }

        public static async Task<bool> IsContactExist(int userId, int friendId)
        {
            var response = await _client.GetAsync($"api/Social/IsContactExist?userId={userId}&friendId={friendId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return false;

            bool contact = JsonConvert.DeserializeObject<bool>(jsonResponse);
            return contact;
        }
        
        public static async Task<bool> IsUserIsBlocked(int userId, int contactId)
        {
            var response = await _client.GetAsync($"api/Social/IsUserIsBlocked?userId={userId}&contactId={contactId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return false;

            bool contact = JsonConvert.DeserializeObject<bool>(jsonResponse);
            return contact;
        }

        public static async Task<bool> IsUserColorExist(int userId)
        {
            var response = await _client.GetAsync($"api/Settings/IsUserColorExist?userId={userId}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            bool res = JsonConvert.DeserializeObject<bool>(jsonResponse);
            return res;
        }

        public static async Task<int> GetUserColorId(int userId)
        {
            var response = await _client.GetAsync($"api/Settings/GetUserColorId?userId={userId}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            int res = JsonConvert.DeserializeObject<int>(jsonResponse);
            return res;
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

        public static async Task<bool> AddUserImage(TelegramLib.MainClasses.User user, string userImageName)
        {
            var date = new
            {
                User = user,
                UserImageName = userImageName
            };

            var json = JsonConvert.SerializeObject(date);
            var contact = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/AddUserImage", contact);

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

        public static async Task RemoveFolder(TelegramLib.MainClasses.FolderObjs.Folder folder, int userId)
        {
            var data = new { Folder = folder, UserId = userId };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/DeleteFolder"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
        }

        public static async Task DeleteMessageById(int id)
        {
            var data = new { Id = id };
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/DeleteMessageById"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
        }

        public static async Task<bool> DeleteManyMessages(List<int> idsToDelete, bool isBoth)
        {
            var data = new { IdsToDelete = idsToDelete, IsBoth = isBoth };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/DeleteManyMessages"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> DeleteChatById(int chatId)
        {
            var data = new { ChatId = chatId };
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/DeleteChatById"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> DeleteContactFromFolder(int folderId, int userId)
        {
            var data = new { FolderId = folderId, UserId = userId };
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/DeleteContactFromFolder"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

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

            json = JsonConvert.SerializeObject(
                data,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Social/AddMessage", content);

            return response.IsSuccessStatusCode;
        }


        //Add chat
        public static async Task AddNewChat(int userId, int contactId)
        {
            var data = new { UserId = userId, ChatterContactId = contactId };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/Social/AddChat", content);
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

        public static async Task<bool> ChangeNotificationState(int chatId, bool state)
        {
            var data = new 
            { 
                ChatId = chatId, 
                State = state
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/ChangeNotificationState", content);

            return response.IsSuccessStatusCode;
        }


        //Set aut delition
        public static async Task<bool> SetAutoDeletion(int chatId,
            TelegramLib.Enums.Chat.AutoDeleteType type)
        {
            var data = new { ChatId = chatId, DelType = type };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/SetAutoDeletion", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<UserChat?> GetChatByUserAndSenderId(int userId, int contactId)
        {
            var response = await _client.GetAsync($"api/Social/GetChatByUserAndContactId?userId={userId}&contactId={contactId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null :
                JsonConvert.DeserializeObject<UserChat>(jsonResponse, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
        }

        public static async Task<bool?> IsChatterIdIsContact(int userId, int friendUserId)
        {
            var response = await _client.GetAsync($"api/Social/IsChatterIdIsContact?userId={userId}&friendUserId={friendUserId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null :
                JsonConvert.DeserializeObject<bool>(jsonResponse, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
        }

        public static async Task<bool?> IsDateMesIsExistInChat(int loggedId, int chatterId, DateTime date)
        {
            string isoDate = date.ToString("yyyy-MM-dd HH:mm:ss.fff");

            var response = await _client.GetAsync($"api/Social/IsDateMesIsExistInChat?loggedId={loggedId}&chatterId={chatterId}&date={isoDate}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null : JsonConvert.DeserializeObject<bool?>(jsonResponse);
        }

        public static async Task<int?> GetMessageIdByDateTime(DateTime sentDate)
        {
            string isDate = sentDate.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            var response = await _client.GetAsync($"api/Social/GetMessageIdByDateTime?sentTime={isDate}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null : JsonConvert.DeserializeObject<int?>(jsonResponse);
        }

        public static async Task<int?> GetStatMessageIdByItsReference(int chatId, int refId)
        {
            var response = await _client.GetAsync($"api/Social/GetStatMessageIdByItsReference?chatId={chatId}&refId={refId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null :
                JsonConvert.DeserializeObject<int?>(jsonResponse);
        }

        public static async Task<Message?> GetPairOfMessage(TelegramLib.MainClasses.Messages.Message mes)
        {
            var response = await _client.GetAsync($"api/Social/GetPairToMessage?mesId={mes.Id}");
            if (!response.IsSuccessStatusCode) return null;

            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonResponse))
                return null;

            return JsonConvert.DeserializeObject<Message>(jsonResponse, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });


      /*      var response = await _client.GetAsync($"api/Social/GetPairToMessage?mesId={mes.Id}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse is null ? null : JsonConvert.DeserializeObject<Message>(jsonResponse); 
       */ }


        public static async Task<int> GetLastFolderIdByUserId(int userId)
        {
            var response = await _client.GetAsync($"api/Social/GetLastFolderIdByUserId?userId={userId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(jsonResponse);
        }

        public static async Task<bool> IsChatExist(int userId, int contactId)
        {
            var response = await _client.GetAsync($"api/Social/IsChatExist?userId={userId}&contactId={contactId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return false;

            return JsonConvert.DeserializeObject<bool>(jsonResponse);
        }

        public static async Task<UserContactcs> GetContactByUserAndFriendIds(int userId, int friendId)
        {
            var response = await _client.GetAsync($"api/Social/ContactBySenderAndReceiverIds?senderId={userId}&receiverId={friendId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return JsonConvert.DeserializeObject<UserContactcs>(jsonResponse);
        }

        public static async Task<bool> SetChatWallpaper(ChatBackground toSet, int chatId)
        {
            var data = new { ToSetPaper = toSet, ChatId = chatId };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/SetChatWallpaper", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task SetPinStatus(int mesId, bool pinStatus, bool isSavedChat)
        {
            var data = new { MesId = mesId, PinStatus = pinStatus, IsSaveMessageChat = isSavedChat};

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/SetPinStatus", content);

            Console.WriteLine(response.IsSuccessStatusCode);
        }

        public static async Task<bool> SetUserOnlineStatus(int userId, bool status)
        {
            var data = new { UserId = userId, Status = status };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/SetUserOnlineStatus", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateMonitor(int userId,
            TelegramLib.Enums.Settings.Notifs.NotifMessageSide side, int mesAmount)
        {
            //var data = new { Sound = sound, Volume = volume, UserId = userId, IsDefault }
            var data = new { Side = side, MessagesAmount = mesAmount, UserId = userId };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateNotifMonitor", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> EditSavedChatMessage(
            TelegramLib.MainClasses.Messages.Message mes)
        {
            var data = new
            {
                ChatId = -1,
                TextMes = mes is TextMessage text ? text : null,
                MediaMes = mes is MediaAction media ? media : null
            };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/EditSavedChatMessage", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> EditSchedMessage(int mesId,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            var data = new
            {
                MesId = mesId,
                TextMes = mes is TextMessage text ? text : null,
                MediaMes = mes is MediaAction media ? media : null
            };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/EditSchedMessage", content);

            return response.IsSuccessStatusCode;
        }


        public static async Task<bool> EditMessage(int chatId, 
            TelegramLib.MainClasses.Messages.Message mes)
        {
            var data = new { 
                ChatId = chatId, 
                TextMes = mes is TextMessage text ? text : null,
                MediaMes = mes is MediaAction media ? media : null
            };

            string json = JsonConvert.SerializeObject(data); 
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Social/EditMessage", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateSound(int userId, string sound,
            int volume, bool isDefault)
        {
            //var data = new { Sound = sound, Volume = volume, UserId = userId, IsDefault }
            var data = new { Sound = sound, Volume = volume, UserId = userId, IsDefault = isDefault };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateSound", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdateFolderPosition(int userId, bool state)
        {
            //var data = new { Sound = sound, Volume = volume, UserId = userId, IsDefault }
            var data = new { UserId = userId, State = state };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateFoldersPosition", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> UpdatePasscode(PasscodeSettings settings)
        {
            var data = new { Settings = settings };

            string json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/Settings/UpdateLocalPasscode", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<List<string>> GetAllSounds()
        {
            var response = await _client.GetAsync($"api/Settings/GetAllSounds");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            List<string>? allSounds = jsonResponse is null ? null :
                JsonConvert.DeserializeObject<List<string>>(jsonResponse);
            return allSounds;
        }

        public static async Task<bool> AddNewUser(string name)
        {
            var data = new
            {
                Name = name
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Settings/AddSound", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> AddStatMessage(StaticMessage message, int chatId)
        {
            var data = new 
            { 
                StatMessage = message,
                ChatId = chatId
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Social/AddStatMessage", content);

            return response.IsSuccessStatusCode;
        }

        //Saved messages
        public static async Task<bool> AddSavedMessage(int savedChatId, TelegramLib.MainClasses.Messages.Message mes)
        {
            var data = new
            {
                SavedChatId = savedChatId,
                Mes = mes
            };

            var json = JsonConvert.SerializeObject(data);

            json = JsonConvert.SerializeObject(
                data,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Social/AddSavedMessage", content);

            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> AddSavedMessagesChat(int userId)
        {
            var data = new
            {
                UserId = userId
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync("api/Social/AddSavedChat", content);

            return response.IsSuccessStatusCode;

        }

        public static async Task<TelegramLib.MainClasses.SavedMessagesChat?> GetSavedMessageChat(int userId)
        {
            var response = await _client.GetAsync($"api/Social/GetSavedMessagesChat?chatId={userId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null :
                JsonConvert.DeserializeObject<TelegramLib.MainClasses.SavedMessagesChat?>(jsonResponse);
        }

        public static async Task<bool?> IsDateStatContainsInSavedMessageChat(int chatId, DateTime date)
        {
            var response = await _client.GetAsync($"api/Social/IsDateStatContainsInSavedMessageChat?chatId={chatId}&date={date}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null :
                JsonConvert.DeserializeObject<bool?>(jsonResponse);
        }

        public static async Task<bool> ClearSaveChatById(int chatId)
        {
            var data = new {Id = chatId};
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_host}api/Social/ClearSaveChatById"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public static async Task RemoveSavedMessage(int savedChatId, List<int> toRemoveIds)
        {
            var data = new { SavedChatId = savedChatId, ToRemoveIds = toRemoveIds};
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                //RequestUri = new Uri("https://localhost:7164/api/Login/RemoveClosedGames"),
                RequestUri = new Uri($"{_host}api/Social/DeleteSavedMessage"),
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
        }

        public static async Task<int?> GetLastStatDateIdInSavedChat(int chatId)
        {
            var response = await _client.GetAsync($"api/Social/GetLastStatDateIdInSavedChat?chatId={chatId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null :
                JsonConvert.DeserializeObject<int>(jsonResponse);
        }

        public static async Task<int?> GetIdOfLastSavedMessage(int chatId)
        {
            var response = await _client.GetAsync($"api/Social/GetIdOfLastSavedMessage?chatId={chatId}");

            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse)) return null;

            return jsonResponse is null ? null :
                JsonConvert.DeserializeObject<int>(jsonResponse);
        }


    }
}
