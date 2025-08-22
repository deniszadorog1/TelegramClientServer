using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Models;

using model = TelegramLib.Models;
using mainClass = TelegramLib.MainClasses;
using privSub = TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;

using TelegramLib.MainClasses.Messages;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Runtime.CompilerServices;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using System.Runtime.Serialization;
using TelegramLib.UserSettings.SettingsTypes;
using AdvancedSettings = TelegramLib.Models.AdvancedSettings;
using ChatSettings = TelegramLib.Models.ChatSettings;
using TelegramLib.MainClasses;
using System.Dynamic;
using TelegramLib.Helpers;
using System.Diagnostics;
using TelegramLib.UserSettings;
using TelegramLib.Enums.Settings.ChatSettings;
using TelegramLib.UserSettings.SettingsTypes.SubSettings;
using System.Data.Entity.Migrations.Design;
using System.CodeDom;
using System.Runtime;
using Microsoft.SqlServer.Server;
using System.Diagnostics.Contracts;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.FolderObjs;
using System.Net;
using System.Runtime.InteropServices;
using System.Net.Configuration;
using TelegramLib.Enums.Chat;
using System.Drawing;

using System.Data.Entity;
using System.Windows.Forms;
using TelegramLib.Enums.Messages;
using System.Text.Json.Serialization.Metadata;
using System.IO;
using System.Security.Permissions;
using System.Data.OleDb;

namespace TelegramLib.Services
{
    public static class DbService
    {
        public static TelSystem GetTelSystem(string login, string password)
        {
            mainClass.User user = GetUserByLoginAndPassword(login, password);
            if (user is null) return null;
            TelSystem system = new TelSystem();

            system.LoggedUser = user;
            system.Settings = GetSettingsByUserId(user.Id);
            system.Chats = GetUserChatsByUserId(user.Id);
            system.Contacts = GetUserContactsByUserId(user.Id);
            system.Folders = GetFoldersByUserId(user.Id);

            return system;
        }

        public static void AddNewUserSystem(string name, string surname, string phoneNumber,
                DateTime birthDate, string login, string password)
        {
            //Add new user

            AddUser(name, surname, phoneNumber, birthDate, login, password);

            int addedUserId = GetLastUsersId();

            //Add new settings   
            AddSettings(addedUserId);
        }

        private static int GetLastUsersId()
        {
            using (var model = new TelegramModel())
            {
                return model.User.Last().Id;
            }
        }

