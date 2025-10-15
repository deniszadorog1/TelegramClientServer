using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.MainClasses.Messages;
using TelegramLib.MainClasses.UserParams;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using Folder = TelegramLib.MainClasses.FolderObjs.Folder;
using UserImage = TelegramLib.MainClasses.UserParams.UserImage;

namespace TelegramLib.MainClasses
{
    public class TelSystem
    {
        public User LoggedUser { get; set; }
        public MainSettings Settings { get; set; }
        public List<UserChat> Chats { get; set; }
        public List<UserContactcs> Contacts { get; set; }

        public List<Folder> Folders { get; set; }

        public List<UserChat> ChatInNewWindow = new List<UserChat>();

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

            //SetTestSystemParams();
        }

        public void AddFolder(int id, string name, string iconName,
            List<User> contacts, List<User> excludedContacts)
        {
            Folder toAdd = new Folder(id, name, iconName, contacts, excludedContacts);
            Folders.Add(toAdd);
        }

        public Folder GetLastFolder()
        {
            return Folders.Last();
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

        public void RemoveFolder(Folder folder)
        {
            Folders.Remove(folder);
        }

        public void RemoveFolderById(int id)
        {
            Folders.Remove(Folders.Where(x => x.Id == id).FirstOrDefault());
        }

        public Folder GetFolderByName(string name)
        {
            return Folders.FirstOrDefault(x => x.Name == name);
        }

        public Folder GetFolderById(int id)
        {
            return Folders.FirstOrDefault(x => x.Id == id);
        }

        public void ChangeFoldersName(string tempName, string newName)
        {
            Folder folder = Folders.FirstOrDefault(x => x.Name == tempName);
            if (folder is null) return;
            folder.SetName(newName);
        }

        public User ChosenChatContact;
        public void SetTempChatter(string login)
        {
            //ChosenChatContact = Chatters.Where(x => x.Name == login).FirstOrDefault();
        }

        public UserChat GetChosenChat()
        {
            if (ChosenChatContact is null) return null;
            return Chats.FirstOrDefault(x => x.GetChatter().Name == ChosenChatContact.Name);
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
            return !(ChosenChatContact is null);
        }

        public UserChat GetUserChatByChatterId(int id)
        {
            //UserChat chat = Chats.Where(x => x.IsNamesAreEqual(chatterName)).FirstOrDefault();
            UserChat chat = Chats.FirstOrDefault(x => x.IsChatterIdsAreEqual(id) /*IsUserLoginsAreEqual(chatterLogin)*/);

            //Get tel system
            if (!(chat is null))
            {
                ChosenChatContact = chat.Chatter;

                //is need to change for ekse bg
                SetChatBg();
                return chat;
            }
            //Create new chat(if its absent)


            throw new Exception("cant be chat should be set!!!");

            /*  UserChat newChat = new UserChat(Chats.Count + 1, GetContactByName(chatterName), new List<Message>(),
                  new ChatFitures.ChatBackground("fray.jpg", false, true));

              Chats.Add(newChat);

              ChosenChatContact = newChat.Chatter;
              SetChatBg();

              return newChat;*/
        }

        public void SetChatBg()
        {
            UserSettings.SettingsTypes.SubSettings.ChatWallpaper wallpaper =
                Settings.GetChatSettings().Wallpaper;

            UserChat chosen = GetChosenChat();

            if (!(chosen.GetBackground() is null) &&
                !chosen.GetBackground().IsGeneral) return;

            chosen.ChatBg = new ChatFitures.ChatBackground
                (wallpaper.WallpaperName, wallpaper.IsBlurred, true);
        }

        /*        public UserContactcs GetContactByName(string name)
                {
                    return Contacts.Where(x => x.IsNamesAreEqual(name)).FirstOrDefault();
                }
        */
        public UserContactcs GetContactByUserId(int id)
        {
            return Contacts.FirstOrDefault(x => x.ContactUserId == id);/*x.IsSendersIdsAreEqual(id)*/
        }

        public User GetUserById(int userId)
        {
            UserChat chat = Chats.FirstOrDefault(x => x.Chatter.Id == userId);
            return chat is null ? null : chat.Chatter;
        }

        public UserContactcs GetContactByLogin(string login)
        {
            return Contacts.FirstOrDefault(x => x.UserLoginsAreEqual(login));
        }

        public User IsUserIsSameId(int id)
        {
            return LoggedUser.IsSameId(id) ? LoggedUser : null;
        }

        public void SetTestSystemParams()
        {
            SetTestUserContacts();
            SetTestFolders();

            SetTestLoggedUserParams();
        }

        public void SetTestLoggedUserParams()
        {
            /*            List<User> blockedContacts = new List<User>();



                        //blockedContacts.Add(Contacts[0]);

                        LoggedUser.BlockedContacts = blockedContacts;*/
        }

        public void SetTestFolders()
        {
            /*            Folders.Add(new Folder(1, "FirstTest", "Folder",
                            new List<UserContactcs>() { Contacts[0], Contacts[1] },
                            new List<UserContactcs>()));

                        Folders.Add(new Folder(1, "Android", "Android",
                            new List<UserContactcs>() { Contacts[1], Contacts[2] },
                            new List<UserContactcs>()));

                        Folders.Add(new Folder(1, "BellTest", "Bell",
                            new List<UserContactcs>() { Contacts[0], Contacts[2] },
                            new List<UserContactcs>()));*/
        }


        public void SetTestUserContacts()
        {
            List<UserImage> imageNames = new List<UserImage>();
            imageNames.Add(new UserImage("fray.jpg", DateTime.Now));
            imageNames.Add(new UserImage("Minato.jpg", DateTime.Now));
            imageNames.Add(new UserImage("WhiteCat.png", DateTime.Now));

            Contacts.Add(new UserContactcs(1, "FirstName", "FirstSurname", "FirstUserName", DateTime.Now, "FirstBIO", "FirstPhoneNumber", DateTime.Now, true, imageNames, null, false));
            Contacts.Add(new UserContactcs(2, "SecondName", "SecondSurname", "SecondUserName", DateTime.Now, "SecondBIO", "SecondPhoneNumber", null, false, imageNames, null, false));
            Contacts.Add(new UserContactcs(3, "ThirdName", "ThirdSurname", "ThirdUserName", DateTime.Now, "ThirdBIO", "ThirdPhoneNumber", DateTime.Now, true, imageNames, null, false));
        }

        public List<Message> GetTestMessages()
        {
            return new List<Message>();
            List<Message> res = new List<Message>();

            /*            res.Add(new TextMessage(1, -1, DateTime.Now, "First"));
                        res.Add(new TextMessage(2, -1, DateTime.Now, "Second"));
                        res.Add(new MediaAction(3, -1, DateTime.Now, "TestGif.gif", false));
                        res.Add(new MediaAction(4, -1, DateTime.Now, "Mine.jpg", false));
                        res.Add(new TextMessage(5, -1, DateTime.Now, "Three"));
                        res.Add(new MediaAction(6, -1, DateTime.Now, "Cow.jpg", false));
                        res.Add(new MediaAction(7, -1, DateTime.Now, "TestVideo.mp4", false));
                        res.Add(new TextMessage(8, -1, DateTime.Now, "Four"));
                        res.Add(new MediaAction(9, -1, DateTime.Now, "Hand.jpg", false));
                        res.Add(new MediaAction(10, -1, DateTime.Now, "TestGif.gif", false));*/
            return res;
        }

        public void SetGeneralBgToChatsBg()
        {
            UserChat chat = GetChosenChat();

            if (chat.GetBackground().IsGeneral == false) return;

            chat.GetBackground().SetPath(Settings.GetChatSettings().Wallpaper.WallpaperName);
            chat.GetBackground().SetBlurState(Settings.GetChatSettings().Wallpaper.IsBlurred);
        }

        public void RemoveElemetFromChosenChat(int mediaIndex, MediaType type)
        {
            GetChosenChat().RemoveElementByIndex(mediaIndex, type);
        }

        public void RemoveContact(UserContactcs contact)
        {
            //Remove chat with messages where contact is
            //RemoveChatsWithContact(contact);

            //remove from folder where contact is
            //RemoveContactFromFolderContacts(contact);

            //Remvoe empty folders
            //RemoveEmptyFolders();

            //Remove from blocked contacts
            //LoggedUser.RemoveBlockedContcatByContact(contact);

            Contacts.Remove(Contacts.Where(x => x.Id == contact.Id).FirstOrDefault());
        }



        private void RemoveEmptyFolders()
        {
            List<Folder> toRemove = new List<Folder>();
            for (int i = 0; i < Folders.Count; i++)
            {
                if (Folders[i].Contacts.Count == 0)
                {
                    toRemove.Add(Folders[i]);
                }
            }

            foreach (var remove in toRemove)
            {
                Folders.Remove(remove);
            }

        }

        private void RemoveContactFromFolderContacts(UserContactcs contact)
        {
            for (int i = 0; i < Folders.Count; i++)
            {
                User toRemove = Folders[i].Contacts.FirstOrDefault(x => x.Id == contact.Id);
                if (!(toRemove is null)) Folders[i].Contacts.Remove(toRemove);

                toRemove = Folders[i].ExcludedContacts.FirstOrDefault(x => x.Id == contact.Id);
                if (!(toRemove is null)) Folders[i].ExcludedContacts.Remove(toRemove);
            }
        }

        private void RemoveChatsWithContact(UserContactcs contact)
        {
            List<UserChat> toRemove = Chats.Where(x => x.Chatter.Id == contact.Id).ToList();

            foreach (var remove in toRemove)
            {
                Chats.Remove(remove);
            }
        }

        public void AddChat(UserChat chat)
        {
            Chats.Add(chat);
        }


        public UserChat GetChatByChatterId(int id)
        {
            return Chats.FirstOrDefault(x => x.Chatter.Id == id);
        }

        public UserChat GetChatById(int id)
        {
            return Chats.FirstOrDefault(x => x.Id == id);
        }

        public List<MediaAction> GetAllImageMessages()
        {
            return GetMesagesByType(MediaType.Image);
        }

        public List<MediaAction> GetAllVideoMessages()
        {
            return GetMesagesByType(MediaType.Video);
        }

        private List<MediaAction> GetMesagesByType(MediaType type)
        {
            List<MediaAction> res = new List<MediaAction>();
            for (int i = 0; i < Chats.Count; i++)
            {
                for (int j = 0; j < Chats[i].Messages.Count; j++)
                {
                    if (!(Chats[i].Messages[j] is MediaAction media)) continue;
                    if (type == MediaType.Image && media.IsImage()) res.Add(media);
                    else if (type == MediaType.Video && media.IsVideo()) res.Add(media);
                }
            }
            return res;
        }

        public bool IsChatContainsInOtherWidowList(UserChat chat)
        {
            return ChatInNewWindow.Contains(chat);
        }

        public void AddChatInOtherWindow(UserChat chat)
        {
            ChatInNewWindow.Add(chat);
        }

        public void RemoveChatFromOtherWindow(UserChat chat)
        {
            ChatInNewWindow.Remove(chat);
        }

        public void AddContactToFolder(string folderName, User contact)
        {
            Folder folder = GetFolderByName(folderName);
            if (folder is null) return;

            folder.AddContact(contact);
        }

        public void RemoveContactFromFolder(string folderName, User contact)
        {
            Folder folder = GetFolderByName(folderName);
            if (folder is null) return;

            folder.RemoveContactById(contact.Id);
        }

        public UserChat GetChatByUserId(UserContactcs contact)
        {
            for (int i = 0; i < Chats.Count; i++)
            {
                if (Chats[i].Chatter.Id == contact.ContactUserId) return Chats[i];
            }
            return null;
        }

        public User GetChatterById(int chatterId)
        {
            UserChat chat = Chats.FirstOrDefault(x => x.Chatter.Id == chatterId);
            return chat is null ? null : chat.Chatter;
        }

        public bool IsChatterIdIsContact(int chatterId)
        {
            return Contacts.Any(x => x.ContactUserId == chatterId);
        }

        public void DeleteChatByChatter(User chatter)
        {
            //Get chat
            UserChat chat = Chats.FirstOrDefault(x => x.Chatter.Id == chatter.Id);
            if (chat is null) return;

            //Delete from chats
            //Chats.Remove(chat);
            chat.ClearChat();

            //Delete from folders
            for (int i = 0; i < Folders.Count; i++)
            {
                User toRemove = Folders[i].Contacts
                    .FirstOrDefault(x => x.Id == chatter.Id);
                if (!(toRemove is null)) Folders[i].Contacts.Remove(toRemove);
            }

            Chats.Remove(chat);
        }

        public List<int> GetFoldersIdWithGivenUserId(int userId)
        {
            return Folders
                .Where(x => x.Contacts.Select(y => y.Id)
                .Contains(userId)).Select(x => x.Id)
                .ToList();
        }

        public List<(TextMessage, int)> GetMessagesChatIdFromChatsWithGivenSubChat(string subString)
        {
            List<(TextMessage, int)> res = new List<(TextMessage, int)>();
            for(int i = 0; i < Chats.Count; i++)
            {
                for(int j = 0; j < Chats[i].Messages.Count; j++)
                {
                    if (Chats[i].Messages[j] is TextMessage text &&
                        text.Text.Contains(subString))
                    {
                        res.Add((text, Chats[i].Id));
                    }
                }
            }
            return res;
        }

        public (string name, string phoneNumber, string imgName) GetChatterNameByChatId(int chatId)
        {
            UserChat chat = Chats.FirstOrDefault(x => x.Id == chatId);
            if (chat is null) return (string.Empty, string.Empty, string.Empty);

            User chatter = chat.Chatter;

            UserContactcs contact = Contacts.FirstOrDefault(x => x.ContactUserId == chatter.Id);

            if (!(contact is null))
            {
                return (contact.Name, contact.PhoneNumber, contact.GetFirstImageNameInString());
            }
            return (chatter.Name, chatter.PhoneNumber, chatter.GetFirstImageNameInString());
        }

        public void AddShareMessage(UserContactcs contact)
        {
            TelegramLib.MainClasses.User share = GetUserById(contact.ContactUserId);

           TelegramLib.MainClasses.Messages.ShareContactMessage toAdd = 
                new TelegramLib.MainClasses.Messages.ShareContactMessage(-1,
                LoggedUser.Id, DateTime.Now, contact.Name, share, false);

            TelegramLib.MainClasses.UserChat chat = GetChatByChatterId(share.Id);
            chat.Messages.Add(toAdd);
        }

        public string GetMessageSenderLoginByMessage(Message message)
        {
            //Is contact contacts - get it
            UserContactcs contact =  GetContactById(message.SenderUserId);
            if (!(contact is null)) return contact.Login;

            //Else - get user chatter by message
            User chatter = GetChatterByMessage(message.SenderUserId);
            if (!(chatter is null)) return chatter.Login;
            return LoggedUser.Login;
        }

        public UserContactcs GetContactById(int senderId)
        {
            return Contacts.FirstOrDefault(x => x.ContactUserId == senderId);
        }

        public User GetChatterByMessage(int senderId)
        {
            return Chats.Select(x => x.Chatter).FirstOrDefault(x => x.Id == senderId);
        }

        public bool IsChatContainsInChats(int chatId)
        {
            return Chats.Any(x => x.Id == chatId);
        }

        public bool IsChatterBlocked(User chatter)
        {
            User toCheck = LoggedUser.BlockedUsers.FirstOrDefault
                (x => x.Id == chatter.Id);

            return !(toCheck is null);

/*            UserContactcs contact = Contacts.FirstOrDefault(x => x.ContactUserId == chatter.Id);
            return contact is null ? false : contact.IsBlockedUserBlocked;*/

            
        }
    }
}
