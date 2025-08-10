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

namespace TelegramLib.Services
{
    public static class DbService
    {
        public static TelSystem GetTelSystem(string login, string password)
        {
            mainClass.User user = GetUserByLoginAndPassword(login, password);

            if (user is null)
            {
                throw new NullReferenceException();
            }
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

                //Add folder contacts
                AddManyContactInContcatsInFolder(model.Folder.Last().Id,
                    folder.Contacts, false);

                AddManyContactInContcatsInFolder(model.Folder.Last().Id,
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

        public static void UpdateFolder(TelegramLib.MainClasses.FolderObjs.Folder folder, int userId)
        {
            using (var model = new TelegramModel())
            {
                model.Folder toUpdate = model.Folder.Where(x => x.Id == folder.Id).FirstOrDefault();
                if (toUpdate is null) return;

                toUpdate.Name = folder.Name;
                toUpdate.IconId = GetFolderIconIdByName(folder.IconName);

                model.SaveChanges();

                //Update folder contacts

                //Remove all folderContacts 
                RemoveContactsFromFolder(folder.Id);

                //Add folder contacts
                AddManyContactInContcatsInFolder(folder.Id, folder.Contacts, false);
                AddManyContactInContcatsInFolder(folder.Id, folder.ExcludedContacts, true);
            }
        }

        private static void RemoveContactsFromFolder(int folderId)
        {
            using (var model = new TelegramModel())
            {
                List<ContactsInFolder> toRemove = new List<ContactsInFolder>();
                foreach (var temp in model.ContactsInFolder)
                {
                    if (temp.FolderId == folderId) toRemove.Add(temp);
                }

                model.ContactsInFolder.RemoveRange(toRemove);
                model.SaveChanges();
            }
        }

        public static void RemoveFolder(int folderId)
        {
            using (var model = new TelegramModel())
            {
                model.Folder.RemoveRange(model.Folder.Where(x => x.Id == folderId).ToList());
                model.SaveChanges();

                RemoveContactsFromFolder(folderId);
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
                        UserChat toAdd = new UserChat(chat.Id, GetUserContactById((int)chat.ChatterId),
                            GetMessagesByChatId(chat.Id), GetChosenBgByChatId(chat.Id));

                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static ChatBackground GetChosenBgByChatId(int chatId)
        {
            return GetChatBackPossibleChatBGs(chatId).Where(x => x.IsGeneral).FirstOrDefault();
        }

        private static int GetChosenBgIdByName(int chatId)
        {
            //Check for exception        
            using (var model = new TelegramModel())
            {
                return (int)model.PossibleChatBGs.Where(x => x.ChatId == chatId && (bool)x.IsGeneral).First().ChatBgId;
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

        private static List<Message> GetMessagesByChatId(int chatId)
        {
            List<Message> res = new List<Message>();

            using (var model = new TelegramModel())
            {
                foreach (var mes in model.Messages)
                {
                    if (mes.ChatId == chatId)
                    {
                        Message toAdd = new Message();

                        toAdd.Id = mes.Id;
                        toAdd.SenderUserId = (int)mes.SenderId;
                        toAdd.SentTime = (DateTime)mes.SentDate;

                        //Set message Action


                        res.Add(toAdd);
                    }
                }
            }
            return res;
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
                        user.Username, user.Birthday, GetBlockedContactsByUserId(user.Id), GetUserImagesByUserId(user.Id), (DateTime)user.LastOnline));
                }
            }
            return res;
        }

        private static mainClass.User GetUserById(int userId)
        {
            mainClass.User res = new mainClass.User();
            using (var model = new TelegramModel())
            {
                model.User user = model.User.Where(x => x.Id == userId).FirstOrDefault();
                if (user is null) return null;

                res.Id = user.Id;
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

        public static UserContactcs GetUserContactById(int contactId)
        {
            using (var model = new TelegramModel())
            {
                Contacts contact = model.Contacts.Where(x => x.Id == contactId).FirstOrDefault();
                if (contact is null) return null;

                UserContactcs toAdd = new UserContactcs();

                toAdd.Id = contact.Id;
                toAdd.Name = contact.Name;
                toAdd.UserName = contact.User.Name;
                toAdd.BirthDate = contact.User.Birthday;
                toAdd.BIO = contact.User.BIO;
                toAdd.PhoneNumber = contact.User.PhoneNumber;
                toAdd.LastSeen = contact.User.LastOnline;
                toAdd.IsNotificationsIsOn = (bool)contact.IsNotifsIsOn;

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

        private static UserContactcs GetContactById(int contactId)
        {
            UserContactcs res = new UserContactcs();
            using (var model = new TelegramModel())
            {
                Contacts contact = model.Contacts.Where(x => x.Id == contactId).FirstOrDefault();
                if (contact is null) return null;

                mainClass.User user = GetUserById((int)contact.UserId);

                res.Id = contact.Id;
                res.Name = contact.Name;
                res.UserName = user.UserName;
                res.BirthDate = user.BirthDay;
                res.BIO = user.BIO;
                res.PhoneNumber = user.PhoneNumber;
                res.LastSeen = user.LastSeenOnline;
                res.IsNotificationsIsOn = (bool)contact.IsNotifsIsOn;
                res.UserImages = GetUserImagesByUserId(user.Id);
                res.IsBlockedUserBlocked = (bool)contact.IsBlocked;
            }

            return res;
        }

        public static void AddContact(UserContactcs contact, int userId)
        {
            using (var model = new TelegramModel())
            {
                Contacts toAdd = new Contacts();

                toAdd.UserId = userId;
                toAdd.FriendId = contact.ContactUserid;
                toAdd.Name = contact.Name;
                toAdd.LastName = string.Empty;
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
                toUpdate.IsNotifsIsOn = contact.IsNotificationsIsOn;
                toUpdate.IsBlocked = contact.IsBlockedUserBlocked;

                model.SaveChanges();
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
                res.Wallpaper = GetChatWallPaperIdByName(settings.BgName);// GetChatWallpaperById(settings.Bg);
                res.PossibleWallpapers = GetPossibleWallpapersForChatSetting(settingsId); // CHECK IF USERID === SETTINGID
            }
            return res;
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

                temp.ThemeId = GetIdByTheme(settings.Theme);
                temp.UserColorId = settings.ChosenColor.Id;
                temp.AutoNightId = GetAutoNightIdByType(settings.NightMode);
                temp.Font = settings.FontName;
                temp.BgName = settings.Wallpaper.WallpaperName;
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

                        res.Add(toAdd);
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
                ChatBG bg = model.ChatBG.Where(x => x.Id == chatBgId).FirstOrDefault();
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


        public static void AddMessage(UserChat chat, Message message)
        {
            using (var model = new TelegramModel())
            {
                Messages toAdd = new Messages();

                toAdd.ChatId = chat.Id;
                toAdd.SenderId = message.SenderUserId;

                toAdd.Message = message is TextMessage text ? text.Text : null;

                if (message is MediaAction video && video.IsVideo()) toAdd.VideoId = GetVideoIdByName(video.MediaName);
                else toAdd.VideoId = null;


                if (message is MediaAction image && image.IsImage()) toAdd.ImageId = GetChatImageIdByName(image.MediaName);
                else toAdd.ImageId = null;

                toAdd.StickerId = message is MediaAction media && media.IsSticker ? GetStickerIdByName(media.MediaName) : null;

                model.Messages.Add(toAdd);
                model.SaveChanges();

            }
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
            using (var model = new TelegramModel())
            {
                Chat toUpdate = model.Chat.Where(x => x.Id == chat.Id).FirstOrDefault();
                if (toUpdate is null) return;

                toUpdate.BgImageId = GetChatBgIdByName(chat.GetBackground().FileName);
                toUpdate.AutoDeleteId = GetAutoDelIdByType(chat.AutoDel);
                toUpdate.IsMute = chat.Chatter.IsBlockedUserBlocked;

                //Update general BG
                SetChosenBgInPossibleBGs(toUpdate.Id, GetChosenBgIdByName(toUpdate.Id));

                model.SaveChanges();
            }
        }

        private static int GetAutoDelIdByType(Enums.Chat.AutoDeleteType type)
        {
            using (var model = new TelegramModel())
            {
                model.AutoDeleteType delType = model.AutoDeleteType.Where(x => x.Name == type.ToString()).FirstOrDefault();
                if (delType is null) return 1;
                return delType.Id;
            }
        }

        public static int GetChatBgIdByName(string name)
        {
            using (var model = new TelegramModel())
            {
                ChatBG bg = model.ChatBG.Where(x => x.Name == name).FirstOrDefault();
                if (bg is null) return 1;
                return bg.Id;
            }
        }

        public static void ClearChat(int chatId)
        {
            using (var model = new TelegramModel())
            {
                model.Messages.RemoveRange(model.Messages.Where(x => x.ChatId == chatId));
                model.SaveChanges();
            }
        }

        public static void SetBgToChat(int chatId, string bgnName)
        {
            using (var model = new TelegramModel())
            {
                int chatBgId = GetChatBgIdByName(bgnName);
            }
        }

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

    }
}
