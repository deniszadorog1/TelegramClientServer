using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.MainClasses.FolderObjs;
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

        public List<Folder> Folders { get; set; }

        public TelSystem(User user, MainSettings settings,
            List<UserChat> chats, List<UserContactcs> contacts,
            List<Folder> folders)
        {
            LoggedUser = user;
            Settings = settings;
            Chats = chats;
            Contacts = contacts;
            Folders = folders;
        }
        
        //Test system
        public TelSystem()
        {
            LoggedUser = new User();
            Settings = new MainSettings();
            Chats = new List<UserChat>();
            Contacts = new List<UserContactcs>();
            Folders = new List<Folder>();

            SetTestSystemParams();
        }

        public void AddFolder(string name, string iconName,  
            List<UserContactcs> contacts, List<UserContactcs> excludedContacts)
        {
            Folder toAdd = new Folder(Folders.Count + 1, name, iconName, contacts, excludedContacts);
            Folders.Add(toAdd);
        }

        public bool IsFolderNameExists(string name)
        {
            return Folders.Any(x => x.Name == name);
        }

        public void AddFolder(Folder folder)
        {
            //Check is need it add NOT AT THE END
            Folders.Add(folder);
        }

        public void RemoveFolderById(int id)
        {
            Folders.Remove(Folders.Where(x => x.Id == id).FirstOrDefault());
        }

        public Folder GetFolderByName(string name)
        {
            return Folders.FirstOrDefault(x => x.Name == name);
        }

        public void ChangeFoldersName(string tempName, string newName)
        {
            Folder folder =  Folders.FirstOrDefault(x => x.Name == tempName);
            if (folder is null) return;
            folder.SetName(newName);
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

            ChosenChatContact = newChat.Chatter;
            SetChatBg();

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
            SetTestFolders();
        }

        public void SetTestFolders()
        {
            Folders.Add(new Folder(1, "FirstTest", "Folder", 
                new List<UserContactcs>() { Contacts[0], Contacts[1] }, 
                new List<UserContactcs>()));

            Folders.Add(new Folder(1, "Android", "Android",
                new List<UserContactcs>() { Contacts[1], Contacts[2] },
                new List<UserContactcs>()));

            Folders.Add(new Folder(1, "BellTest", "Bell",
                new List<UserContactcs>() { Contacts[0], Contacts[2] },
                new List<UserContactcs>()));
        }

        public void SetTestUserContacts()
        {
            Contacts.Add(new UserContactcs(1, "FirstName", "FirstUserName",  DateTime.Now, "FirstBIO", "FirstPhoneNumber", null, DateTime.Now, true));
            Contacts.Add(new UserContactcs(2, "SecondName", "SecondUserName",  DateTime.Now, "SecondBIO", "SecondPhoneNumber", null, null, false));
            Contacts.Add(new UserContactcs(3, "ThirdName", "ThirdUserName",  DateTime.Now, "ThirdBIO", "ThirdPhoneNumber", null, DateTime.Now, true));
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
