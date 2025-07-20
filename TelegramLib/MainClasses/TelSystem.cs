using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.MainClasses.Messages;
using TelegramLib.UserSettings;
using TelegramLib.UserSettings.SettingsTypes;

namespace TelegramLib.MainClasses
{
    public class TelSystem
    {
        public User LoggedUser { get; set; }
        public MainSettings Settings { get; set; }

        public List<UserChat> Chats { get; set; }

        public List<UserContactcs> Contacts { get; set; }

        public TelSystem(User user, MainSettings settings,
            List<UserChat> chats, List<UserContactcs> contacts)
        {
            LoggedUser = user;
            Settings = settings;
            Chats = chats;
            Contacts = contacts;
        }
        
        //Test system
        public TelSystem()
        {
            LoggedUser = new User();
            Settings = new MainSettings();
            Chats = new List<UserChat>();
            Contacts = new List<UserContactcs>();

            SetTestSystemParams();
        }

        public UserContactcs ChosenChatContact;
        public void SetTempChatter(string login)
        {
            ChosenChatContact = Contacts.Where(x => x.Name == login).FirstOrDefault();
        }

        public UserChat GetChosenChat()
        {
            return Chats.Where(x => x.GetChatter().Name == ChosenChatContact.Name).FirstOrDefault();
        }

        public int GetChatsAmount()
        {
            return Chats.Count;
        }

        public UserChat GetChatByIndex(int index)
        {
            return Chats[index];
        }

        public bool IsChatterIsSet()
        {
            return ChosenChatContact is not null;
        }

        public UserChat GetUserChatByChatterName(string chatterName)
        {
            UserChat chat = Chats.Where(x => x.IsNamesAreEqual(chatterName)).FirstOrDefault();
            if (chat is not null)
            {
                ChosenChatContact = chat.Chatter;

                //is need to change for ekse bg
                SetChatBg();
                return chat;
            }
            //Create new chat(if its absent)
            UserChat newChat = new UserChat(Chats.Count + 1, GetContactByName(chatterName), new List<Message>(), 
                new ChatFitures.ChatBackground("fray.jpg", false, true), null);

            Chats.Add(newChat);
            SetChatBg();

            ChosenChatContact = newChat.Chatter;
            return newChat;
        }

        public void SetChatBg()
        {
            UserSettings.SettingsTypes.SubSettings.ChatWallpaper wallpaper =
                Settings.GetChatSettings().Wallpaper;

            UserChat chosen =  GetChosenChat();

            if (!chosen.GetBackground().IsGeneral) return;

            chosen.ChatBg = new ChatFitures.ChatBackground
                (wallpaper.WallpaperName, wallpaper.IsBlurred, true);
        }

        public UserContactcs GetContactByName(string name)
        {
            return Contacts.Where(x => x.IsNamesAreEqual(name)).FirstOrDefault();
        }

        public UserContactcs GetContactById(int id)
        {
            return Contacts.Where(x => x.IsSendersIdsAreEqual(id)).FirstOrDefault();
        }

        public User IsUserIsSameId(int id)
        {
            return LoggedUser.IsSameId(id) ? LoggedUser : null;
        }

        public void SetTestSystemParams()
        {
            SetTestUserContacts();
        }

        public void SetTestUserContacts()
        {
            Contacts.Add(new UserContactcs(1, "FirstName", "FirstUserName",  DateTime.Now, "FirstBIO", "FirstPhoneNumber", null, DateTime.Now, true));
            Contacts.Add(new UserContactcs(1, "SecondName", "SecondUserName",  DateTime.Now, "SecondBIO", "SecondPhoneNumber", null, null, false));
            Contacts.Add(new UserContactcs(1, "ThirdName", "ThirdUserName",  DateTime.Now, "ThirdBIO", "ThirdPhoneNumber", null, DateTime.Now, true));
        }

        public List<Message> GetTestMessages()
        {
            return new List<Message>();
            List<Message> res = new List<Message>();

            res.Add(new TextMessage(1, -1, DateTime.Now, "First"));
            res.Add(new TextMessage(2, -1, DateTime.Now, "Second"));
            res.Add(new MediaAction(3, -1, DateTime.Now, "TestGif.gif"));
            res.Add(new MediaAction(4, -1, DateTime.Now, "Mine.jpg"));
            res.Add(new TextMessage(5, -1, DateTime.Now, "Three"));
            res.Add(new MediaAction(6, -1, DateTime.Now, "Cow.jpg"));
            res.Add(new MediaAction(7, -1, DateTime.Now, "TestVideo.mp4"));
            res.Add(new TextMessage(8, -1, DateTime.Now, "Four"));
            res.Add(new MediaAction(9, -1, DateTime.Now, "Hand.jpg"));
            res.Add(new MediaAction(10, -1, DateTime.Now, "TestGif.gif"));
            return res;
        }
       
        public void SetGeneralBgToChatsBg()
        {
           UserChat chat =  GetChosenChat();

            if (chat.GetBackground().IsGeneral == false) return;

            chat.GetBackground().SetPath(Settings.GetChatSettings().Wallpaper.WallpaperName);
            chat.GetBackground().SetBlurState(Settings.GetChatSettings().Wallpaper.IsBlurred);
        }

    }
}