        private static List<mainClass.FolderObjs.Folder> GetFoldersByUserId(int userId)
        {
            List<mainClass.FolderObjs.Folder> res = new List<mainClass.FolderObjs.Folder>();

            using (var model = new TelegramModel())
            {
                foreach (var tempFolder in model.Folder)
                {
                    if (tempFolder.OwnerId == userId)
                    {
                        mainClass.FolderObjs.Folder toAdd = GetFolderById(tempFolder.Id);
                        if (toAdd is null) continue;
                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static mainClass.FolderObjs.Folder GetFolderById(int id)
        {
            mainClass.FolderObjs.Folder res = new mainClass.FolderObjs.Folder();

            using (var model = new TelegramModel())
            {
                model.Folder toAdd = model.Folder.Where(x => x.Id == id).FirstOrDefault();
                if (toAdd is null) return null;

                res.Id = toAdd.Id;
                res.Name = toAdd.Name;
                res.IconName = GetFolderIconNameById((int)toAdd.IconId);
                res.Contacts = GetContactsForFolder(toAdd.Id, false);
                res.ExcludedContacts = GetContactsForFolder(toAdd.Id, true);
            }

            return res;
        }

        public static void AddFolder(TelegramLib.MainClasses.FolderObjs.Folder folder, int userId)
        {
            using (var model = new TelegramModel())
            {
                model.Folder toAdd = new model.Folder();

                toAdd.OwnerId = userId;
                toAdd.Name = folder.Name;
                toAdd.IconId = GetFolderIconIdByName(folder.IconName);


                model.Folder.Add(toAdd);
                model.SaveChanges();

                var lastFolder = model.Folder
                .OrderByDescending(f => f.Id)
                .FirstOrDefault();

                //Add folder contacts
                AddManyContactInContcatsInFolder(lastFolder.Id,
                    folder.Contacts, false);

                AddManyContactInContcatsInFolder(lastFolder.Id,
                    folder.ExcludedContacts, true);
            }
        }

        public static void AddManyContactInContcatsInFolder(int folderId,
            List<UserContactcs> contacts, bool isExclude)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                AddContactInCOntcatsInFolder(folderId, contacts[i], isExclude);
            }
        }

        public static void AddContactInCOntcatsInFolder(int folderId,
            UserContactcs contact, bool isExclude)
        {
            using (var model = new TelegramModel())
            {
                ContactsInFolder toAdd = new ContactsInFolder();

                toAdd.FolderId = folderId;
                toAdd.ContactId = contact.Id;
                toAdd.IsExclude = isExclude;

                model.ContactsInFolder.Add(toAdd);
                model.SaveChanges();
            }
        }

        public static bool UpdateFolder(TelegramLib.MainClasses.FolderObjs.Folder folder, int userId)
        {
            using (var model = new TelegramModel())
            {
                model.Folder toUpdate = model.Folder.FirstOrDefault(x => x.Id == folder.Id);
                if (toUpdate is null) return false;

                toUpdate.Name = folder.Name;
                toUpdate.IconId = GetFolderIconIdByName(folder.IconName);


                //Update folder contacts
                //Remove Extra Contcats

                RemoveExtraContacts(toUpdate.Id, folder.Contacts, false);
                RemoveExtraContacts(toUpdate.Id, folder.ExcludedContacts, true);

                AddExtraContacts(toUpdate.Id, folder.Contacts, false);
                AddExtraContacts(toUpdate.Id, folder.ExcludedContacts, false);

                /*
                                //Remove all folderContacts 
                                RemoveContactsFromFolder(folder.Id);

                                //Add folder contacts
                                AddManyContactInContcatsInFolder(folder.Id, folder.Contacts, false);
                                AddManyContactInContcatsInFolder(folder.Id, folder.ExcludedContacts, true);*/

                model.SaveChanges();
            }
            return true;
        }

        private static void AddExtraContacts(int folderId, List<UserContactcs> contacts, bool isExclude)
        {
            using (var model = new TelegramModel())
            {
                List<ContactsInFolder> toCheck =
                    model.ContactsInFolder.Where(x => x.FolderId == folderId && x.IsExclude == isExclude).ToList();

                foreach (var contact in contacts)
                {
                    if (!toCheck.Where(x => x.ContactId == contact.Id).Any())
                    {
                        ContactsInFolder toAdd = new ContactsInFolder();

                        toAdd.FolderId = folderId;
                        toAdd.ContactId = contact.Id;
                        toAdd.IsExclude = isExclude;

                        model.ContactsInFolder.Add(toAdd);
                    }
                }
                model.SaveChanges();
            }
        }

        private static void RemoveExtraContacts(int folderId, List<UserContactcs> contacts, bool isExclude)
        {
            //Get contacts to remove 
            //Remove
            using (var model = new TelegramModel())
            {
                List<ContactsInFolder> foldContacts =
                    model.ContactsInFolder.Where(x => x.FolderId == folderId && x.IsExclude == isExclude).ToList();

                List<ContactsInFolder> toRemove = foldContacts.Where(x => contacts.Exists(y => y.Id == x.ContactId)).ToList();
                model.ContactsInFolder.RemoveRange(toRemove);

                model.SaveChanges();
            }
        }

        private static void RemoveContactsFromFolder(int folderId)
        {
            using (var model = new TelegramModel())
            {
                List<ContactsInFolder> toRemove =
                    model.ContactsInFolder.Where(x => x.FolderId == folderId).ToList();

                model.ContactsInFolder.RemoveRange(toRemove);
                model.SaveChanges();
            }
        }

        public static void RemoveFolder(int folderId)
        {
            using (var model = new TelegramModel())
            {
                RemoveContactsFromFolder(folderId);

                model.Folder.RemoveRange(model.Folder.Where(x => x.Id == folderId).ToList());
                model.SaveChanges();
            }
        }


        private static List<UserContactcs> GetContactsForFolder(int folderId, bool isExclude)
        {
            List<UserContactcs> res = new List<UserContactcs>();
            using (var model = new TelegramModel())
            {
                foreach (var canFold in model.ContactsInFolder)
                {
                    if (canFold.FolderId == folderId &&
                        canFold.IsExclude == isExclude)
                    {
                        UserContactcs toAdd = GetContactById((int)canFold.ContactId);
                        if (toAdd is null) continue;
                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static string GetFolderIconNameById(int id)
        {
            using (var model = new TelegramModel())
            {
                FolderIcons foldIcon = model.FolderIcons.Where(x => x.Id == id).FirstOrDefault();
                if (foldIcon is null) return string.Empty;
                return foldIcon.Name;
            }
        }

        private static int GetFolderIconIdByName(string name)
        {
            using (var model = new TelegramModel())
            {
                FolderIcons foldIcon = model.FolderIcons.Where(x => x.Name == name).FirstOrDefault();
                if (foldIcon is null) return 1;
                return foldIcon.Id;
            }
        }

        private static List<UserContactcs> GetUserContactsByUserId(int userId)
        {
            List<UserContactcs> res = new List<UserContactcs>();

            using (var model = new TelegramModel())
            {
                foreach (var cont in model.Contacts)
                {
                    if (cont.UserId == userId)
                    {
                        UserContactcs toAdd = GetContactById(cont.Id);
                        if (toAdd is null) continue;

                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static List<UserChat> GetUserChatsByUserId(int userId)
        {
            List<UserChat> res = new List<UserChat>();

            using (var model = new TelegramModel())
            {
                foreach (var chat in model.Chat)
                {
                    if (userId == chat.UserId)
                    {
                        UserChat toAdd = new UserChat(chat.Id, 
                            GetUserContactById((int)chat.ChatterId),
                            GetMessagesByChatId(chat.Id), GetChosenBgByChatId(chat.Id),
                            GetAutoDelTypeById(chat.AutoDeleteId));

                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static ChatBackground GetChosenBgByChatId(int chatId)
        {
            return GetChatBackPossibleChatBGs(chatId).FirstOrDefault();
        }

        private static int GetChosenBgIdByName(int chatId)
        {
            //Check for exception        
            using (var model = new TelegramModel())
            {
                PossibleChatBGs chosen = model.PossibleChatBGs.FirstOrDefault(x => x.ChatId == chatId && (bool)x.IsGeneral);
                if (chosen is null) return -1;
                return (int)chosen.ChatBgId;

                //return (int)model.PossibleChatBGs.Where(x => x.ChatId == chatId && (bool)x.IsGeneral).First().ChatBgId;
            }
        }

        private static List<ChatBackground> GetChatBackPossibleChatBGs(int chatId)
        {
            List<ChatBackground> res = new List<ChatBackground>();

            using (var model = new TelegramModel())
            {
                foreach (var chatBG in model.PossibleChatBGs)
                {
                    if (chatBG.ChatId == chatId)
                    {
                        ChatBackground toAdd = GetChatBgById((int)chatBG.ChatBgId);
                        toAdd.IsGeneral = toAdd.IsGeneral;// Set general
                        if (toAdd is null) continue;

                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static ChatBackground GetChatBgById(int bgId)
        {
            ChatBackground res = new ChatBackground();

            using (var model = new TelegramModel())
            {
                ChatBG bg = model.ChatBG.Where(x => x.Id == bgId).FirstOrDefault();
                if (bg is null) return null;

                res.FileName = bg.Name;
                res.IsBlurred = (bool)bg.IsBlurred;
                res.IsGeneral = false;
            }

            return res;
        }

        private static List<TelegramLib.MainClasses.Messages.Message> GetMessagesByChatId(int chatId)
        {
            List<TelegramLib.MainClasses.Messages.Message> res =
                new List<TelegramLib.MainClasses.Messages.Message>();

            using (var model = new TelegramModel())
            {
                foreach (var mes in model.Messages)
                {
                    if (mes.ChatId == chatId)
                    {
                        TelegramLib.MainClasses.Messages.Message toAdd;
                        if (mes.Message is null) toAdd = new MediaAction();
                        else toAdd = new TextMessage();

                        toAdd.Id = mes.Id;
                        toAdd.SenderUserId = (int)mes.SenderId;
                        toAdd.SenderId = (int)mes.SenderId;
                        toAdd.SentTime = mes.SentDate is null ? DateTime.Now : (DateTime)mes.SentDate;

                        if (toAdd is TextMessage) ((TextMessage)toAdd).Text = mes.Message;
                        else if (!(mes.ImageId is null))
                        {
                            ((MediaAction)toAdd).IsSticker = false;
                            ((MediaAction)toAdd).MediaName = GetChatImageNameById((int)mes.ImageId);
                        }
                        else if (!(mes.VideoId is null))
                        {
                            ((MediaAction)toAdd).IsSticker = false;
                            ((MediaAction)toAdd).MediaName = GetChatVideoNameById((int)mes.VideoId);
                        }
                        else if (!(mes.GifId is null))
                        {
                            ((MediaAction)toAdd).IsSticker = false;
                            ((MediaAction)toAdd).MediaName = GetChatGifNameById((int)mes.GifId);
                        }
                        else if (!(mes.StickerId is null))
                        {
                            ((MediaAction)toAdd).IsSticker = true;
                            ((MediaAction)toAdd).MediaName = GetChatStickerNameById((int)mes.StickerId);
                        }

                        //Set media or text

                        //Set message Action


                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static string GetChatStickerNameById(int id)
        {
            using (var model = new TelegramModel())
            {
                return model.StickerImage.FirstOrDefault(x => x.Id == id).Name;
            }
        }

        private static string GetChatGifNameById(int id)
        {
            using (var model = new TelegramModel())
            {
                return model.GIF.FirstOrDefault(x => x.Id == id).Name;
            }
        }

        private static string GetChatVideoNameById(int id)
        {
            using (var model = new TelegramModel())
            {
                return model.MessageVideo.FirstOrDefault(x => x.Id == id).Name;
            }
        }

        private static string GetChatImageNameById(int id)
        {
            using (var model = new TelegramModel())
            {
                return model.ChatImage.FirstOrDefault(x => x.Id == id).Name;
            }
        }

        public static List<mainClass.User> GetAllUsers()
        {
            List<mainClass.User> res = new List<mainClass.User>();
            using (var model = new TelegramModel())
            {
                foreach (var user in model.User)
                {
                    res.Add(new mainClass.User(user.Id, user.Login, user.Password,
                        user.Name, user.Surname, user.BIO,
                        new Helpers.ColorHelper(user.Id), user.PhoneNumber,
                        user.Username, user.Birthday, GetBlockedContactsByUserId(user.Id),
                        GetUserImagesByUserId(user.Id), (DateTime)user.LastOnline, (bool)user.IsOnline));
                }
            }
            return res;
        }

        public static mainClass.User GetUserById(int userId)
        {
            mainClass.User res = new mainClass.User();
            using (var model = new TelegramModel())
            {
                model.User user = model.User.Where(x => x.Id == userId).FirstOrDefault();
                if (user is null) return null;

                res.Id = user.Id;
                res.IsOnline = (bool)user.IsOnline;
                res.Login = user.Login;
                res.Password = user.Password;
                res.Name = user.Name;
                res.Surname = user.Surname;
                res.BIO = user.BIO;

                res.PhoneNumber = user.PhoneNumber;
                res.UserName = user.Username;
                res.BirthDay = user.Birthday;

                res.MainColor = GetUserColorByUserId(user.Id);
                res.LastSeenOnline = user.LastOnline is null ? DateTime.Now : (DateTime)user.LastOnline;

                res.UserImages = GetUserImagesByUserId(user.Id);
            }
            return res;
        }

        public static mainClass.User GetUserByLoginPass(string login, string password)
        {
            using (var model = new TelegramModel())
            {
                model.User user = model.User.FirstOrDefault(x => x.Login == login && x.Password == password);
                if (user is null) return null;

                return GetUserById(user.Id);
            }
        }

        public static mainClass.User GetUserByLoginAndPassword(string login, string password)
        {
            List<mainClass.User> users = GetAllUsers();
            return users.Where(x => x.Login == login && x.Password == password).FirstOrDefault();
        }

        //Correct (When add new fields in user table)
        public static void AddUser(string name, string surname, string phoneNumber,
            DateTime? birthdate, string login, string password)
        {
            using (var model = new TelegramModel())
            {
                model.User user = new model.User();

                user.Name = name;
                user.IsOnline = true;
                user.Surname = surname;
                user.PhoneNumber = phoneNumber;
                user.Birthday = birthdate;
                user.Login = login;
                user.Password = password;
                user.LastOnline = DateTime.Now;

                model.User.Add(user);

                //AddBasicUserColor(model.User.Last().Id);

                //Add start settings
                //AddSettings(model.User.Last().Id);

                model.SaveChanges();
            }
        }

        private static void AddBasicUserColor(int userId)
        {
            using (var model = new TelegramModel())
            {
                UserColor toAdd = new UserColor();

                toAdd.R = ColorHelper._basicRGB.R;
                toAdd.G = ColorHelper._basicRGB.G;
                toAdd.B = ColorHelper._basicRGB.B;
                toAdd.UserId = userId;

                model.UserColor.Add(toAdd);
                model.SaveChanges();
            }
        }

        public static void UpdateUser(mainClass.User user)
        {
            using (var model = new TelegramModel())
            {
                model.User toUpdate = model.User.Where(x => x.Id == user.Id).FirstOrDefault();

                toUpdate.Name = user.Name;
                toUpdate.IsOnline = user.IsOnline;
                toUpdate.Surname = user.Name;
                toUpdate.PhoneNumber = user.PhoneNumber;
                toUpdate.Birthday = user.BirthDay;
                toUpdate.Login = user.Login;
                toUpdate.Password = user.Password;
                toUpdate.LastOnline = user.LastSeenOnline;
                toUpdate.BIO = user.BIO;
                toUpdate.Username = user.UserName;

                UpdateColor(user.MainColor);

                model.SaveChanges();
            }
        }

        public static bool IsUserExist(string login)
        {
            using (var model = new TelegramModel())
            {
                return model.User.Where(x => x.Login == login).Any();
            }
        }

        //Contacts

        public static void GetUsersContacts(int userId)
        {
            using (var model = new TelegramModel())
            {
                List<Contacts> contacts = model.Contacts.Where(x => x.UserId == userId).ToList();

                List<UserContactcs> resContacts = new List<mainClass.UserContactcs>();
                foreach (var tempContact in contacts)
                {
                    UserContactcs toAdd = new UserContactcs();

                    toAdd.Id = tempContact.Id;
                    toAdd.IsOnline = IsContactOnlineByUserId((int)tempContact.FriendId);
                    toAdd.Name = tempContact.Name;
                    toAdd.UserName = tempContact.User.Name;
                    toAdd.BirthDate = tempContact.User.Birthday;
                    toAdd.BIO = tempContact.User.BIO;
                    toAdd.PhoneNumber = tempContact.User.PhoneNumber;
                    toAdd.LastSeen = tempContact.User.LastOnline;
                    toAdd.IsNotificationsIsOn = (bool)tempContact.IsNotifsIsOn;

                    resContacts.Add(toAdd);
                }

                /*
        public string BIO { get; set; }
        public bool IsNotificationsIsOn { get; set; }
        public List<UserImage> UserImages { get; set; }
        public bool IsBlockedUserBlocked { get; set; }
                 */
            }
        }

        private static bool IsContactOnlineByUserId(int userId)
        {
            using (var model = new TelegramModel())
            {
                model.User toGetOnlineStatus = model.User.FirstOrDefault(x => x.Id == userId);
                if (toGetOnlineStatus is null) return false;
                return (bool)toGetOnlineStatus.IsOnline;
            }
        }

        public static UserContactcs GetUserContactById(int contactId)
        {
            using (var model = new TelegramModel())
            {
                Contacts contact = model.Contacts.Where(x => x.Id == contactId).FirstOrDefault();
                if (contact is null) return null;

                UserContactcs toAdd = new UserContactcs();

                toAdd.Id = contact.Id;
                toAdd.Name = contact.Name;
                toAdd.ContactUserId = (int)contact.FriendId;
                toAdd.IsOnline = IsContactOnlineByUserId((int)contact.FriendId);
                toAdd.UserName = contact.User.Name;
                toAdd.BirthDate = contact.User.Birthday;
                toAdd.BIO = contact.User.BIO;
                toAdd.PhoneNumber = contact.User.PhoneNumber;
                toAdd.LastSeen = contact.User.LastOnline;
                toAdd.IsNotificationsIsOn = (bool)contact.IsNotifsIsOn;

                toAdd.UserImages = GetUserImagesByUserId((int)contact.FriendId);
                return toAdd;
            }
        }

        private static List<UserContactcs> GetBlockedContactsByUserId(int userId)
        {
            List<UserContactcs> res = new List<UserContactcs>();
            using (var model = new TelegramModel())
            {
                foreach (var blockedItem in model.BlockedContacts)
                {
                    if (blockedItem.Id == userId)
                    {
                        res.Add(GetContactById((int)blockedItem.BlockedContactId));
                    }
                }
            }
            return res;
        }

        public static UserContactcs GetContactById(int contactId)
        {
            UserContactcs res = new UserContactcs();
            using (var model = new TelegramModel())
            {
                Contacts contact = model.Contacts.Where(x => x.Id == contactId).FirstOrDefault();
                if (contact is null) return null;

                mainClass.User user = GetUserById((int)contact.UserId);

                res.Id = contact.Id;
                res.ContactUserId = (int)contact.FriendId;
                res.IsOnline = IsContactOnlineByUserId((int)contact.FriendId);
                res.Name = contact.Name;
                res.Surname = contact.LastName;
                res.UserName = user.UserName;
                res.BirthDate = user.BirthDay;
                res.BIO = user.BIO;
                res.PhoneNumber = user.PhoneNumber;
                res.LastSeen = user.LastSeenOnline;
                res.IsNotificationsIsOn = (bool)contact.IsNotifsIsOn;
                //res.UserImages = GetUserImagesByUserId(user.Id);
                res.IsBlockedUserBlocked = (bool)contact.IsBlocked;

                res.UserImages = GetUserImagesByUserId((int)contact.FriendId);
            }
            return res;
        }

        public static void AddContact(UserContactcs contact, int userId)
        {
            using (var model = new TelegramModel())
            {
                Contacts toAdd = new Contacts();

                toAdd.UserId = userId;
                toAdd.FriendId = contact.ContactUserId;

                toAdd.Name = contact.Name;
                toAdd.LastName = contact.UserName;
                toAdd.IsNotifsIsOn = contact.IsNotificationsIsOn;
                toAdd.IsBlocked = contact.IsBlockedUserBlocked;

                model.Contacts.Add(toAdd);
                model.SaveChanges();
            }
        }

        public static void UpdateContact(UserContactcs contact, int userId)
        {
            using (var model = new TelegramModel())
            {
                Contacts toUpdate = model.Contacts.Where(x => x.Id == contact.Id).FirstOrDefault();
                if (toUpdate is null) return;

                toUpdate.Name = contact.Name;
                toUpdate.LastName = contact.Surname;
                toUpdate.IsNotifsIsOn = contact.IsNotificationsIsOn;
                toUpdate.IsBlocked = contact.IsBlockedUserBlocked;

                model.SaveChanges();
            }
        }

        public static UserContactcs GetLastAddedContactByUser(int userId)
        {
            using (var model = new TelegramModel())
            {
                List<Contacts> toGet = model.Contacts.Where(x => x.UserId == userId).ToList();
                if (toGet is null) return null;

                return GetContactById(toGet.Last().Id);
            }
        }

        //Contacts


        // SETTINGS OPTIONS

        public static MainSettings GetSettingsByUserId(int userId)
        {
            MainSettings res = new MainSettings();

            using (var model = new TelegramModel())
            {
                //Or user Id
                Settings setting = model.Settings
                    .FirstOrDefault(x => x.UserId == userId);
                if (setting is null) return null;

                res.Id = setting.Id;


                res.NotSettings = GetNotifSettingsBySettingsId(setting.Id);

                res.ChatsSettings = GetChatSettingsBySettingsId(setting.Id);
                res.AdvSettings = GetAdvansedSettingsById(setting.Id);
                res.PrivacySettings = GetPrivacySettings(setting.Id);
            }
            return res;
        }

        public static void UpdatePrivacySettings(PrivAndSecSettings settings)
        {
            //settings = new PrivAndSecSettings();

            using (var model = new TelegramModel())
            {
                foreach (var privSet in model.PrivacySetting)
                {
                    if (privSet.Id == settings.Id)
                    {
                        //passcode
                        //privSet.Passcode = settings.LocalPasscode;

                        //selfDeletetime
                        privSet.AwayForTypeId = GetSelfDeleteTypeIdByType(settings.SelfDeleteTime);
                        //phone priv
                        UpdatePhoneNumberSetting(settings.Id, settings.PhonePrivacy);

                        //profile priv
                        UpdateProfilePhoto(settings.Id, settings.ProfPhotoPrivacy);

                        //forward priv
                        UpdateForwardMessages(settings.Id, settings.ForwardMesPrivacy);

                        //message priv
                        UpdateMessagesPrivacy(settings.Id, settings.MessagesPrivacy);

                        //date birth priv
                        UpdateDateofBirth(settings.Id, settings.DateBirthPrivacy);

                        //bio priv
                        UpdateBioSetting(settings.Id, settings.BioPrivacy);

                        break;
                    }
                }

                model.SaveChanges();
            }
        }



        private static int GetSelfDeleteTypeIdByType(AwayForTime deleteType)
        {
            using (var model = new TelegramModel())
            {
                foreach (var type in model.AwayForType)
                {
                    if (type.Name == deleteType.ToString())
                    {
                        model.SaveChanges();
                        return type.Id;
                    }
                }
            }
            return 1;
        }

        public static PrivAndSecSettings GetPrivacySettings(int settingId)
        {
            PrivAndSecSettings res = new PrivAndSecSettings();

            using (var model = new TelegramModel())
            {
                PrivacySetting settings = model.PrivacySetting.Where(x => x.SettingId == settingId).FirstOrDefault();
                if (settings is null) return null;

                res.Id = settings.Id;
                res.LocalPasscode = settings.Passcode;
                res.SelfDeleteTime = GetAwayForTimeById((int)settings.AwayForTypeId);

                res.PhonePrivacy = GetPhoneNumberSettingsById((int)settings.PhoneNumberSetId, settingId);
                res.LastSeenPrivacy = GetLastSeenSubById((int)settings.LastSeenSetId, settingId);
                res.ProfPhotoPrivacy = GetProfPhotoSub((int)settings.PhoneNumberSetId, settingId);
                res.ForwardMesPrivacy = GetForwardMesSubById((int)settings.ForwardMesSetId, settingId);
                res.MessagesPrivacy = GetMessagesPrivById((int)settings.MessagesSetId);
                res.DateBirthPrivacy = GetBirthDateById((int)settings.DateOfBirthSetId, settingId);
                res.BioPrivacy = GetBioById((int)settings.BioSetId, settingId);
            }
            return res;
        }

        private static BioSub GetBioById(int id, int settingId)
        {
            BioSub res = new BioSub();

            using (var model = new TelegramModel())
            {
                BioSettings forMes = model.BioSettings.Where(x => x.Id == id).FirstOrDefault();
                if (forMes is null) return null;

                res.ShareType = GetShareWithById((int)forMes.WhoSeeId);
                res.ShareWithExps = GetChosenShareContacts(true, settingId, SubSettingType.Bio);
                res.NeverShareExps = GetChosenShareContacts(false, settingId, SubSettingType.Bio);
            }
            return res;
        }

        private static DateOfBirthSub GetBirthDateById(int id, int settingId)
        {
            DateOfBirthSub res = new DateOfBirthSub();

            using (var model = new TelegramModel())
            {
                DateOfBirthSettings forMes = model.DateOfBirthSettings.Where(x => x.Id == id).FirstOrDefault();
                if (forMes is null) return null;

                res.ShareType = GetShareWithById((int)forMes.WhoSeeId);
                res.ShareWithExps = GetChosenShareContacts(true, settingId, SubSettingType.DateOfBirth);
                res.NeverShareExps = GetChosenShareContacts(false, settingId, SubSettingType.DateOfBirth);
            }
            return res;
        }

        private static MessagesSub GetMessagesPrivById(int id)
        {
            MessagesSub res = new MessagesSub();

            using (var model = new TelegramModel())
            {
                MessagesSettings mesSet = model.MessagesSettings.Where(x => x.Id == id).FirstOrDefault();
                if (mesSet is null) return null;

                res.WhoCanSend = GetShareWithById((int)mesSet.WhoSeeId);
            }
            return res;
        }

        private static ForwardedMessagesSub GetForwardMesSubById(int id, int settingId)
        {
            ForwardedMessagesSub res = new ForwardedMessagesSub();

            using (var model = new TelegramModel())
            {
                ForwardMessagesSettings forMes = model.ForwardMessagesSettings.Where(x => x.Id == id).FirstOrDefault();
                if (forMes is null) return null;

                res.ShareType = GetShareWithById((int)forMes.WhoSeeId);
                res.ShareWithExps = GetChosenShareContacts(true, settingId, SubSettingType.ForwardMessage);
                res.NeverShareExps = GetChosenShareContacts(false, settingId, SubSettingType.ForwardMessage);
            }
            return res;
        }

        private static ProfilePhotosSub GetProfPhotoSub(int id, int settingId)
        {
            ProfilePhotosSub res = new ProfilePhotosSub();

            using (var model = new TelegramModel())
            {
                ProfilePhotoSettings photo = model.ProfilePhotoSettings.Where(x => x.Id == id).FirstOrDefault();
                if (photo is null) return null;

                res.PublicPhotoPath = photo.PublicPhotoId is null ? null : GetPublicPhotoPathName((int)photo.PublicPhotoId);

                res.ShareType = GetShareWithById((int)photo.WhoSeeId);
                res.ShareWithExps = GetChosenShareContacts(true, settingId, SubSettingType.PhoneNumber);
                res.NeverShareExps = GetChosenShareContacts(false, settingId, SubSettingType.PhoneNumber);
            }
            return res;
        }

        private static string GetPublicPhotoPathName(int userImageId)
        {
            using (var model = new TelegramModel())
            {
                UserImage img = model.UserImage.Where(x => x.Id == userImageId).FirstOrDefault();
                if (img is null) return string.Empty;

                return img.Name;
            }
            return string.Empty;
        }

        private static LastSeenSub GetLastSeenSubById(int id, int settingsId)
        {
            LastSeenSub res = new LastSeenSub();

            using (var model = new TelegramModel())
            {
                LastSeenSettings lastSeen = model.LastSeenSettings.Where(x => x.Id == id).FirstOrDefault();
                if (lastSeen is null) return null;

                res.IsHideReadAction = (bool)lastSeen.IsHideReadTime;
                res.ShareType = GetShareWithById((int)lastSeen.WhoSeeId);
                res.ShareWithExps = GetChosenShareContacts(true, settingsId, SubSettingType.LastSeen);
                res.NeverShareExps = GetChosenShareContacts(false, settingsId, SubSettingType.LastSeen);
            }
            return res;
        }

        private static PhoneNumberSub GetPhoneNumberSettingsById(int id, int settingsId)
        {
            PhoneNumberSub res = new PhoneNumberSub();

            using (var model = new TelegramModel())
            {
                PhoneNumberSettings settings = model.PhoneNumberSettings.Where(x => x.Id == id).FirstOrDefault();
                if (settings is null) return null;

                res.ShareType = GetShareWithById((int)settings.WhoSeeId);
                res.WhoCanSearch = GetAllOrNooneById((int)settings.WhoCanFindNumber);
                res.ShareWithExps = GetChosenShareContacts(true, settingsId, SubSettingType.PhoneNumber);
                res.NeverShareExps = GetChosenShareContacts(false, settingsId, SubSettingType.PhoneNumber);
            }
            return res;
        }

        public static void UpdateAdvanced(UserSettings.SettingsTypes.AdvancedSettings settings)
        {
            using (var model = new TelegramModel())
            {
                AdvancedSettings advModel = model.AdvancedSettings.Where(x => x.Id == settings.Id).FirstOrDefault();
                if (advModel is null) return;

                advModel.IsShowChatName = settings.IsShowChatName;
                advModel.IsTotalUnredCount = settings.IsShowTotalUnReads;
                advModel.IsUseSysWIndowFrame = settings.IsUserWindowSysFrame;
                advModel.IsShowTrayIcon = settings.IsShowTrayIcon;
                advModel.IsShowTaskBarIcon = settings.IsShowTaskbarIcon;
                advModel.IsCloseToTaskBar = settings.IsCloseToTaskbar;
                advModel.IsLaunchWhenStart = settings.LaunchTelegram;
                advModel.IsUpdateAutomatically = settings.IsUpdateAutomatically;
                advModel.IsInstallBetaVersion = settings.IsInstallBetaVersion;

                model.SaveChanges();
            }
        }


        private static UserSettings.SettingsTypes.AdvancedSettings GetAdvansedSettingsById(int settingsId)
        {
            UserSettings.SettingsTypes.AdvancedSettings res = new UserSettings.SettingsTypes.AdvancedSettings();

            using (var model = new TelegramModel())
            {
                AdvancedSettings settings = model.AdvancedSettings.Where(x => x.SettingId == settingsId).FirstOrDefault();
                if (settings is null) return null;

                res.Id = settings.Id;
                res.IsAskDownloadPath = (bool)settings.IsAskDownloadPath;
                res.IsShowChatName = (bool)settings.IsShowChatName;
                res.IsShowTotalUnReads = (bool)settings.IsTotalUnredCount;
                res.IsUserWindowSysFrame = (bool)settings.IsUseSysWIndowFrame;
                res.IsShowTrayIcon = (bool)settings.IsShowTrayIcon;
                res.IsShowTaskbarIcon = (bool)settings.IsShowTaskBarIcon;
                res.IsCloseToTaskbar = (bool)settings.IsCloseToTaskBar;
                res.LaunchTelegram = (bool)settings.IsLaunchWhenStart;
                res.IsUpdateAutomatically = (bool)settings.IsUpdateAutomatically;
                res.IsInstallBetaVersion = (bool)settings.IsInstallBetaVersion;
            }

            return res;
        }

        public static UserSettings.SettingsTypes.ChatSettings GetChatSettingsBySettingsId(int settingsId)
        {
            UserSettings.SettingsTypes.ChatSettings res = new UserSettings.SettingsTypes.ChatSettings();

            using (var model = new TelegramModel())
            {
                ChatSettings settings = model.ChatSettings.Where(x => x.SettingId == settingsId).FirstOrDefault();
                if (settings is null) return null;

                res.Id = settings.Id;
                res.Theme = GetThemeById((int)settings.ThemeId);
                res.ChosenColor = GetColorHelprtById((int)settings.UserColorId);// user color id
                res.NightMode = GetNightModeById((int)settings.AutoNightId);
                res.FontName = settings.Font;
                res.IsSendWithEnter = (bool)settings.IsSentWithEnter;
                res.Wallpaper = GetChatWallpaperByChatSettingsId(res.Id);// GetChatWallpaperById(settings.Bg);
                res.PossibleWallpapers = GetPossibleWallpapersForChatSetting(settingsId); // CHECK IF USERID === SETTINGID
            }
            return res;
        }


        private static ChatWallpaper GetChatWallpaperByChatSettingsId(int chatSettingId)
        {
            using (var model = new TelegramModel())
            {
                ChatSettings setting = model.ChatSettings.FirstOrDefault(x => x.Id == chatSettingId);
                if (setting is null || setting.BgName is null) return null;
                return GetChatWallpaperById((int)setting.BgName);
            }
        }

        private static ChatWallpaper GetChatWallPaperIdByName(string name)
        {
            ChatWallpaper res = new ChatWallpaper();
            using (var model = new TelegramModel())
            {
                foreach (var temp in model.ChatBG)
                {
                    if (temp.Name == name)
                    {
                        res.Id = temp.Id;
                        res.WallpaperName = temp.Name;
                        res.IsBlurred = (bool)temp.IsBlurred;
                    }
                }
            }
            return res;
        }
        private static List<string> GetPossibleWallpapersForChatSetting(int userId)
        {
            List<string> res = new List<string>();
            using (var model = new TelegramModel())
            {
                foreach (var temp in model.ChatBG)
                {
                    res.Add(temp.Name);
                }
            }
            return res;
        }

        private static ColorHelper GetColorHelprtById(int id)
        {
            using (var model = new TelegramModel())
            {
                UserColor color = model.UserColor.Where(x => x.Id == id).FirstOrDefault();
                if (color is null) return null;

                return new ColorHelper(id, (byte)color.R, (byte)color.G, (byte)color.B);
            }
        }

        public static void UpdateColor(ColorHelper color)
        {
            using (var model = new TelegramModel())
            {
                UserColor toChange = model.UserColor.Where(x => x.Id == color.Id).FirstOrDefault();
                if (toChange is null) return;

                toChange.R = color.R;
                toChange.G = color.G;
                toChange.B = color.B;

                model.SaveChanges();
            }
        }

        public static void AddUserBasicColor(int userId)
        {
            using (var model = new TelegramModel())
            {
                ColorHelper color = new ColorHelper(userId);

                UserColor toAdd = new UserColor();

                toAdd.UserId = userId;
                toAdd.R = color.R;
                toAdd.G = color.G;
                toAdd.B = color.B;

                model.UserColor.Add(toAdd);

                model.SaveChanges();
            }
        }

        private static List<string> GetPossibleChatWallPapersByChatId(int chatId)
        {
            List<string> res = new List<string>();

            using (var model = new TelegramModel())
            {
                foreach (var posWalp in model.PossibleChatBGs)
                {
                    if (posWalp.ChatId == chatId)
                    {
                        ChatWallpaper paper = GetChatWallpaperById((int)posWalp.ChatBgId);
                        if (paper is null) continue;
                        res.Add(paper.WallpaperName);
                    }
                }
            }
            return res;
        }

        public static NotificationSettings GetNotifSettingsBySettingsId(int settingId)
        {
            using (var model = new TelegramModel())
            {
                NotificatioonsAndSound settings = model.NotificatioonsAndSound.FirstOrDefault(x => x.SettingId == settingId);
                if (settings is null) return null;

                return new NotificationSettings(settings.Id, (bool)settings.DesktopNotification, (bool)settings.FlashTaskBar,
                    (bool)settings.AllowSound, (bool)settings.PrivateChat, (bool)settings.PinnedMessage);
            }
        }


        public static void AddSettings(int userId)
        {
            //Addsettings
            AddBaseSettings(userId);

            //Privacy settings
            AddPrivacySettings(userId);

            //Chat settings
            AddChatSettings(userId);

            //Advanced settings
            AddAdvancedSettings(userId);

            //Notifications and sounds settings
            AddNotificationSettings(userId);

        }

        public static void AddBaseSettings(int userId)
        {
            using (var model = new TelegramModel())
            {
                Settings settings = new Settings();

                settings.UserId = userId;
                model.Settings.Add(settings);

                model.SaveChanges();
            }
        }

        //Chat settings
        private static void AddChatSettings(int newSettingId)
        {
            using (var model = new TelegramModel())
            {
                ChatSettings settings = new ChatSettings();

                settings.SettingId = newSettingId;
                settings.ThemeId = 1;
                settings.UserColorId = 1;// new ColorHelper();
                settings.AutoNightId = 1;
                settings.Font = "Time New Roman";
                settings.BgName = null; ///
                settings.IsSentWithEnter = true;

                model.ChatSettings.Add(settings);
                model.SaveChanges();
            }
        }

        public static void UpdateChatSettings(UserSettings.SettingsTypes.ChatSettings settings)
        {
            using (var model = new TelegramModel())
            {
                ChatSettings temp = model.ChatSettings.Where(x => x.Id == settings.Id).FirstOrDefault();
                if (temp is null) return;

                int? bgIdVal;
                if (settings.Wallpaper.Id <= 0) bgIdVal = null;
                else bgIdVal = settings.Wallpaper.Id;

                temp.ThemeId = GetIdByTheme(settings.Theme);
                temp.UserColorId = settings.ChosenColor.Id;
                temp.AutoNightId = GetAutoNightIdByType(settings.NightMode);
                temp.Font = settings.FontName;
                temp.BgName = bgIdVal;
                temp.IsSentWithEnter = settings.IsSendWithEnter;

                //UpdateUserColor
                UpdateUserColor(settings.ChosenColor);

                model.SaveChanges();
            }
        }

        private static void UpdateUserColor(TelegramLib.Helpers.ColorHelper color)
        {
            using (var model = new TelegramModel())
            {
                UserColor toUpdate = model.UserColor.FirstOrDefault(x => x.Id == color.Id);
                if (toUpdate is null) return;

                toUpdate.R = color.R;
                toUpdate.G = color.G;
                toUpdate.B = color.B;

                model.SaveChanges();
            }
        }

        private static int GetAutoNightIdByType(AutoNightMode type)
        {
            using (var model = new TelegramModel())
            {
                foreach (var autoNight in model.AutoNight)
                {
                    if (autoNight.Name == type.ToString()) return autoNight.Id;
                }
            }
            return 1;
        }

        private static int GetIdByTheme(ThemeType theme)
        {
            using (var model = new TelegramModel())
            {
                foreach (var tempTheme in model.Theme)
                {
                    if (tempTheme.Name == theme.ToString()) return tempTheme.Id;
                }
            }
            return 1;
        }

        //NOTIFICATION SETTINGS
        private static void AddNotificationSettings(int newSettingId)
        {
            using (var model = new TelegramModel())
            {
                NotificatioonsAndSound settings = new NotificatioonsAndSound();

                settings.SettingId = newSettingId;
                settings.DesktopNotification = false;
                settings.FlashTaskBar = false;
                settings.AllowSound = false;
                settings.PrivateChat = false;
                settings.PinnedMessage = false;

                model.NotificatioonsAndSound.Add(settings);

                model.SaveChanges();
            }
        }
        public static void UpdateNotificationSoundsSettings(/*int settingsId,*/ NotificationSettings newSettings)
        {
            using (var model = new TelegramModel())
            {
                NotificatioonsAndSound settings =
                    model.NotificatioonsAndSound.Where(x => x.SettingId == newSettings.Id).FirstOrDefault();

                if (settings is null) return;

                settings.DesktopNotification = newSettings.IsDesktopNotifications;
                settings.FlashTaskBar = newSettings.IsFlashTaskBar;
                settings.AllowSound = newSettings.IsAllowSounds;
                settings.PrivateChat = newSettings.IsPrivateChats;
                settings.PinnedMessage = newSettings.IsPinnedMessages;

                model.SaveChanges();
            }
        }
        //NOTIFICATION SETTINGS


        private static void AddAdvancedSettings(int newSettingId)
        {
            using (var model = new TelegramModel())
            {
                AdvancedSettings settings = new AdvancedSettings();

                settings.SettingId = newSettingId;
                settings.IsShowChatName = true;
                settings.IsTotalUnredCount = false;
                settings.IsUseSysWIndowFrame = false;
                settings.IsShowTrayIcon = false;
                settings.IsShowTaskBarIcon = false;
                settings.IsCloseToTaskBar = false;
                settings.IsLaunchWhenStart = false;
                settings.IsUpdateAutomatically = false;
                settings.IsInstallBetaVersion = false;
                settings.IsAskDownloadPath = false;

                model.AdvancedSettings.Add(settings);

                model.SaveChanges();
            }
        }
        private static void UpdateAdvancedSettings(int newSettingId,
            UserSettings.SettingsTypes.AdvancedSettings newSettings)
        {
            using (var model = new TelegramModel())
            {
                AdvancedSettings settings = model.AdvancedSettings.Where(x => x.SettingId == newSettingId).FirstOrDefault();
                if (settings is null) return;

                settings.IsShowChatName = newSettings.IsShowChatName;
                settings.IsTotalUnredCount = newSettings.IsShowTotalUnReads;
                settings.IsUseSysWIndowFrame = newSettings.IsUserWindowSysFrame;
                settings.IsShowTrayIcon = newSettings.IsShowTrayIcon;
                settings.IsShowTaskBarIcon = newSettings.IsShowTaskbarIcon;
                settings.IsCloseToTaskBar = newSettings.IsCloseToTaskbar;
                settings.IsLaunchWhenStart = newSettings.LaunchTelegram;
                settings.IsUpdateAutomatically = newSettings.IsUpdateAutomatically;
                settings.IsInstallBetaVersion = newSettings.IsInstallBetaVersion;

                model.SaveChanges();
            }
        }

        //Privacy
        public static void AddPrivacySettings(int newSettingId)
        {
            //Settings params to add in new privacySettings row
            AddPhoneNumberSetting();
            AddLastSeenSetting();
            AddProfilePhotoSetting();
            AddForwardMessageSettings();
            AddDateOfBirthSettings();
            AddBioSettings();
            AddMessagesSettings();

            using (var model = new TelegramModel())
            {
                PrivacySetting settings = new PrivacySetting();

                settings.SettingId = newSettingId;
                settings.PhoneNumberSetId = newSettingId;
                settings.LastSeenSetId = newSettingId;
                settings.ProfPhotoSetId = newSettingId;
                settings.ForwardMesSetId = newSettingId;
                settings.MessagesSetId = newSettingId;
                settings.DateOfBirthSetId = newSettingId;
                settings.BioSetId = newSettingId;
                settings.AwayForTypeId = 6;

                model.PrivacySetting.Add(settings);
                model.SaveChanges();
            }
        }
        public static void UpdateAwayForType(int settingId, PrivAndSecSettings settings)
        {
            using (var model = new TelegramModel())
            {
                PrivacySetting setting = model.PrivacySetting.Where(x => x.SettingId == settingId).FirstOrDefault();
                if (setting is null) return;

                setting.AwayForTypeId = GetAwayForTypeIdByName(settings.SelfDeleteTime.ToString());
                model.SaveChanges();
            }
        }


        private static void AddPhoneNumberSetting()
        {
            using (var model = new TelegramModel())
            {
                PhoneNumberSettings settings = new PhoneNumberSettings();

                settings.WhoSeeId = 1;
                settings.WhoCanFindNumber = 1;
                model.PhoneNumberSettings.Add(settings);
                model.SaveChanges();
            }
        }
        public static void UpdatePhoneNumberSetting(int settingId, PhoneNumberSub sub)
        {
            using (var model = new TelegramModel())
            {
                PhoneNumberSettings settings = model.PhoneNumberSettings.Where(
                    x => x.Id == settingId).FirstOrDefault();
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.ShareType.ToString());
                settings.WhoCanFindNumber = GetWhoSeeIdByShareWithType(sub.WhoCanSearch.ToString());

                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.PhoneNumber), sub.ShareWithExps, true);
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.PhoneNumber), sub.ShareWithExps, false);

                model.SaveChanges();
            }
        }

        private static void UpdateChosenPrivContacts(int settingId, int settingTypeId,
            List<UserContactcs> contacts, bool isShare)
        {
            //Remove all rows that confirm conditions
            List<ChosenPrivacyContacts> toRemove = GetChosenPrivContaactWithCondition(settingId, settingTypeId, isShare);

            using (var model = new TelegramModel())
            {
                //Remove
                foreach (var remove in toRemove)
                {
                    model.ChosenPrivacyContacts.Remove(remove);
                }

                //Add
                AddChosenPrivContactsByUserContacts(contacts, settingTypeId, isShare, settingId);
                model.SaveChanges();
            }


            //Add all of them from 
        }

        private static void AddChosenPrivContactsByUserContacts(
            List<UserContactcs> contacts, int settingTypeId, bool isShare, int settingId)
        {
            using (var model = new TelegramModel())
            {
                for (int i = 0; i < contacts.Count; i++)
                {
                    ChosenPrivacyContacts toAdd = new ChosenPrivacyContacts();

                    toAdd.ContactId = contacts[i].Id;
                    toAdd.SettingTypeId = settingTypeId;
                    toAdd.IsShare = isShare;
                    toAdd.SttingId = settingId;

                    model.ChosenPrivacyContacts.Add(toAdd);
                }

                model.SaveChanges();
            }
        }

        private static List<ChosenPrivacyContacts> GetChosenPrivContaactWithCondition(int settingId, int settingTypeId, bool isShare)
        {
            List<ChosenPrivacyContacts> res = new List<ChosenPrivacyContacts>();

            using (var model = new TelegramModel())
            {
                foreach (var contact in model.ChosenPrivacyContacts)
                {
                    if (contact.SettingTypeId == settingTypeId &&
                       contact.SttingId == settingId &&
                       contact.IsShare == isShare)
                    {
                        res.Add(contact);
                    }
                }
            }

            return res;
        }

        private static void AddLastSeenSetting()
        {
            using (var model = new TelegramModel())
            {
                LastSeenSettings settings = new LastSeenSettings();

                settings.WhoSeeId = 1;
                settings.IsHideReadTime = false;

                model.LastSeenSettings.Add(settings);
                model.SaveChanges();
            }
        }
        public static void UpdateLastSeenSetting(int settingId, LastSeenSub sub)
        {
            using (var model = new TelegramModel())
            {
                LastSeenSettings settings = model.LastSeenSettings.Where(
                    x => x.Id == settingId).FirstOrDefault();
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.ShareType.ToString());
                settings.IsHideReadTime = sub.IsHideReadAction;
                model.SaveChanges();
            }
        }


        private static void AddProfilePhotoSetting()
        {
            using (var model = new TelegramModel())
            {
                ProfilePhotoSettings settings = new ProfilePhotoSettings();

                settings.WhoSeeId = 1;
                settings.PublicPhotoId = null;

                model.ProfilePhotoSettings.Add(settings);
                model.SaveChanges();
            }
        }
        public static void UpdateProfilePhoto(int settingId, ProfilePhotosSub sub)
        {
            using (var model = new TelegramModel())
            {
                ProfilePhotoSettings settings = model.ProfilePhotoSettings.Where(
                    x => x.Id == settingId).FirstOrDefault();
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.ShareType.ToString());
                settings.PublicPhotoId = GetUserImageByName(sub.PublicPhotoPath);


                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Profile), sub.ShareWithExps, true);
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Profile), sub.ShareWithExps, true);

                model.SaveChanges();
            }
        }

        private static void AddForwardMessageSettings()
        {
            using (var model = new TelegramModel())
            {
                ForwardMessagesSettings settings = new ForwardMessagesSettings();

                settings.WhoSeeId = 1;

                model.ForwardMessagesSettings.Add(settings);
                model.SaveChanges();
            }
        }
        public static void UpdateForwardMessages(int settingId, ForwardedMessagesSub sub)
        {
            using (var model = new TelegramModel())
            {
                ForwardMessagesSettings settings = model.ForwardMessagesSettings.Where(
                    x => x.Id == settingId).FirstOrDefault();
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.ShareType.ToString());

                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.ForwardMessage), sub.ShareWithExps, true);
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.ForwardMessage), sub.ShareWithExps, true);

                model.SaveChanges();
            }
        }

        private static void AddDateOfBirthSettings()
        {
            using (var model = new TelegramModel())
            {
                DateOfBirthSettings settings = new DateOfBirthSettings();

                settings.WhoSeeId = 1;

                model.DateOfBirthSettings.Add(settings);
                model.SaveChanges();
            }
        }
        public static void UpdateDateofBirth(int settingId, DateOfBirthSub sub)
        {
            using (var model = new TelegramModel())
            {
                DateOfBirthSettings settings = model.DateOfBirthSettings.Where(
                    x => x.Id == settingId).FirstOrDefault();
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.ShareType.ToString());

                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.DateOfBirth), sub.ShareWithExps, true);
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.DateOfBirth), sub.ShareWithExps, true);

                model.SaveChanges();
            }
        }


        private static void AddBioSettings()
        {
            using (var model = new TelegramModel())
            {
                BioSettings settings = new BioSettings();

                settings.WhoSeeId = 1;

                model.BioSettings.Add(settings);
                model.SaveChanges();
            }
        }
        public static void UpdateBioSetting(int settingId, BioSub sub)
        {
            using (var model = new TelegramModel())
            {
                BioSettings settings = model.BioSettings.Where(
                    x => x.Id == settingId).FirstOrDefault();
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.ShareType.ToString());

                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Bio), sub.ShareWithExps, true);
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Bio), sub.ShareWithExps, true);

                model.SaveChanges();
            }
        }


        private static void AddMessagesSettings()
        {
            using (var model = new TelegramModel())
            {
                MessagesSettings settings = new MessagesSettings();

                settings.WhoSeeId = 1;

                model.MessagesSettings.Add(settings);
                model.SaveChanges();
            }
        }

        public static void UpdateMessagesPrivacy(int settingId, MessagesSub sub)
        {
            using (var model = new TelegramModel())
            {
                MessagesSettings settings = model.MessagesSettings.Where(x => x.Id == settingId).FirstOrDefault();
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.WhoCanSend.ToString());

                /*                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Messages), 
                                    sub.ShareWithExps, true);
                                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Messages), 
                                    sub.ShareWithExps, true);*/

                model.SaveChanges();
            }
        }


        private static string GetAwayForTypeNameById(int id)
        {
            using (var model = new TelegramModel())
            {
                foreach (var type in model.AwayForType)
                {
                    if (type.Id == id) return type.Name;
                }
            }
            return "Everybody";
        }
        private static int GetAwayForTypeIdByName(string awayForType)
        {
            using (var model = new TelegramModel())
            {
                foreach (var type in model.AwayForType)
                {
                    if (type.Name == awayForType) return type.Id;
                }
            }
            return 1;
        }
        private static int GetWhoSeeIdByShareWithType(string shareType)
        {
            using (var model = new TelegramModel())
            {
                foreach (var type in model.WhoCanSeeType)
                {
                    if (shareType.ToString() == type.Name) return type.Id;
                }
            }
            return 1;
        }
        private static int? GetUserImageByName(string name)
        {
            using (var model = new TelegramModel())
            {
                foreach (var img in model.UserImage)
                {
                    if (img.Name == name) return img.Id;
                }
            }
            return null;
        }

        private static List<mainClass.UserParams.UserImage> GetUserImagesByUserId(int userId)
        {
            List<mainClass.UserParams.UserImage> res = new List<mainClass.UserParams.UserImage>();
            using (var model = new TelegramModel())
            {
                foreach (var img in model.UserImage)
                {
                    if (img.UserId == userId)
                    {
                        mainClass.UserParams.UserImage toAdd = new mainClass.UserParams.UserImage();

                        toAdd.Name = img.Name;
                        toAdd.Date = (DateTime)img.AddDate;

                        res.Insert(0, toAdd); 
                        //res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static ColorHelper GetUserColorByUserId(int userId)
        {
            using (var model = new TelegramModel())
            {
                foreach (var color in model.UserColor)
                {
                    if (color.UserId == userId)
                    {
                        return new ColorHelper(userId, (byte)color.R, (byte)color.G, (byte)color.B);
                    }
                }
            }
            return new ColorHelper(userId);
        }

        private static ThemeType GetThemeById(int themeId)
        {
            using (var model = new TelegramModel())
            {
                Theme theme = model.Theme.Where(x => x.Id == themeId).FirstOrDefault();
                if (theme is null) return ThemeType.Classic;

                for (int i = 1; i <= (int)ThemeType.Night; i++)
                {
                    if (theme.Name == ((ThemeType)i).ToString())
                    {
                        return (ThemeType)i;
                    }
                }
            }
            return ThemeType.Classic;
        }

        private static AutoNightMode GetNightModeById(int nightModeId)
        {
            using (var model = new TelegramModel())
            {
                AutoNight autoNight = model.AutoNight.Where(x => x.Id == nightModeId).FirstOrDefault();
                if (autoNight is null) return AutoNightMode.Off;

                for (int i = 1; i <= (int)AutoNightMode.System; i++)
                {
                    if (autoNight.Name == ((AutoNightMode)i).ToString())
                    {
                        return (AutoNightMode)i;
                    }
                }
            }
            return AutoNightMode.Off;
        }

        private static ChatWallpaper GetChatWallpaperById(int chatBgId)
        {
            ChatWallpaper res = new ChatWallpaper();

            using (var model = new TelegramModel())
            {
                ChatBG bg = model.ChatBG.FirstOrDefault(x => x.Id == chatBgId);
                if (bg is null) return null;

                res.Id = bg.Id;
                res.WallpaperName = bg.Name;
                res.IsBlurred = (bool)bg.IsBlurred;
            }

            return res;
        }

        private static AwayForTime GetAwayForTimeById(int id)
        {
            using (var model = new TelegramModel())
            {
                AwayForType type = model.AwayForType.Where(x => x.Id == id).FirstOrDefault();
                if (type is null) return AwayForTime.ThreeMonths;

                for (int i = 0; i <= (int)AwayForTime.TwentyFourMonths; i++)
                {
                    if (type.Name == ((AwayForTime)i).ToString())
                    {
                        return (AwayForTime)i;
                    }
                }
            }
            return AwayForTime.ThreeMonths;
        }

        private static ShareWith GetShareWithById(int id)
        {
            using (var model = new TelegramModel())
            {
                WhoCanSeeType type = model.WhoCanSeeType.Where(x => x.Id == id).FirstOrDefault();
                if (type is null) return ShareWith.Contacts;

                for (int i = 0; i <= (int)ShareWith.Nobody; i++)
                {
                    if (type.Name == ((ShareWith)i).ToString())
                    {
                        return (ShareWith)i;
                    }
                }
            }
            return ShareWith.Contacts;
        }

        private static AllOrNone GetAllOrNooneById(int id)
        {
            using (var model = new TelegramModel())
            {
                WhoCanSeeType type = model.WhoCanSeeType.Where(x => x.Id == id).FirstOrDefault();
                if (type is null) return AllOrNone.Contacts;

                for (int i = 0; i <= (int)AllOrNone.Contacts; i++)
                {
                    if (type.Name == ((AllOrNone)i).ToString())
                    {
                        return (AllOrNone)i;
                    }
                }
            }
            return AllOrNone.Contacts;
        }

        private static List<UserContactcs> GetChosenShareContacts(bool isShare, int settingId, SubSettingType type)
        {
            List<UserContactcs> res = new List<UserContactcs>();
            int subSettingId = GetSubSettingTypeByEnum(type);

            using (var model = new TelegramModel())
            {
                foreach (var contact in model.ChosenPrivacyContacts)
                {
                    if (contact.IsShare == isShare &&
                        contact.SettingTypeId == subSettingId &&
                        contact.SttingId == settingId)
                    {
                        res.Add(GetContactById((int)contact.ContactId));
                    }
                }
            }

            return res;
        }

        private static int GetSubSettingTypeByEnum(SubSettingType type)
        {
            using (var model = new TelegramModel())
            {
                PrivacySettingType res = model.PrivacySettingType.Where(
                    x => x.Name == type.ToString()).FirstOrDefault();
                if (res is null) return 1;
                return res.Id;

            }

            return 1;
        }



        //SETTINGS OPTIONS


        //Chats
        public static bool AddMessage(UserChat chat, TelegramLib.MainClasses.Messages.Message message)
        {
            if (message is MediaAction mediaAction) AddChatMedia(chat, mediaAction);

            using (var model = new TelegramModel())
            {
                Messages toAdd = new Messages();

                toAdd.ChatId = chat.Id;
                toAdd.SenderId = message.SenderUserId;
                toAdd.SentDate = message.SentTime;

                toAdd.Message = message is TextMessage text ? text.Text : null;

                if (message is MediaAction video && video.IsVideo()) toAdd.VideoId = GetVideoIdByName(video.MediaName);
                else toAdd.VideoId = null;

                if (message is MediaAction image && image.IsImage()) toAdd.ImageId = GetChatImageIdByName(image.MediaName);
                else toAdd.ImageId = null;

                if (message is MediaAction gif && gif.IsGif()) toAdd.GifId = GetChatGifIdByName(gif.MediaName);
                else toAdd.GifId = null;

                toAdd.StickerId = message is MediaAction media && media.IsSticker ? GetStickerIdByName(media.MediaName) : null;

                model.Messages.Add(toAdd);
                model.SaveChanges();
            }
            return true;
        }

        public static TelegramLib.MainClasses.Messages.Message GetLastChatMessage(int chatId)
        {
            using (var model = new TelegramModel())
            {
                List<Messages> toGet = model.Messages.Where(x => x.ChatId == chatId).ToList();
                Messages chosen = toGet.Last();

                TelegramLib.MainClasses.Messages.Message res;

                if (chosen.Message is null || chosen.Message == string.Empty) res = new MediaAction();
                else res = new TextMessage();


                res.Id = chosen.Id;
                res.SenderUserId = (int)chosen.SenderId;
                res.SentTime = chosen.SentDate is null ? DateTime.Now : (DateTime)chosen.SentDate;

                if (res is TextMessage text)
                {
                    text.Text = chosen.Message;
                }
                else if (res is MediaAction media)
                {
                    media.IsSticker = chosen.StickerId is null ? false : true;
                    media.MediaName = GetMediaNameByMessageId(chosen.Id);
                }
                return res;
            }
        }

        private static string GetMediaNameByMessageId(int mesId)
        {
            using (var model = new TelegramModel())
            {
                Messages mes = model.Messages.Where(x => x.Id == mesId).FirstOrDefault();
                if (mes is null) return string.Empty;

                if (!(mes.ImageId is null)) return GetChatImageNameById((int)mes.ImageId);
                else if (!(mes.VideoId is null)) return GetChatVideoNameById((int)mes.VideoId);
                else if (!(mes.GIF is null)) return GetChatGifNameById((int)mes.GifId);
                else if (!(mes.StickerId is null)) return GetChatStickerNameById((int)mes.StickerId);
            }
            return string.Empty;
        }


        private static void AddChatMedia(UserChat chat, TelegramLib.MainClasses.Messages.MediaAction message)
        {
            MediaType type = message.IsSticker ? MediaType.Sticker : chat.GetMediaTypeFromFilename(message.MediaName);

            if (type is MediaType.Image) AddChatImage(message.MediaName);
            else if (type is MediaType.Video) AddChatVideo(message.MediaName);
            else if (type is MediaType.Gif) AddChatGif(message.MediaName);
            else if (type is MediaType.Sticker) AddChatSticker(message.MediaName);
        }

        private static int GetChatImageIdByName(string imgName)
        {
            using (var model = new TelegramModel())
            {
                ChatImage img = model.ChatImage.Where(x => x.Name == imgName).FirstOrDefault();
                if (img is null) return 1;
                return img.Id;
            }
        }

        public static int GetChatGifIdByName(string name)
        {
            using (var model = new TelegramModel())
            {
                GIF gif = model.GIF.Where(x => x.Name == name).FirstOrDefault();
                if (gif is null) return 1;
                return gif.Id;
            }
        }

        private static int? GetStickerIdByName(string name)
        {
            using (var model = new TelegramModel())
            {
                StickerImage img = model.StickerImage.OfType<StickerImage>().Where(x => x.Name == name).FirstOrDefault();
                if (img is null) return null;
                return img.Id;
            }
        }

        public static void UpdateChat(UserChat chat)
        {
            //Update messages

            /*            //Remove all messages
                        ClearAllChatMessages(chat.Id);

                        //Add all messages
                        AddMessagesInChat(chat);*/

            //Update chat messages
            UpdateMessages(chat);
            using (var model = new TelegramModel())
            {
                Chat toUpdate = model.Chat.Where(x => x.Id == chat.Id).FirstOrDefault();
                if (toUpdate is null) return;

                toUpdate.BgImageId = GetChatBgIdByName(chat.GetBackground().FileName);
                toUpdate.AutoDeleteId = GetAutoDelIdByType(chat.AutoDel);
                toUpdate.IsMute = chat.Chatter.IsBlockedUserBlocked;

                //Update general BG

                int chosenBgId = GetChosenBgIdByName(toUpdate.Id);
                if (chosenBgId > 0)
                {
                    SetChosenBgInPossibleBGs(toUpdate.Id, chosenBgId);
                }

                model.SaveChanges();
            }
        }

        private static void UpdateMessages(UserChat chat)
        {
            using (var model = new TelegramModel())
            {
                List<Messages> toRemove = new List<Messages>();

                //Get chat messages
                foreach (var mes in model.Messages)
                {
                    if (mes.ChatId == chat.Id &&
                        !chat.Messages.Where(x => x.Id == mes.Id).Any())
                    {
                        toRemove.Add(mes);
                    }
                }

                //Remove chat messages
                foreach (var mes in toRemove)
                {
                    model.Messages.Remove(mes);
                }

                model.SaveChanges();
            }
        }

        private static void AddMessagesInChat(UserChat chat)
        {
            for (int i = 0; i < chat.Messages.Count(); i++)
            {
                AddMessage(chat, chat.Messages[i]);
            }
        }

        private static void ClearAllChatMessages(int chatId)
        {
            using (var model = new TelegramModel())
            {
                List<Messages> toRemove = new List<Messages>();

                //Get chat messages
                foreach (var mes in model.Messages)
                {
                    if (mes.ChatId == chatId)
                    {
                        toRemove.Add(mes);
                    }
                }

                //Remove chat messages
                foreach (var mes in toRemove)
                {
                    model.Messages.Remove(mes);
                }

                model.SaveChanges();
            }
        }

        private static int? GetAutoDelIdByType(Enums.Chat.AutoDeleteType type)
        {
            using (var model = new TelegramModel())
            {
                model.AutoDeleteType delType = model.AutoDeleteType.Where(x => x.Name == type.ToString()).FirstOrDefault();
                if (delType is null) return null;
                return delType.Id;
            }
        }

        private static Enums.Chat.AutoDeleteType GetAutoDelTypeById(int? id)
        {
            if (id is null) return Enums.Chat.AutoDeleteType.Nothing;
            using (var model = new TelegramModel())
            {
                model.AutoDeleteType type = model.AutoDeleteType.FirstOrDefault(x => x.Id == id);
                if (type is null) return Enums.Chat.AutoDeleteType.Nothing;

                return GetAutoDelTypeByTypeString(type.Name);
            }
        }

        private static Enums.Chat.AutoDeleteType GetAutoDelTypeByTypeString(string type)
        {
            for (int i = 0; i < (int)Enums.Chat.AutoDeleteType.OneYear; i++)
            {
                if (type == ((Enums.Chat.AutoDeleteType)i).ToString()) return (Enums.Chat.AutoDeleteType)i;
            }
            return Enums.Chat.AutoDeleteType.Nothing;
        }

        public static int GetChatBgIdByName(string name)
        {
            using (var model = new TelegramModel())
            {
                ChatBG bg = model.ChatBG.Where(x => x.Name == name).FirstOrDefault();
                if (bg is null) return -1;
                return bg.Id;
            }
        }

        public static void ClearChat(int chatId)
        {
            using (var model = new TelegramModel())
            {
                model.Messages
                    .RemoveRange(model.Messages
                        .Where(x => x.ChatId == chatId));
                model.SaveChanges();
            }
        }

        /*        public static void SetBgToChat(int chatId, string bgnName)
                {
                    using (var model = new TelegramModel())
                    {
                        int chatBgId = GetChatBgIdByName(bgnName);
                    }
                }*/

        public static void SetChosenBgInPossibleBGs(int chatId, int chosenBGid)
        {
            using (var model = new TelegramModel())
            {
                //DisGeneral all of states
                SetStateToGeneralParamInPossibleBgs(chatId, false);

                //Set state to one (If does not pressent too)
                if (!IsPossibleBgIsExist(chatId, chosenBGid))
                {
                    //To add possible chat bg
                    AddBgToPossibleBGs(chatId, chosenBGid);
                }
                else
                {
                    // set genral state to it
                    SetGeneralStateToPossibleBg(chatId, chosenBGid);
                }
                model.SaveChanges();

            }
        }

        private static void SetGeneralStateToPossibleBg(int chatId, int bgId)
        {
            using (var model = new TelegramModel())
            {
                PossibleChatBGs toUpdate = model.PossibleChatBGs.Where
                    (x => x.ChatId == chatId && x.ChatBgId == bgId).FirstOrDefault();
                if (toUpdate is null) return;

                toUpdate.IsGeneral = true;
            }
        }

        private static void AddBgToPossibleBGs(int chatId, int bgId)
        {
            using (var model = new TelegramModel())
            {
                PossibleChatBGs toAdd = new PossibleChatBGs();

                toAdd.ChatId = chatId;
                toAdd.ChatBgId = bgId;
                toAdd.IsGeneral = true;

                model.SaveChanges();
            }
        }

        private static bool IsPossibleBgIsExist(int chatId, int bgId)
        {
            using (var model = new TelegramModel())
            {
                return model.PossibleChatBGs.Where(x => x.ChatId == chatId && x.ChatBgId == bgId).Any();
            }
        }

        private static void SetStateToGeneralParamInPossibleBgs(int chatId, bool state)
        {
            using (var model = new TelegramModel())
            {
                foreach (var chatBg in model.PossibleChatBGs)
                {
                    if (chatBg.ChatId == chatId)
                    {
                        chatBg.IsGeneral = state;
                    }
                }
                model.SaveChanges();
            }
        }

        public static void AddChatImage(string imgName)
        {
            if (IsChatImageisExistByName(imgName)) return;

            using (var model = new TelegramModel())
            {
                ChatImage toAdd = new ChatImage();

                toAdd.Name = imgName;

                model.ChatImage.Add(toAdd);
                model.SaveChanges();
            }
        }

        private static bool IsChatImageisExistByName(string name)
        {
            using (var model = new TelegramModel())
            {
                return model.ChatImage.Where(x => x.Name == name).Any();
            }
        }

        public static void AddBlockedContact(int userId, int contactId)
        {
            if (IsContactIsBlocked(userId, contactId)) return;
            using (var model = new TelegramModel())
            {
                BlockedContacts toBlock = new BlockedContacts();

                toBlock.UserId = userId;
                toBlock.BlockedContactId = contactId;

                model.SaveChanges();
            }
        }

        public static bool IsContactIsExist(int userId, int friendId)
        {
            using (var model = new TelegramModel())
            {
                return model.Contacts.Where(x => x.UserId == userId && x.FriendId == friendId).Any();
            }
        }

        public static void UnBlockContact(int userId, int contactId)
        {
            using (var model = new TelegramModel())
            {
                BlockedContacts toRemove = model.BlockedContacts.Where(
                    x => x.UserId == userId && x.BlockedContactId == contactId).FirstOrDefault();
                if (toRemove is null) return;

                model.BlockedContacts.Remove(toRemove);
                model.SaveChanges();
            }
        }

        private static bool IsContactIsBlocked(int userId, int contactId)
        {
            using (var model = new TelegramModel())
            {
                return model.BlockedContacts.Where(
                    x => x.UserId == userId && x.BlockedContactId == contactId).Any();
            }
        }

        public static void RemoveContact(UserContactcs contact)
        {
            //Remove contact chat messages
            //RemoveMessagesBySenderId(contact.Id); //CHECK THIS

            //Remove chat
            //RemoveChatByChatterId(contact.Id);


            //Remove chat and message where chatter is ContactId
            RemoveChatWhereChatterIsContact(contact.Id);

            //Remove COntactsFolder
            RemoveContactsInFolderByContactId(contact.Id);

            //Remove contact from blockedContacts
            RemoveBlockedByContactId(contact.Id);

            //Remove contact from Chosen contacts
            RemoveFromChosenPrivacyContactsByContactId(contact.Id);

            //Remove folder(if they are empty)
            RemoveEmptyFolders(); //CHECK THIS

            //Remove contact
            using (var model = new TelegramModel())
            {
                model.Contacts.RemoveRange(model.Contacts.Where(x => x.Id == contact.Id));
                model.SaveChanges();
            }
        }

        private static void RemoveChatWhereChatterIsContact(int contactId)
        {
            //Get chat
            //remove messages in chat
            //remove chat

            using (var model = new TelegramModel())
            {
                List<Chat> toRemove = model.Chat.Where(x => x.ChatterId == contactId).ToList();

                for (int i = 0; i < toRemove.Count(); i++)
                {
                    //remove all chat messages
                    RemoveMessagesByChatId(toRemove[i].Id);
                }

                //remove chat
                foreach (var chat in toRemove)
                {
                    model.Chat.Remove(chat);
                }
                model.SaveChanges();
            }
        }

        private static void RemoveMessagesByChatId(int chatId)
        {
            using (var model = new TelegramModel())
            {
                List<Messages> toRemove = model.Messages.Where(x => x.ChatId == chatId).ToList();

                model.Messages.RemoveRange(toRemove);
                model.SaveChanges();
            }
        }

        private static void RemoveBlockedByContactId(int contactId)
        {
            using (var model = new TelegramModel())
            {
                List<BlockedContacts> toRemove =
                    model.BlockedContacts.Where(x => x.BlockedContactId == contactId).ToList();

                model.BlockedContacts.RemoveRange(toRemove);
                model.SaveChanges();
            }
        }

        private static void RemoveFromChosenPrivacyContactsByContactId(int contactId)
        {
            using (var model = new TelegramModel())
            {
                List<ChosenPrivacyContacts> toRemove =
                    model.ChosenPrivacyContacts.Where(x => x.ContactId == contactId).ToList();

                model.ChosenPrivacyContacts.RemoveRange(toRemove);
                model.SaveChanges();
            }
        }

        private static void RemoveEmptyFolders()
        {
            using (var model = new TelegramModel())
            {
                List<model.Folder> toRemove = new List<model.Folder>();

                foreach (var folder in model.Folder)
                {
                    if (!IsFolderHasContacts(folder))
                    {
                        toRemove.Add(folder);
                    }
                }

                foreach (var remove in toRemove)
                {
                    model.Folder.Remove(remove);
                }

                model.SaveChanges();
            }
        }

        private static bool IsFolderHasContacts(model.Folder folder)
        {
            using (var model = new TelegramModel())
            {
                return model.ContactsInFolder.Where(x => x.FolderId == folder.Id).Any();
            }
        }

        private static void RemoveContactsInFolderByContactId(int contactId)
        {
            using (var model = new TelegramModel())
            {
                model.ContactsInFolder.RemoveRange(model.ContactsInFolder.Where(x => x.ContactId == contactId));
                model.SaveChanges();
            }
        }

        private static void RemoveChatByChatterId(int chatterId)
        {
            using (var model = new TelegramModel())
            {
                model.Chat.RemoveRange(model.Chat.Where(x => x.ChatterId == chatterId));
                model.SaveChanges();
            }
        }

        private static void RemoveMessagesBySenderId(int senderId)
        {
            using (var model = new TelegramModel())
            {
                model.Messages.RemoveRange(model.Messages.Where(x => x.SenderId == senderId));
                model.SaveChanges();
            }
        }

        private static bool IsChatGifNameIsExist(string gifName)
        {
            using (var model = new TelegramModel())
            {
                return model.GIF.Where(x => x.Name == gifName).Any();
            }
        }

        public static void AddChatGif(string name)
        {
            if (IsChatGifNameIsExist(name)) return;
            using (var model = new TelegramModel())
            {
                GIF toAdd = new GIF();

                toAdd.Name = name;

                model.GIF.Add(toAdd);
                model.SaveChanges();
            }
        }

        private static bool IsChatStickerNameIsExist(string stickerName)
        {
            using (var model = new TelegramModel())
            {
                return model.StickerImage.Where(x => x.Name == stickerName).Any();
            }
        }

        public static void AddChatSticker(string name)
        {
            if (IsChatStickerNameIsExist(name)) return;
            using (var model = new TelegramModel())
            {
                StickerImage toAdd = new StickerImage();

                toAdd.Name = name;

                model.StickerImage.Add(toAdd);
                model.SaveChanges();
            }
        }

        private static bool IsChatVideoNameIsExist(string videoName)
        {
            using (var model = new TelegramModel())
            {
                return model.MessageVideo.Where(x => x.Name == videoName).Any();
            }
        }

        public static void AddChatVideo(string name)
        {
            if (IsChatVideoNameIsExist(name)) return;
            using (var model = new TelegramModel())
            {
                MessageVideo toAdd = new MessageVideo();

                toAdd.Name = name;

                model.MessageVideo.Add(toAdd);
                model.SaveChanges();
            }
        }

        private static int GetVideoIdByName(string name)
        {
            using (var model = new TelegramModel())
            {
                MessageVideo video = model.MessageVideo.Where(x => x.Name == name).FirstOrDefault();
                if (video is null) return 1;
                return video.Id;
            }
        }

        public static TelegramLib.MainClasses.User GetUserByPhoneNumber(string phoneNumber)
        {
            using (var model = new TelegramModel())
            {
                var user = model.User.FirstOrDefault(x =>
                    x.PhoneNumber.Replace("+", "") == phoneNumber.Replace("+", ""));

                if (user is null) return null;
                return GetUserById(user.Id);
            }
        }

        private static string NormalizePhone(string phone) => phone?.Replace("+", "") ?? "";


        public static string GetPhoneNumberFromUser()
        {
            using (var model = new TelegramModel())
            {
                Models.User user = model.User.FirstOrDefault(x => x.Id == 1003);

                return user.PhoneNumber;
            }
        }

        public static void AddChat(int userId, int chatterContactId)
        {
            using (var model = new TelegramModel())
            {
                Chat toAdd = new Chat();

                toAdd.UserId = userId;
                toAdd.ChatterId = chatterContactId;
                toAdd.BgImageId = null;
                toAdd.AutoDeleteId = null;
                toAdd.IsMute = false;

                model.Chat.Add(toAdd);
                model.SaveChanges();
            }
        }

        public static UserChat GetChatByUserAndContactIds(int userId, int contactId)
        {
            using (var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.UserId == userId && x.ChatterId == contactId);
                if (chat is null) return null;
                return new UserChat(chat.Id, GetUserContactById((int)chat.ChatterId),
                    GetMessagesByChatId(chat.Id), GetChosenBgByChatId(chat.Id), GetAutoDelTypeById(chat.AutoDeleteId));
            }
        }

        public static void SetAutoDel(int chatId,
            Enums.Chat.AutoDeleteType type)
        {
            using (var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.Id == chatId);
                if (chat is null) return;

                chat.AutoDeleteId = GetAutoDelIdByType(type);

                model.SaveChanges();
            }
        }

        public static void RemoveAutoDel(int chatId)
        {
            using (var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.Id == chatId);
                if (chat is null) return;

                chat.AutoDeleteId = null;

                model.SaveChanges();
            }
        }

        public static void SetChatWallpaper(ChatBackground toSet, int chatId)
        {
            if (IsPosChatPgIsExisitByChatId(chatId))
            {
                UpdateChatWallpaper(toSet, chatId);
                return;
            }

            string fileName = Path.GetFileName(toSet.FileName);
            using (var model = new TelegramModel())
            {
                //Clear genral state to possible chat ids
                SetStateToGeneralParamInPossibleBgs(chatId, false);

                //Add in possible bgs
                PossibleChatBGs toAdd = new PossibleChatBGs();
                toAdd.ChatId = chatId;
                toAdd.ChatBgId = GetChatBgIdByName(fileName);
                toAdd.IsGeneral = true;

                model.PossibleChatBGs.Add(toAdd);

                model.SaveChanges();
            }
        }

        public static void UpdateChatWallpaper(ChatBackground toSet, int chatId)
        {
            string fileName = Path.GetFileName(toSet.FileName);

            using (var model = new TelegramModel())
            {
                PossibleChatBGs toUpdate = model.PossibleChatBGs.FirstOrDefault(x => x.ChatId == chatId);
                if (toUpdate is null) return;

                //Clear genral state to possible chat ids
                //SetStateToGeneralParamInPossibleBgs(chatId, false);

                toUpdate.ChatBgId = GetChatBgIdByName(fileName);
                toUpdate.IsGeneral = true;

                model.SaveChanges();
            }
        }

        public static bool IsPosChatPgIsExisitByChatId(int chatId)
        {
            using (var model = new TelegramModel())
            {
                return !(model.PossibleChatBGs.FirstOrDefault(x => x.ChatId == chatId) is null);
            }
        }

        public static bool IsRegistrationParamsareExist(string login, string phoneNumber)
        {
            using (var model = new TelegramModel())
            {
                return !(model.User.FirstOrDefault(x => x.Login == login || x.PhoneNumber == phoneNumber) is null);
            }
        }

        //sender and receiver is USER ids
        public static UserContactcs GetContactBySenderReceiverUserIds(int senderId, int receiverId)
        {
            using (var model = new TelegramModel())
            {
                Contacts cont = model.Contacts.FirstOrDefault(x => x.UserId == senderId && x.FriendId == receiverId);
                if (cont is null) return null;
                return GetContactById(cont.Id);
            }
        }

        public static void SetOnlineStatus(int userId, bool isOnline)
        {
            using (var model = new TelegramModel())
            {
                model.User toSet = model.User.FirstOrDefault(x => x.Id == userId);
                if (toSet is null) return;

                toSet.IsOnline = isOnline;
                model.SaveChanges();
            }
        }

        public static bool IsUserOnline(int userId)
        {
            using (var model = new TelegramModel())
            {
                model.User toCheck = model.User.FirstOrDefault(x => x.Id == userId);
                if (toCheck is null) return false;
                return (bool)toCheck.IsOnline;
            }
        }

        public static bool IsContactContactinsInContacts(UserContactcs contact,
            UserContactcs toCheckCotact)
        {
            using (var model = new TelegramModel())
            {
                return model.Contacts.Where(x => x.UserId == contact.ContactUserId &&
                x.FriendId == toCheckCotact.ContactUserId).Any();
            }
        }

        public static void AddUserImage(TelegramLib.MainClasses.User user, string userImageName)
        {
            if (IsUserImageisExist(user, userImageName)) return;
            using (var model = new TelegramModel())
            {
                UserImage img = new UserImage();

                img.UserId = user.Id;
                img.Name = userImageName;
                img.AddDate = DateTime.Now;

                model.UserImage.Add(img);

                model.SaveChanges();
            }
        }

        private static bool IsUserImageisExist(TelegramLib.MainClasses.User user, string userImageNmae)
        {
            using (var model = new TelegramModel())
            {
                return model.UserImage
                    .Where(x => x.Name == userImageNmae && x.UserId == user.Id)
                    .Any();
            }
        }

        public static bool? GetLastSeenStateByUserId(int userId)
        {
            using(var model = new TelegramModel())
            {
                Settings startSetting = model.Settings.FirstOrDefault(x => x.UserId == userId);
                if (startSetting is null) return null;

                PrivacySetting privSet = model.PrivacySetting.FirstOrDefault(x => x.SettingId == startSetting.Id);
                if (privSet is null || privSet.LastSeenSetId is null) return null;

                LastSeenSettings res = model.LastSeenSettings.FirstOrDefault(x => x.Id == privSet.LastSeenSetId);
                if (res is null) return null;

                return res.IsHideReadTime;
            }

        }

    }
}
