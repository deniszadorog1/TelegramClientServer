using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect.Configuration;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.IdentityModel.Metadata;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Threading;
using System.Windows.Forms;
using TelegramLib.Enums.Chat;
using TelegramLib.Enums.Messages;
using TelegramLib.Enums.Settings.ChatSettings;
using TelegramLib.Enums.Settings.Notifs;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.Helpers;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses.Messages;
using TelegramLib.MainClasses.UserParams;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramLib.UserSettings.SettingsTypes.SubSettings;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;
using AdvancedSettings = TelegramLib.Models.AdvancedSettings;
using ChatSettings = TelegramLib.Models.ChatSettings;
using mainClass = TelegramLib.MainClasses;
using model = TelegramLib.Models;

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
            system.SavedMesesChat = GetSavedMessageChat(user.Id);

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

        public static int GetLastFolderIdByOwnerId(int userIndex)
        {
            using (var model = new TelegramModel())
            {
                var list = model.Folder.Where(x => x.OwnerId == userIndex).ToList();
                var last = list.LastOrDefault();
                return last?.Id ?? -1;
            }
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
            List<mainClass.User> contacts, bool isExclude)
        {
            for (int i = 0; i < contacts.Count; i++)
            {
                AddContactInCOntcatsInFolder(folderId, contacts[i], isExclude);
            }
        }

        public static void AddContactInCOntcatsInFolder(int folderId,
            mainClass.User contact, bool isExclude)
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

        public static void UpdateUserNameSurname(int userId, string name, string surname)
        {
            using(var model = new TelegramModel())
            {
                model.User toUpdate = model.User.FirstOrDefault(x => x.Id == userId);
                if (toUpdate is null) return;

                toUpdate.Name = name;
                toUpdate.Surname = surname;

                model.SaveChanges();
            }
        }

        private static void AddExtraContacts(int folderId, List<mainClass.User> contacts, bool isExclude)
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

        private static void RemoveExtraContacts(int folderId, List<mainClass.User> contacts, bool isExclude)
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

        private static List<mainClass.User> GetContactsForFolder(int folderId, bool isExclude)
        {
            List<mainClass.User> res = new List<mainClass.User>();
            using (var model = new TelegramModel())
            {
                foreach (var canFold in model.ContactsInFolder)
                {
                    if (canFold.FolderId == folderId &&
                        canFold.IsExclude == isExclude)
                    {
                        mainClass.User toAdd = GetUserById((int)canFold.ContactId);
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
                        UserContactcs toAdd = GetContactById(cont.Id, userId);
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
                    if (userId == chat.UserId && !chat.IsSavedMessagesChat)
                    {
                        int? correctAutoDelId = null;
                        if (!(chat.AutoDeleteId is null)) correctAutoDelId = (int)chat.AutoDeleteId;

                        UserChat toAdd = new UserChat(chat.Id,
                            GetUserById((int)chat.ChatterId),
                            GetMessagesByChatId(chat.Id, false),
                            GetChosenBgByChatId(chat.Id),
                            GetAutoDelTypeById(correctAutoDelId),
                            GetPinnedMessages(chat.Id),
                            GetMessagesByChatId(chat.Id, true));

                        //Set mask for chatterId
                        SetMaskForChatterId(toAdd.Chatter, userId);

                        toAdd.NotificationStatus = GetNotificationStatusByChatId(chat.Id);

                        toAdd.IsPinned = chat.IsPinned is null ? false : (bool)chat.IsPinned;
                        toAdd.IsMarked = chat.IsRead is null ? false : (bool)chat.IsRead;

                        res.Add(toAdd);
                    }
                }
            }
            return res;
        }

        private static void SetMaskForChatterId(TelegramLib.MainClasses.User chatter, int loggedUserId)
        {
            using(var model = new TelegramModel())
            {
                TelegramLib.MainClasses.UserParams.UserImage mask = 
                    GetContactMaskByContactUserId(loggedUserId, chatter.Id);

                if (mask is null) return;

                chatter.ImageMask = 
                    new TelegramLib.MainClasses.UserParams.UserImage(
                        System.IO.Path.GetFileName(mask.Name), mask.Date);

                chatter.UserImages.Insert(0, chatter.ImageMask);
            }
        }

        private static bool GetNotificationStatusByChatId(int chatId)
        {
            using (var model = new TelegramModel())
            {
                NotificationChats not = model.NotificationChats.FirstOrDefault(x => x.ChatId == chatId);
                return not is null ? false : (bool)not.IsOn;
            }
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
                        toAdd.IsBlurred = chatBG.IsBlurred is null ? false : (bool)chatBG.IsBlurred;
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
                res.IsGeneral = false;
            }

            return res;
        }

        private static List<TelegramLib.MainClasses.Messages.Message> GetPinnedMessages(int chatId)
        {
            List<TelegramLib.MainClasses.Messages.Message> res = new List<mainClass.Messages.Message>();
            using (var model = new TelegramModel())
            {
                List<Messages> messes = model.Messages.Where(x => x.ChatId == chatId && (bool)x.IsPinned).ToList();

                foreach (var mes in messes)
                {
                    res.Add(GetMessageByMessages(mes));
                }
            }
            return res;
        }

        public static List<TelegramLib.MainClasses.Messages.Message> GetMessagesByChatId(int chatId,
            bool isGetSchedMessages = false)
        {
            List<TelegramLib.MainClasses.Messages.Message> res =
                new List<TelegramLib.MainClasses.Messages.Message>();

            using (var model = new TelegramModel())
            {
                foreach (var mes in model.Messages)
                {
                    if (mes.ChatId == chatId && 
                        mes.IsInSchedule == isGetSchedMessages)
                    {
                        res.Add(GetMessageByMessages(mes));
                    }
                }
            }
            return res;
        }

        public static TelegramLib.MainClasses.Messages.Message GetMessageById(int id)
        {
            using (var model = new TelegramModel())
            {
                Messages mes = model.Messages.FirstOrDefault(x => x.Id == id);

                TelegramLib.MainClasses.Messages.Message res =
                    mes is null ? null : GetMessageByMessages(mes);

                return res;
            }
        }

        private static TelegramLib.MainClasses.Messages.Message GetMessageByMessages(Messages mes)
        {
            TelegramLib.MainClasses.Messages.Message toAdd;
            if (!(mes.MessageRefference is null) ||
                !(mes.ChangedAutoDelId is null) ||
                !(mes.StatDate is null)) toAdd = new mainClass.Messages.StaticMessage();

            else if (mes.Message is null) toAdd = new MediaAction();
            else if (!(mes.ShareContactMessage is null)) toAdd = new TelegramLib.MainClasses.Messages.ShareContactMessage();
            else toAdd = new TextMessage();

            toAdd.Id = mes.Id;
            toAdd.SenderUserId = mes.SenderId is null ? -1 : (int)mes.SenderId;

            toAdd.RepliedQuote = mes.MessageQuote;
            toAdd.SentTime = mes.SentDate is null ? DateTime.Now : (DateTime)mes.SentDate;
            toAdd.IsRead = mes.IsRead;

            if (toAdd is MediaAction band) band.BandId = mes.BandId is null ? -1 : (int)mes.BandId;

            if (toAdd is TextMessage)
            {
                ((TextMessage)toAdd).Text = mes.Message;
                ((TextMessage)toAdd).IsEdited = mes.IsEdited;
            }
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

            if (toAdd is TelegramLib.MainClasses.Messages.ShareContactMessage share &&
                !(mes.ShareContactId is null))
            {
                model.ShareContactMessage message = GetShareModelById((int)mes.ShareContactId);

                share.SharedName = message.Name;
                share.SharedUser = GetUserById((int)message.UserId);
            }

            toAdd.IsPinned = mes.IsPinned is null ? false : (bool)mes.IsPinned;
            toAdd.ForwardedFromId = mes.ForwardedFrom;

            if (toAdd is TextMessage text)
            {
                text.RepliedMessageId = mes.ReplyId;
            }

            if (toAdd is StaticMessage statMessage)
            {
                statMessage.MessageReferenceId = mes.MessageRefference is null ? -1 : mes.MessageRefference;

                if (mes.ChangedAutoDelId is null) statMessage.DelType = null;
                else statMessage.DelType = GetAutoDelTypeById(mes.ChangedAutoDelId);

                statMessage.Date = mes.StatDate;
            }
            return toAdd;
        }

        private static model.ShareContactMessage GetShareModelById(int id)
        {
            using (var model = new TelegramModel())
            {
                return model.ShareContactMessage.FirstOrDefault(x => x.Id == id);
            }
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
                    UserColor color = model.UserColor.FirstOrDefault(x => x.UserId == user.Id);

                    ColorHelper colorHelper = new ColorHelper(user.Id);

                    if (!(color is null))
                    {
                        colorHelper.R = (byte)color.R;
                        colorHelper.G = (byte)color.G;
                        colorHelper.B = (byte)color.B;
                    }

                    res.Add(new mainClass.User(user.Id, user.Login, user.Password,
                        user.Name, user.Surname, user.BIO,
                        colorHelper, user.PhoneNumber,
                        user.Birthday, GetBlockedContactsByUserId(user.Id),
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
                //res.UserName = user.Username;
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
            return users.Where(x => x.Login == login &&
            x.Password == password).FirstOrDefault();
        }

        //Correct (When add new fields in user table)
        public static void AddUser(string name, string surname, string phoneNumber,
            DateTime? birthdate, string login, string password)
        {
            using (var model = new TelegramModel())
            {
                model.User user = new model.User();

                user.Name = name;
                user.IsOnline = false;
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
                //toUpdate.Username = user.UserName;

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
                    toAdd.Login = tempContact.User.Name;
                    toAdd.BirthDate = tempContact.User.Birthday;
                    toAdd.BIO = tempContact.User.BIO;
                    toAdd.PhoneNumber = tempContact.User.PhoneNumber;
                    toAdd.LastSeen = tempContact.User.LastOnline;

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

        //IS NEED TO ADD AUTO DEL IN HERE
        public static UserContactcs GetUserContactById(int contactId)
        {
            using (var model = new TelegramModel())
            {
                Contacts contact = model.Contacts.Where(x => x.Id == contactId).FirstOrDefault();
                if (contact is null) return null;

                //Is realy friend id should be here
                Models.User user = model.User.FirstOrDefault(x => x.Id == contact.FriendId);
                if (user is null) return null;

                UserContactcs toAdd = new UserContactcs();

                toAdd.Id = contact.Id;
                toAdd.Name = contact.Name;
                toAdd.ContactUserId = (int)contact.FriendId;
                toAdd.IsOnline = IsContactOnlineByUserId((int)contact.FriendId);
                toAdd.Login = user.Login;// contact.User.Name;
                toAdd.BirthDate = user.Birthday;//  contact.User.Birthday;
                toAdd.BIO = user.BIO;// contact.User.BIO;
                toAdd.PhoneNumber = user.PhoneNumber;// contact.User.PhoneNumber;
                toAdd.LastSeen = contact.User.LastOnline;
                toAdd.Surname = contact.LastName;

                toAdd.UserImages = GetUserImagesByUserId((int)contact.FriendId);
                return toAdd;
            }
        }

        private static List<TelegramLib.MainClasses.User> GetBlockedContactsByUserId(int userId)
        {
            List<mainClass.User> res = new List<mainClass.User>();
            using (var model = new TelegramModel())
            {
                foreach (var blockedItem in model.BlockedContacts)
                {
                    if (blockedItem.UserId == userId)
                    {
                        res.Add(GetUserById((int)blockedItem.BlockedContactId));
                    }
                }
            }
            return res;
        }

        public static UserContactcs GetContactById(int contactId, int loggedUserId)
        {
            UserContactcs res = new UserContactcs();
            using (var model = new TelegramModel())
            {
                Contacts contact = model.Contacts.Where(x => x.Id == contactId).FirstOrDefault();
                if (contact is null) return null;

                mainClass.User user = GetUserById((int)contact.FriendId);//Or userId

                res.Id = contact.Id;
                res.ContactUserId = (int)contact.FriendId;
                res.IsOnline = IsContactOnlineByUserId((int)contact.FriendId);
                res.Name = contact.Name;
                res.Surname = contact.LastName;
                res.Login = user.Login;
                res.BirthDate = user.BirthDay;
                res.BIO = user.BIO;
                res.PhoneNumber = user.PhoneNumber;
                res.LastSeen = user.LastSeenOnline;
                res.MaskImage = GetContactMaskByContactUserId(loggedUserId, (int)contact.FriendId);

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

                if (model.Contacts.Any(x => x.UserId == userId && x.FriendId == contact.ContactUserId)) return;

                toAdd.Name = contact.Name;
                toAdd.LastName = contact.Surname;

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

                model.SaveChanges();
            }
        }

        public static UserContactcs GetLastAddedContactByUser(int userId)
        {
            using (var model = new TelegramModel())
            {
                List<Contacts> toGet = model.Contacts.Where(x => x.UserId == userId).ToList();
                if (toGet is null) return null;

                return GetContactById(toGet.Last().Id, userId);
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

                res.IsTabsOnTheLeft = setting.IsFolderTabsIsLeft is null ?
                     true : (bool)setting.IsFolderTabsIsLeft;

                res.NotSettings = GetNotifSettingsBySettingsId(setting.Id);
                (res.NotSettings.SideType, res.NotSettings.AmountOfMonMessages) =
                    GetMonitorParams(userId);

                res.ChatsSettings = GetChatSettingsBySettingsId(setting.Id, userId);
                res.AdvSettings = GetAdvansedSettingsById(setting.Id);
                res.PrivacySettings = GetPrivacySettings(setting.Id);

                res.SoundNotifSettings = GetUserSoundByUserId(userId);

                res.LanguageSettings = GetLanguage(userId);
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

                        //Last seen online
                        UpdateLastSeenSetting(settings.Id, settings.LastSeenPrivacy);

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
                res.SelfDeleteTime = GetAwayForTimeById((int)settings.AwayForTypeId);

                res.PhonePrivacy = GetPhoneNumberSettingsById((int)settings.PhoneNumberSetId, settingId);
                res.LastSeenPrivacy = GetLastSeenSubById((int)settings.LastSeenSetId, settingId);
                res.ProfPhotoPrivacy = GetProfPhotoSub((int)settings.ProfPhotoSetId, settingId);
                res.ForwardMesPrivacy = GetForwardMesSubById((int)settings.ForwardMesSetId, settingId);
                res.MessagesPrivacy = GetMessagesPrivById((int)settings.MessagesSetId);
                res.DateBirthPrivacy = GetBirthDateById((int)settings.DateOfBirthSetId, settingId);
                res.BioPrivacy = GetBioById((int)settings.BioSetId, settingId);
                res.PassCode = GetLocalPasscodeSettings(settingId);
            }
            return res;
        }

        private static PasscodeSettings GetLocalPasscodeSettings(int settingId)
        {
            PasscodeSettings res = new PasscodeSettings();

            using (var model = new TelegramModel())
            {
                PassCode code = model.PassCode.FirstOrDefault(x => x.Id == settingId);
                if (code is null) return null;

                res.IsWinUnLock = (bool)code.IsWinUnlock;
                res.MinutesTimer = (int)code.Minutes;
                res.Id = (int)code.Id;
                res.PassCode = code.Passcode1;
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
                res.ShareWithExps = GetChosenShareContacts(true, settingId, SubSettingType.Profile);
                res.NeverShareExps = GetChosenShareContacts(false, settingId, SubSettingType.Profile);
            }
            return res;
        }

        private static string GetPublicPhotoPathName(int userImageId)
        {
            using (var model = new TelegramModel())
            {
                model.UserImage img = model.UserImage.Where(x => x.Id == userImageId).FirstOrDefault();
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

        public static UserSettings.SettingsTypes.ChatSettings GetChatSettingsBySettingsId(int settingsId, int userId)
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
                                                                                          // res.Themes = GetThemesByUserId(userId);
            }
            return res;
        }

        private static List<mainClass.ChatFitures.Theme> GetThemesByUserId(int userId)
        {
            List<mainClass.ChatFitures.Theme> res = new List<mainClass.ChatFitures.Theme>();
            using (var model = new TelegramModel())
            {
                List<model.UserTheme> themes = model.UserTheme.Where(x => x.UserId == userId).ToList();

                foreach (var theme in themes)
                {
                    mainClass.ChatFitures.Theme toAdd = new mainClass.ChatFitures.Theme();

                    toAdd.Id = theme.Id;
                    toAdd.Type = GetThemeTypeById((int)theme.TypeId);
                    toAdd.Color = GetThemeColor(theme.Id);
                    res.Add(toAdd);
                }
            }
            return res;
        }

        private static ColorHelper GetThemeColor(int themeId)
        {
            ColorHelper res = new ColorHelper();
            using (var model = new TelegramModel())
            {
                ThemeColor color = model.ThemeColor.FirstOrDefault(x => x.Id == themeId);
                if (color is null) return res;

                res.R = (byte)color.R;
                res.G = (byte)color.G;
                res.B = (byte)color.B;
            }
            return res;
        }

        private static ThemeType GetThemeTypeById(int typeId)
        {
            using (var model = new TelegramModel())
            {
                foreach (var type in model.Theme)
                {
                    if (type.Id == typeId)
                    {
                        return (ThemeType)type.Id;
                    }
                }
            }
            return ThemeType.Night;
        }

        private static int GetThemeIdByType(ThemeType type)
        {
            using (var model = new TelegramModel())
            {
                model.Theme res = model.Theme.FirstOrDefault(x => x.Name == type.ToString());
                return res is null ? 4 : res.Id;
            }
        }

        public static void UpdateTheme(mainClass.ChatFitures.Theme theme)
        {
            using (var model = new TelegramModel())
            {
                /*                int? id = GetThemeIdByType(theme.Type);
                                if (id is null) return;

                                UserTheme toUpdate = model.UserTheme.FirstOrDefault(x => x.Id == theme.Id && x.TypeId == (int)id);
                                if (toUpdate is null) return;*/

                ThemeColor color = model.ThemeColor.FirstOrDefault(x => x.Id == theme.Id);
                if (color is null) return;

                color.R = theme.Color.R;
                color.G = theme.Color.G;
                color.B = theme.Color.B;

                model.SaveChanges();
            }
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

            //Add not monitor
            AddNotifMonitor(userId);

            //Add Sounds
            AddUserSound(userId);

            //Add language
            AddLanguageForUser(userId);

            //Add passcode
            AddPassCode(userId);
        }

        private static void AddPassCode(int userId)
        {
            using (var model = new TelegramModel())
            {
                PassCode toAdd = new PassCode();

                toAdd.Passcode1 = string.Empty;
                toAdd.Minutes = -1;
                toAdd.IsWinUnlock = false;

                model.PassCode.Add(toAdd);
                model.SaveChanges();
            }
        }

        public static void AddBaseSettings(int userId)
        {
            using (var model = new TelegramModel())
            {
                Settings settings = new Settings();

                settings.UserId = userId;
                settings.IsFolderTabsIsLeft = true;

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
                settings.BgName = 1; ///
                settings.IsSentWithEnter = true;

                model.ChatSettings.Add(settings);

                //Add Themes
                AddThemes(newSettingId);

                model.SaveChanges();
            }
        }

        private static void AddThemes(int userId)
        {
            using (var model = new TelegramModel())
            {
                for (int i = 1; i <= (int)ThemeType.Night; i++)
                {
                    //Add new color
                    AddThemeColor();
                    //Get temp theme id
                    int themeId = GetIdByTheme((ThemeType)i);

                    UserTheme toAdd = new UserTheme();
                    toAdd.UserId = userId;
                    toAdd.TypeId = themeId;
                    toAdd.ColorId = model.UserColor.FirstOrDefault().Id;

                    model.UserTheme.Add(toAdd);
                }
                model.SaveChanges();
            }
        }

        private static void AddThemeColor()
        {
            using (var model = new TelegramModel())
            {
                ThemeColor color = new ThemeColor();
                color.R = 128;
                color.G = 128;
                color.B = 256;

                model.ThemeColor.Add(color);

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

        public static void AddUserColor(int r, int g, int b, int userId)
        {
            using (var model = new TelegramModel())
            {
                UserColor toAdd = new UserColor();

                toAdd.R = r;
                toAdd.G = g;
                toAdd.B = b;
                toAdd.UserId = userId;

                model.UserColor.Add(toAdd);

                model.SaveChanges();
            }
        }

        public static bool IsUserColorExist(int userId)
        {
            using (var model = new TelegramModel())
            {
                return model.UserColor.Any(x => x.UserId == userId);
            }
        }

        public static int GetUserColorIdByUserId(int userId)
        {
            using (var model = new TelegramModel())
            {
                UserColor res = model.UserColor.FirstOrDefault(x => x.UserId == userId);

                return res is null ? -1 : res.Id;
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
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.PhoneNumber), sub.NeverShareExps, false);

                model.SaveChanges();
            }
        }

        private static void UpdateChosenPrivContacts(int settingId, int settingTypeId,
            List<mainClass.User> contacts, bool isShare)
        {
            //Remove all rows that confirm conditions
            List<ChosenPrivacyContacts> toRemove = new List<ChosenPrivacyContacts>();// GetChosenPrivContaactWithCondition(settingId, settingTypeId, isShare);

            using (var model = new TelegramModel())
            {

                foreach (var contact in model.ChosenPrivacyContacts)
                {
                    if (contact.SettingTypeId == settingTypeId &&
                       contact.SttingId == settingId &&
                       contact.IsShare == isShare)
                    {
                        toRemove.Add(contact);
                    }
                }

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
            List<mainClass.User> contacts, int settingTypeId, bool isShare, int settingId)
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
                LastSeenSettings settings = model.LastSeenSettings.FirstOrDefault(x => x.Id == settingId);
                if (settings is null) return;

                settings.WhoSeeId = GetWhoSeeIdByShareWithType(sub.ShareType.ToString());
                settings.IsHideReadTime = sub.IsHideReadAction;

                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.LastSeen), sub.ShareWithExps, true);
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.LastSeen), sub.NeverShareExps, false);

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
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Profile), sub.NeverShareExps, false);

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
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.ForwardMessage), sub.NeverShareExps, false);

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
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.DateOfBirth), sub.NeverShareExps, false);

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
                UpdateChosenPrivContacts(settingId, GetSubSettingTypeByEnum(SubSettingType.Bio), sub.NeverShareExps, false);

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
                model.Theme theme = model.Theme.Where(x => x.Id == themeId).FirstOrDefault();
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

        private static List<mainClass.User> GetChosenShareContacts(bool isShare, int settingId, SubSettingType type)
        {
            List<mainClass.User> res = new List<mainClass.User>();
            int subSettingId = GetSubSettingTypeByEnum(type);

            using (var model = new TelegramModel())
            {
                foreach (var contact in model.ChosenPrivacyContacts)
                {
                    if (contact.IsShare == isShare &&
                        contact.SettingTypeId == subSettingId &&
                        contact.SttingId == settingId)
                    {
                        res.Add(GetUserById((int)contact.ContactId));
                    }
                }
            }

            return res;
        }

        public static int GetSubSettingTypeByEnum(SubSettingType type)
        {
            using (var model = new TelegramModel())
            {
                PrivacySettingType res = model.PrivacySettingType.FirstOrDefault(
                    x => x.Name == type.ToString());
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
                toAdd.IsRead = false;
                toAdd.IsInSchedule = message.IsSchedule;

                toAdd.MessageQuote = message.RepliedQuote;
                toAdd.BandId = message is MediaAction bandMedia ? bandMedia.BandId : -1;

                toAdd.Message = message is TextMessage text ? text.Text : null;

                if (message is MediaAction video && video.IsVideo()) toAdd.VideoId = GetVideoIdByName(video.MediaName);
                else toAdd.VideoId = null;

                if (message is MediaAction image && image.IsImage()) toAdd.ImageId = GetChatImageIdByName(image.MediaName);
                else toAdd.ImageId = null;

                if (message is MediaAction gif && gif.IsGif()) toAdd.GifId = GetChatGifIdByName(gif.MediaName);
                else toAdd.GifId = null;

                toAdd.StickerId = message is MediaAction media && media.IsSticker ? GetStickerIdByName(media.MediaName) : null;

                toAdd.IsPinned = message.IsPinned;
                toAdd.ForwardedFrom = message.ForwardedFromId;

                if (message is TextMessage addText)
                {
                    toAdd.ReplyId = addText.RepliedMessageId;
                    toAdd.IsEdited = addText.IsEdited;
                }

                model.Messages.Add(toAdd);
                model.SaveChanges();
            }
            return true;
        }

        public static void EditSchedMessage(int mesId,
                TextMessage textMes,
                MediaAction mediaMes)
        {
            using (var model = new TelegramModel())
            {
                Messages toEdit = model.Messages.FirstOrDefault(x => x.Id == mesId);
                if (toEdit is null) return;

                if(!(textMes is null))
                {
                    toEdit.Message = textMes.Text;
                }
                else if(!(mediaMes is null))
                {
                    if (mediaMes is MediaAction video && video.IsVideo()) toEdit.VideoId = GetVideoIdByName(video.MediaName);
                    if (mediaMes is MediaAction image && image.IsImage()) toEdit.ImageId = GetChatImageIdByName(image.MediaName);
                    if (mediaMes is MediaAction gif && gif.IsGif()) toEdit.GifId = GetChatGifIdByName(gif.MediaName);
                }

                model.SaveChanges();
            }
        }

        public static void EditMessage(int chatId,
            TelegramLib.MainClasses.Messages.TextMessage textMes,
            TelegramLib.MainClasses.Messages.MediaAction mediaMes)
        {
            TelegramLib.MainClasses.Messages.Message mes = null;
            if (textMes is null) mes = mediaMes;
            else mes = textMes;

            if (mes is null) return;

            TelegramLib.MainClasses.Messages.Message pair = GetPairOfMessageBySentTime(textMes.Id);

            using (var model = new TelegramModel())
            {
                //Get message
                Messages toEdit = model.Messages.FirstOrDefault(
                    x =>/* x.ChatId == chatId &&*/ x.Id == mes.Id);

                Messages pairDb = pair is null ? null : model.Messages.FirstOrDefault(x => x.Id == pair.Id);

                if (toEdit is null) return;

                //Update params
                if (mes is MediaAction video && video.IsVideo()) toEdit.VideoId = GetVideoIdByName(video.MediaName);
                if (mes is MediaAction image && image.IsImage()) toEdit.ImageId = GetChatImageIdByName(image.MediaName);
                if (mes is MediaAction gif && gif.IsGif()) toEdit.GifId = GetChatGifIdByName(gif.MediaName);

                if (mes is TextMessage text)
                {
                    if (!(pairDb is null))
                    {
                        pairDb.Message = text.Text;
                        pairDb.IsEdited = true;
                    }

                    toEdit.Message = text.Text;
                    toEdit.IsEdited = true;
                }
                //Save
                model.SaveChanges();
            }
        }

        public static void EditSavedMessage(TelegramLib.MainClasses.Messages.TextMessage textMes,
            TelegramLib.MainClasses.Messages.MediaAction mediaMes)
        {
            TelegramLib.MainClasses.Messages.Message mes = null;
            if (textMes is null) mes = mediaMes;
            else mes = textMes;

            using (var model = new TelegramModel())
            {
                //Get message
                Messages toEdit = model.Messages.FirstOrDefault(x => x.Id == mes.Id);

                if (toEdit is null) return;

                //Update params
                if (mes is MediaAction video && video.IsVideo()) toEdit.VideoId = GetVideoIdByName(video.MediaName);
                if (mes is MediaAction image && image.IsImage()) toEdit.ImageId = GetChatImageIdByName(image.MediaName);
                if (mes is MediaAction gif && gif.IsGif()) toEdit.GifId = GetChatGifIdByName(gif.MediaName);

                if (mes is TextMessage text)
                {
                    toEdit.Message = text.Text;
                    toEdit.IsEdited = true;
                }
                //Save
                model.SaveChanges();
            }
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
                res.IsPinned = chosen.IsPinned is null ? false : (bool)chosen.IsPinned;
                res.IsSchedule = /*chosen.IsInSchedule is null ? false :*/ chosen.IsInSchedule;

                res.RepliedQuote = chosen.MessageQuote;

                if (res is TextMessage text)
                {
                    text.Text = chosen.Message;
                    text.RepliedMessageId = chosen.ReplyId;
                    text.ForwardedFromId = chosen.ForwardedFrom;
                }
                else if (res is MediaAction media)
                {
                    media.IsSticker = chosen.StickerId is null ? false : true;
                    media.MediaName = GetMediaNameByMessageId(chosen.Id);
                    media.BandId = chosen.BandId is null ? -1 : (int)chosen.BandId;
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
                ChatImage img = model.ChatImage.FirstOrDefault(x => x.Name == imgName);
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
            //Update chat messages

            UpdateMessages(chat);
            using (var model = new TelegramModel())
            {
                Chat toUpdate = model.Chat.FirstOrDefault(x => x.Id == chat.Id);
                if (toUpdate is null) return;

                if (!(chat.ChatBg is null)) toUpdate.BgImageId = GetChatBgIdByName(chat.GetBackground().FileName);
                toUpdate.AutoDeleteId = GetAutoDelIdByType(chat.AutoDel);
                //toUpdate.IsMute = chat.Chatter.IsBlockedUserBlocked;

                //Update general BG

                toUpdate.IsPinned = chat.IsPinned;
                toUpdate.IsRead = chat.IsMarked;

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
                        !chat.Messages.Any(x => x.Id == mes.Id) && 
                        !mes.IsInSchedule)
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
                        ChangeForRepPointers(mes.Id, model);
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
            for (int i = 0; i <= (int)Enums.Chat.AutoDeleteType.OneYear; i++)
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
                foreach(var mes in model.Messages)
                {
                    if (mes.ChatId == chatId)
                    {
                        ChangeForRepPointers(mes.Id, model);
                    }
                }

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
            //if (IsContactIsBlocked(userId, contactId)) return;
            using (var model = new TelegramModel())
            {
                BlockedContacts toBlock = new BlockedContacts();

                toBlock.UserId = userId;
                toBlock.BlockedContactId = contactId;

                model.BlockedContacts.Add(toBlock);
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
                BlockedContacts toRemove = model.BlockedContacts.FirstOrDefault(
                    x => x.UserId == userId && x.BlockedContactId == contactId);
                if (toRemove is null) return;

                model.BlockedContacts.Remove(toRemove);
                model.SaveChanges();
            }
        }

        private static bool IsContactIsBlocked(int userId, int contactId)
        {
            using (var model = new TelegramModel())
            {
                return model.BlockedContacts.Any(
                    x => x.UserId == userId && x.BlockedContactId == contactId);
            }
        }

        public static void RemoveContact(UserContactcs contact, TelegramLib.MainClasses.User loggedUser)
        {
            //Get pair contact id => remove pair of chats with this ids => remove all messagges with this ids 

            //contact - first chatter
            UserContactcs pairContact = GetContactBySenderReceiverUserIds(contact.ContactUserId, loggedUser.Id);
            UserContactcs other = GetContactBySenderReceiverUserIds(loggedUser.Id, contact.ContactUserId);


            //Remove chat
            //RemoveChatsByChatterId(contact.Id);
            //RemoveChatsByChatterId(pairContact.Id);


            //Remove ContactsFolder
            //RemoveContactsInFolderByContactId(contact.Id);

            //Remove contact from blockedContacts
            //RemoveBlockedByContactId(contact.Id);

            //Remove contact from Chosen contacts
            //RemoveFromChosenPrivacyContactsByContactId(contact.Id);

            //Remove folder(if they are empty)
            //RemoveEmptyFolders(); 

            //Remove contact
            using (var model = new TelegramModel())
            {
                model.Contacts.RemoveRange(model.Contacts.Where(
                    x => x.Id == contact.Id /*|| 
                    x.Id == pairContact.Id || 
                    x.Id == other.Id*/));

                model.SaveChanges();
            }
        }

        private static void RemovePosBgsByChatId(int chatId)
        {
            using (var model = new TelegramModel())
            {
                List<PossibleChatBGs> toRemove = model.PossibleChatBGs
                    .Where(x => x.ChatId == chatId)
                    .ToList();

                foreach (var rem in toRemove)
                {
                    model.PossibleChatBGs.Remove(rem);
                }
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

                    DeleteNotificationChat(toRemove[i].Id);
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

                foreach(var mes in toRemove)
                {
                    ChangeForRepPointers(mes.Id, model);
                }

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

        public static void DeleteChatById(int chatId)
        {
            DeleteNotificationChat(chatId);

            using (var model = new TelegramModel())
            {
                Chat toRemove = model.Chat.FirstOrDefault(x => x.Id == chatId);
                if (toRemove is null) return;

                RemovePosBgsByChatId(toRemove.Id);
                RemoveMessagesByChatId(toRemove.Id);

                model.Chat.Remove(toRemove);
                model.SaveChanges();
            }
        }

        private static void RemoveMessagesBySenderId(int chatId)
        {
            using (var model = new TelegramModel())
            {
                model.Messages.RemoveRange(model.Messages.Where(x => x.ChatId == chatId));
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
                if (model.Chat.Any(x => x.UserId == userId && x.ChatterId == chatterContactId)) return;

                Chat toAdd = new Chat();

                toAdd.UserId = userId;
                toAdd.ChatterId = chatterContactId;
                toAdd.BgImageId = 1;
                toAdd.AutoDeleteId = null;
                toAdd.IsMute = false;

                model.Chat.Add(toAdd);
                model.SaveChanges();
            }

            UserChat chat = GetChatByUserAndContactIds(userId, chatterContactId);
            if (chat is null) return;
            AddNotificationChat(chat.Id);
        }

        private static void AddNotificationChat(int chatId)
        {
            using (var model = new TelegramModel())
            {
                NotificationChats toAdd = new NotificationChats();
                toAdd.ChatId = chatId;
                toAdd.IsOn = false;

                model.NotificationChats.Add(toAdd);
                model.SaveChanges();
            }
        }
        public static void ChangeNotificationState(int chatId, bool state)
        {
            using (var model = new TelegramModel())
            {
                NotificationChats toCorrect = model.NotificationChats.FirstOrDefault(x => x.ChatId == chatId);
                if (toCorrect is null) return;

                toCorrect.IsOn = state;
                model.SaveChanges();
            }
        }

        public static bool IsUserIsBlocked(int userId, int contactId)
        {
            using (var model = new TelegramModel())
            {
                return !(model.BlockedContacts
                            .FirstOrDefault(x => x.UserId == userId &&
                                        x.BlockedContactId == contactId) is null);
            }
        }

        private static void DeleteNotificationChat(int chatId)
        {
            using (var model = new TelegramModel())
            {
                NotificationChats toDelete = model.NotificationChats.FirstOrDefault(x => x.ChatId == chatId);
                if (toDelete is null) return;

                model.NotificationChats.Remove(toDelete);
                model.SaveChanges();
            }
        }


        public static UserChat GetChatByUserAndContactIds(int userId, int contactId)
        {
            using (var model = new TelegramModel())
            {
                /*                Contacts contact = model.Contacts.FirstOrDefault(x => x.Id == contactId);
                                if (contact is null) return null;*/

                Chat chat = model.Chat.FirstOrDefault(x => x.UserId == userId && x.ChatterId == contactId /*contact.FriendId*/);
                if (chat is null) return null;
                return new UserChat(chat.Id, 
                    GetUserById((int)chat.ChatterId),
                    GetMessagesByChatId(chat.Id, false), 
                    GetChosenBgByChatId(chat.Id),
                    GetAutoDelTypeById(chat.AutoDeleteId),
                    new List<mainClass.Messages.Message>(),
                    GetMessagesByChatId(chat.Id, true));
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
                toAdd.IsBlurred = toAdd.IsBlurred;

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
                return model.User.Any(x => x.Login == login || x.PhoneNumber == phoneNumber);
            }
        }

        //sender and receiver is USER ids
        public static UserContactcs GetContactBySenderReceiverUserIds(int senderId, int receiverId)
        {
            using (var model = new TelegramModel())
            {
                Contacts cont = model.Contacts.FirstOrDefault(x => x.UserId == senderId && x.FriendId == receiverId);
                if (cont is null) return null;
                return GetContactById(cont.Id, senderId);
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

        public static bool IsContactContactsInContacts(UserContactcs contact,
            UserContactcs toCheckContact)
        {
            if (contact is null || toCheckContact is null) return false;

            using (var model = new TelegramModel())
            {
                return model.Contacts.Where(x => x.UserId == contact.ContactUserId &&
                x.FriendId == toCheckContact.ContactUserId).Any();
            }
        }

        public static bool IsLoginExist(string login)
        {
            using(var model = new TelegramModel())
            {
                return model.User.Any(x => x.Login == login);
            }
        }

        public static void UpdateUserLogin(int id, string newLogin)
        {
            using(var model = new TelegramModel())
            {
                model.User toUpdate = model.User.FirstOrDefault(x => x.Id == id);
                if (toUpdate is null) return;

                toUpdate.Login = newLogin;

                model.SaveChanges();
            }
        }

        public static void RemoveUserImage(
            TelegramLib.MainClasses.UserParams.UserImage toRemove,
            int userId)
        {
            if (toRemove is null) return;
            const int maxDateDiffer = 100;
            using (var model = new TelegramModel())
            {
/*                var from = toRemove.Date.AddMilliseconds(-maxDateDiffer);
                var to = toRemove.Date.AddMilliseconds(maxDateDiffer);
*/
                model.UserImage img =
                    model.UserImage.FirstOrDefault(x =>
                        x.UserId == userId &&
                        x.Name == toRemove.Name/* &&
                        x.AddDate >= from &&
                        x.AddDate <= to*/);

                if (img is null) return;
                model.UserImage.Remove(img);

                model.SaveChanges();
            }
        }

        public static void AddUserImage(TelegramLib.MainClasses.User user, string userImageName)
        {
            if (IsUserImageIsExist(user, userImageName)) return;
            using (var model = new TelegramModel())
            {
                model.UserImage img = new model.UserImage();

                img.UserId = user.Id;
                img.Name = userImageName;
                img.AddDate = DateTime.Now;

                model.UserImage.Add(img);

                model.SaveChanges();
            }
        }



        private static bool IsUserImageIsExist(TelegramLib.MainClasses.User user, string userImageNmae)
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
            using (var model = new TelegramModel())
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

        public static void AddWallpaper(string imgName)
        {
            using (var model = new TelegramModel())
            {
                if (model.ChatBG.Where(x => x.Name == imgName).Any()) return;

                ChatBG toAdd = new ChatBG();
                toAdd.Name = imgName;

                model.ChatBG.Add(toAdd);

                model.SaveChanges();
            }
        }

        public static void UpdateTabsPosType(bool isOnTheLeft, int settingId)
        {
            using (var model = new TelegramModel())
            {
                Settings toUpdate = model.Settings.FirstOrDefault(x => x.Id == settingId);
                if (toUpdate is null) return;

                toUpdate.IsFolderTabsIsLeft = isOnTheLeft;
                model.SaveChanges();
            }
        }

        //Monitor notification stuff
        public static void AddNotifMonitor(int userId)
        {
            using (var model = new TelegramModel())
            {
                MonitorNotifs notif = new MonitorNotifs();

                notif.Type = 3;
                notif.MessagesAmount = 5;
                notif.UserId = userId;

                model.MonitorNotifs.Add(notif);
                model.SaveChanges();
            }
        }

        public static void UpdateWindowNotifcation(NotifMessageSide side,
            int mesAmount, int userId)
        {
            using (var model = new TelegramModel())
            {
                MonitorNotifs notif = model.MonitorNotifs.FirstOrDefault(x => x.UserId == userId);
                if (notif is null) return;

                notif.Type = GetSideIdByType(side);
                notif.MessagesAmount = mesAmount;

                model.SaveChanges();
            }
        }

        private static int GetSideIdByType(NotifMessageSide side)
        {
            using (var model = new TelegramModel())
            {
                MonitorSidesType type =
                    model.MonitorSidesType.FirstOrDefault(x => x.Name == side.ToString());

                return type is null ? -1 : type.Id;
            }
        }

        private static NotifMessageSide GetMonitorSideById(int id)
        {
            using (var model = new TelegramModel())
            {
                MonitorSidesType type =
                      model.MonitorSidesType.FirstOrDefault(x => x.Id == id);
                if (type is null) return NotifMessageSide.BottomRight;

                for (int i = (int)NotifMessageSide.TopLeft;
                    i <= (int)NotifMessageSide.BottomLeft; i++)
                {
                    if (((NotifMessageSide)i).ToString() == type.Name)
                    {
                        return (NotifMessageSide)i;
                    }
                }
            }
            return NotifMessageSide.BottomRight;
        }

        private static (NotifMessageSide, int) GetMonitorParams(int userId)
        {
            using (var model = new TelegramModel())
            {
                MonitorNotifs notif = model.MonitorNotifs.FirstOrDefault(x => x.UserId == userId);

                return notif is null ? (NotifMessageSide.BottomRight, 5) :
                    (GetMonitorSideById((int)notif.Type), (int)notif.MessagesAmount);
            }
        }

        //Sound stuff

        public static void AddSound(string name)
        {
            using (var model = new TelegramModel())
            {
                if (model.Sounds.Any(x => x.Name == name)) return;
                Sounds toAdd = new Sounds();
                toAdd.Name = name;

                model.Sounds.Add(toAdd);
                model.SaveChanges();
            }
        }

        public static List<string> GetAllSounds()
        {
            List<string> res = new List<string>();
            using (var model = new TelegramModel())
            {
                foreach (var sound in model.Sounds)
                {
                    res.Add(sound.Name);
                }
            }
            return res;
        }

        public static void UpdateSounds(int userId, string soundName,
            int vol, bool isDefault)
        {
            using (var model = new TelegramModel())
            {
                UserSounds sound = model.UserSounds.FirstOrDefault(x => x.UserId == userId);
                if (sound is null) return;

                sound.Volume = vol;
                sound.IsDefaultSound = isDefault;
                if (!isDefault) sound.ChosenSoundId = GetSoundIdByName(soundName);

                model.SaveChanges();
            }
        }

        private static int GetSoundIdByName(string name)
        {
            using (var model = new TelegramModel())
            {
                Sounds sound = model.Sounds.FirstOrDefault(x => x.Name == name);

                return sound is null ? -1 : sound.Id;
            }
        }

        private static void AddUserSound(int userId)
        {
            using (var model = new TelegramModel())
            {
                UserSounds sound = new UserSounds();

                sound.Volume = 100;
                sound.IsDefaultSound = true;
                sound.ChosenSoundId = 2;
                sound.UserId = userId;

                model.UserSounds.Add(sound);
                model.SaveChanges();
            }
        }

        private static string GetSoundInText(int soundId)
        {
            using (var model = new TelegramModel())
            {
                Sounds sound = model.Sounds.FirstOrDefault(x => x.Id == soundId);
                return sound is null ? "Default" : sound.Name;
            }
        }

        private static SoundSettings GetUserSoundByUserId(int userId)
        {
            SoundSettings res = new SoundSettings();

            using (var model = new TelegramModel())
            {
                UserSounds sound = model.UserSounds.FirstOrDefault(x => x.Id == userId);
                if (sound is null) return res;

                res.Volume = (int)sound.Volume;
                res.Id = sound.Id;

                res.MesSounds = GetAllSounds();

                res.ChosenSound = !(sound.IsDefaultSound is null) && (bool)sound.IsDefaultSound ? "Default.mp3" :
                    GetSoundInText((int)sound.ChosenSoundId);

                return res;
            }
        }

        public static void UpdateFolderPosition(int userId, bool isLeft)
        {
            using (var model = new TelegramModel())
            {
                Settings toUpdate = model.Settings.FirstOrDefault(x => x.UserId == userId);
                if (toUpdate is null) return;
                toUpdate.IsFolderTabsIsLeft = isLeft;

                model.SaveChanges();
            }
        }

        public static bool IsChatIsExist(int userId, int contactId)
        {
            using (var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.UserId == userId && x.ChatterId == contactId);
                bool res = !(chat is null);
                return res;
            }
        }

        public static SysLanguage GetLanguage(int userId)
        {
            using (var model = new TelegramModel())
            {
                Languages lang = model.Languages.FirstOrDefault(x => x.UserId == userId);
                if (lang is null) return null;

                SysLanguage res = new SysLanguage();

                res.Id = lang.Id;
                res.Type = GetLangTypeById((int)lang.TypeId);
                return res;
            }
        }

        private static Enums.Settings.Language.LanguageType GetLangTypeById(int id)
        {
            using (var model = new TelegramModel())
            {
                model.LanguageType type = model.LanguageType.FirstOrDefault(x => x.Id == id);

                return type.Id == 1 ? Enums.Settings.Language.LanguageType.English :
                     Enums.Settings.Language.LanguageType.Russian;
            }
        }

        public static void UpdateLanguage(int userId, Enums.Settings.Language.LanguageType type)
        {
            using (var model = new TelegramModel())
            {
                Languages lang = model.Languages.FirstOrDefault(x => x.UserId == userId);
                if (lang is null) return;

                int langTypeId = GetLangTypeIdByEnum(type);
                lang.TypeId = langTypeId == -1 ? 1 : langTypeId;

                model.SaveChanges();
            }
        }

        public static void AddLanguageForUser(int userId)
        {
            using (var model = new TelegramModel())
            {
                Languages toAdd = new Languages();
                toAdd.TypeId = 1;
                toAdd.UserId = userId;

                model.Languages.Add(toAdd);
                model.SaveChanges();
            }
        }

        private static int GetLangTypeIdByEnum(TelegramLib.Enums.Settings.Language.LanguageType type)
        {
            using (var model = new TelegramModel())
            {
                model.LanguageType res = model.LanguageType.FirstOrDefault(x => x.Name == type.ToString());

                return res is null ? -1 : res.Id;
            }
        }

        public static void UpdatePassCode(PasscodeSettings settings)
        {
            using (var model = new TelegramModel())
            {
                PassCode code = model.PassCode.FirstOrDefault(x => x.Id == settings.Id);
                if (code is null) return;

                code.Passcode1 = settings.PassCode;
                code.IsWinUnlock = settings.IsWinUnLock;
                code.Minutes = settings.MinutesTimer;

                model.SaveChanges();
            }
        }

        public static void DeleteUserFromFolder(int folderId, int userId)
        {
            using (var model = new TelegramModel())
            {
                //Get folder
                ContactsInFolder toRemove = model.ContactsInFolder
                    .FirstOrDefault(x => x.FolderId == folderId && x.ContactId == userId);

                if (toRemove is null) return;
                model.ContactsInFolder.Remove(toRemove);

                model.SaveChanges();
            }
        }

        public static void AddShareMessage(int userId,
            string contactName, int chatId, int senderId, string message)
        {
            AddShareContactMessage(contactName, userId);

            using (var model = new TelegramModel())
            {
                Messages toAdd = new Messages();
                toAdd.ChatId = chatId;
                toAdd.SenderId = senderId;
                toAdd.Message = message;
                toAdd.ImageId = null;
                toAdd.StickerId = null;
                toAdd.GifId = null;
                toAdd.VideoId = null;
                toAdd.SentDate = DateTime.Now;
                toAdd.ShareContactId = GetLastShareMessageId();
                toAdd.IsRead = false;
                toAdd.MessageQuote = string.Empty;

                model.Messages.Add(toAdd);
                model.SaveChanges();
            }
        }



        public static int GetLastSharedMessageId(int chatId)
        {
            using (var model = new TelegramModel())
            {
                List<Messages> mes = model.Messages.Where(x => x.ChatId == chatId).ToList();
                Messages res = mes.LastOrDefault(x => !(x.ShareContactId is null));
                return res is null ? -1 : res.Id;
            }
        }

        private static int? GetLastShareMessageId()
        {
            using (var model = new TelegramModel())
            {
                model.ShareContactMessage res =
                    model.ShareContactMessage.ToList().LastOrDefault();

                if (res is null) return null;
                return res.Id;
            }
        }

        private static void AddShareContactMessage(string name, int userId)
        {
            using (var model = new TelegramModel())
            {
                model.ShareContactMessage toAdd = new model.ShareContactMessage();

                toAdd.UserId = userId;
                toAdd.Name = name;

                model.ShareContactMessage.Add(toAdd);

                model.SaveChanges();
            }
        }

        public static void SetReadMessageAction(int messageId)
        {
            using (var model = new TelegramModel())
            {
                Messages toRead = model.Messages.FirstOrDefault(x => x.Id == messageId);
                if (toRead is null) return;
                toRead.IsRead = true;

                model.SaveChanges();
            }
        }

        public static void SetReadStatusByMessIdBySendTime(int messageId)
        {
            using (var model = new TelegramModel())
            {
                //Get message 
                Messages mes = model.Messages.FirstOrDefault(x => x.Id == messageId);
                if (mes is null) return;

                //Get message with same sendTime (but differ id)
                var sameTimeMessage = model.Messages
                .AsEnumerable()
                .FirstOrDefault(x =>
                    x.Id != mes.Id &&
                    x.SentDate.HasValue && mes.SentDate.HasValue &&
                    Math.Abs((x.SentDate.Value - mes.SentDate.Value).TotalMilliseconds) < 100);


                /*                var sameTimeMessage = model.Messages
                                    .FirstOrDefault(x =>
                                        x.Id != mes.Id &&
                                        x.SentDate.HasValue && mes.SentDate.HasValue &&
                                        x.SentDate.Value.Year == mes.SentDate.Value.Year &&
                                        x.SentDate.Value.Month == mes.SentDate.Value.Month &&
                                        x.SentDate.Value.Day == mes.SentDate.Value.Day &&
                                        x.SentDate.Value.Hour == mes.SentDate.Value.Hour &&
                                        x.SentDate.Value.Minute == mes.SentDate.Value.Minute &&
                                        x.SentDate.Value.Second == mes.SentDate.Value.Second &&
                                        x.SentDate.Value.Millisecond == mes.SentDate.Value.Millisecond);*/

                //1 mil sec differ
                /*  sameTimeMessage = model.Messages
                     .FirstOrDefault(x => x.Id != mes.Id &&
                          Math.Abs(((TimeSpan)(x.SentDate - mes.SentDate)).TotalMilliseconds) < 1);*/
                if (sameTimeMessage is null) return;

                //compare them(read status)
                if (sameTimeMessage.IsRead) mes.IsRead = true;
                model.SaveChanges();
            }
        }

        public static TelegramLib.MainClasses.Messages.Message GetPairOfMessageBySentTime(int mesId)
        {
            //Add chat chacker if need
            using (var model = new TelegramModel())
            {
                Messages toCompare = model.Messages.FirstOrDefault(x => x.Id == mesId);
                if (toCompare is null) return null;

                Messages res = model.Messages
                    .AsEnumerable()
                    .FirstOrDefault(x =>
                        x.ChatId != toCompare.ChatId &&
                        x.Id != mesId &&
                        x.SentDate.HasValue && toCompare.SentDate.HasValue &&
                        Math.Abs((x.SentDate.Value - toCompare.SentDate.Value).TotalMilliseconds)
                        < 10);

                return res is null ? null : GetMessageByMessages(res);
            }
        }

        public static int? GetCorrectIdBySentDate(DateTime sentTime)
        {
            using (var model = new TelegramModel())
            {
                Messages res = model.Messages
                    .AsEnumerable()
                    .FirstOrDefault(x =>
                        x.SentDate.HasValue &&
                        Math.Abs((x.SentDate.Value - sentTime).TotalMilliseconds) < 100);

                if (res is null) return null;

                TelegramLib.MainClasses.Messages.Message mes = GetMessageByMessages(res);
                if (mes is null) return null;
                return mes.Id;
            }
        }

        public static bool GetMessageReadStatusById(int id)
        {
            using (var model = new TelegramModel())
            {
                Messages mes = model.Messages.FirstOrDefault(x => x.Id == id);
                return mes is null ? false : mes.IsRead;
            }
        }

        public static void SetReadStatusForChat(int chatId, int loggedUserId)
        {
            using (var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.Id == chatId);
                if (chat is null) return;


            }
        }

        public static void SetPinStatus(int messageId, bool isPin, bool isSaveMessageChat)
        {
            using (var model = new TelegramModel())
            {
                if (!isSaveMessageChat)
                {
                    Messages mes = model.Messages.FirstOrDefault(x => x.Id == messageId);
                    if (mes is null) return;
                    mes.IsPinned = isPin;
                }
                else
                {
                    Messages mes = model.Messages.FirstOrDefault(x => x.Id == messageId);
                    if (mes is null) return;
                    mes.IsPinned = isPin;
                }
                model.SaveChanges();
            }
        }

        public static void RemoveMessageById(int id)
        {
            using (var model = new TelegramModel())
            {
                RemoveMessageById(id, model);

                model.SaveChanges();
            }
        }

        public static void RemoveManyMessages(List<int> mesIds, bool isBoth)
        {
            using(var model = new TelegramModel())
            {
                for(int i = 0; i < mesIds.Count; i++)
                {
                    //Get pair of message
                    if (isBoth)
                    {
                       Messages pairToRemove =  GetPairOfMessageById(mesIds[i], model);
                        if (!(pairToRemove is null)) RemoveMessageById(pairToRemove.Id, model);
                    }
                    RemoveMessageById(mesIds[i], model);
                }

                //Check with static message

                model.SaveChanges();
            }
        }

        private static Messages GetPairOfMessageById(int mesId, TelegramModel model)
        {
            Messages toCompare = model.Messages.FirstOrDefault(x => x.Id == mesId);
            if (toCompare is null) return null;

            Messages res = model.Messages
                .AsEnumerable()
                .FirstOrDefault(x =>
                    x.ChatId != toCompare.ChatId &&
                    x.Id != mesId &&
                    x.SentDate.HasValue && toCompare.SentDate.HasValue &&
                    Math.Abs((x.SentDate.Value - toCompare.SentDate.Value).TotalMilliseconds)
                    < 10);

            return res;
        }

        private static void RemoveMessageById(int id, TelegramModel model)
        {
            Messages mes =
            model.Messages.FirstOrDefault(x => x.Id == id);
            if (mes is null) return;
            model.Messages.Remove(mes);

            ChangeForRepPointers(id, model);
        }

        private static void ChangeForRepPointers(int id, TelegramModel model)
        {
            model.Messages
                .Where(x => x.ReplyId == id)
                .ToList()
                .ForEach(x => x.ReplyId = -1);

            model.Messages
                .Where(x => x.ForwardedFrom == id)
                .ToList()
                .ForEach(x => x.ForwardedFrom = -1);

/*            model.SavedMessages
                .Where(x => x.ReplyId == id)
                .ToList()
                .ForEach(x => x.ReplyId = -1);

            model.SavedMessages
                .Where(x => x.ForwardedFrom == id)
                .ToList()
                .ForEach(x => x.ForwardedFrom = -1);*/
        }

        public static void AddStatMessage(int chatId,
            TelegramLib.MainClasses.Messages.StaticMessage statMes)
        {
            using (var model = new TelegramModel())
            {
                Messages toAdd = new Messages();

                toAdd.SenderId = statMes.SenderUserId;
                toAdd.SentDate = statMes.SentTime;
                toAdd.MessageRefference = statMes.MessageReferenceId;
                toAdd.StatDate = statMes.Date;
                toAdd.MessageQuote = string.Empty;

                if (statMes.DelType is null) toAdd.ChangedAutoDelId = null;
                else toAdd.ChangedAutoDelId = ((int)((Enums.Chat.AutoDeleteType)statMes.DelType) + 1);


                toAdd.ChatId = chatId;

                model.Messages.Add(toAdd);
                model.SaveChanges();
            }
        }

        public static bool IsChatterIdIsContact(int userId, int friendUserId)
        {
            using (var model = new TelegramModel())
            {
               return model.Contacts.Any(x => x.UserId == userId && x.FriendId == friendUserId);
            }
        }
        public static int? GetLastStatMesIdByChatId(int chatId)
        {
            using (var model = new TelegramModel())
            {
                Messages mes = model.Messages
                    .Where(x => x.ChatId == chatId &&
                    (x.MessageRefference != null ||
                    x.ChangedAutoDelId != null || x.StatDate != null))
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

                if (mes is null) return null;
                return mes.Id;
            }
        }

        public static int? GetStatMessageIdByItsReference(int chatId, int refId)
        {
            using (var model = new TelegramModel())
            {
                Messages res = model.Messages.FirstOrDefault(x => x.ChatId == chatId && x.MessageRefference == refId);

                if (res is null) return null;
                return res.Id;
            }
        }

        public static bool? IsInChatterChatIsExistDateMessage(
            int loggedId, int chatterId, DateTime date)
        {
            using (var model = new TelegramModel())
            {
                //Get chat
                Chat chat = model.Chat.FirstOrDefault(
                    x => x.UserId == chatterId && x.ChatterId == loggedId);
                if (chat is null) return null;

                //Is date message exist

                bool isExist = model.Messages.Any(x => x.ChatId == chat.Id &&
                !(x.StatDate == null) &&
                x.StatDate.Value.Year == date.Year &&
                x.StatDate.Value.Month == date.Month &&
                x.StatDate.Value.Day == date.Day);

                return isExist;
            }
        }

        //Saved messages 
        //Get savedUserMessageChat
        //Get messages
        //Add message
        //Delete message
        //

        public static void ClearSavedChatMessages(int chatId)
        {
            using (var model = new TelegramModel())
            {
                List<Messages> toRemove =
                    model.Messages.Where(x => x.ChatId == chatId && x.IsSavedMessage).ToList();

                foreach (var mes in toRemove)
                {
                    model.Messages.Remove(mes);
                }

                model.SaveChanges();
            }
        }

        public static int? GetLastStatDateIdInSavedChat(int chatId)
        {
            using (var model = new TelegramModel())
            {
                Messages mes = model.Messages
                    .Where(x => x.ChatId == chatId &&
                                x.StatDate.HasValue)
                    .OrderByDescending(x => x.StatDate)
                    .FirstOrDefault();

                return mes is null ? (int?)null : mes.Id;
            }
        }

        public static int? GetIdOfLastSavedMessage(int chatId)
        {
            using (var model = new TelegramModel())
            {
                Messages mes = model.Messages.Where(x => x.ChatId == chatId)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();


                return mes is null ? (int?)null : mes.Id;
            }
        }

        public static bool IsDateStatContainsInSavedMessageChat(int chatId, DateTime date)
        {
            using (var model = new TelegramModel())
            {
                return model.Messages.Any(x => x.ChatId == chatId &&
                x.StatDate.HasValue &&
                x.StatDate.Value.Day == date.Day);
            }
        }

        public static void AddSavedMessagesChat(int userId)
        {
            using (var model = new TelegramModel())
            {
                model.Chat toAdd = new model.Chat();

                toAdd.UserId = userId;
                toAdd.BgImageId = 1;
                toAdd.IsRead = false;
                toAdd.IsPinned = false;
                toAdd.IsSavedMessagesChat = true;

                model.Chat.Add(toAdd);
                model.SaveChanges();
            }
        }

        

        public static void AddSavedMessage(int savedMessageChatId,
            TelegramLib.MainClasses.Messages.Message toAdd)
        {
            using (var model = new TelegramModel())
            {
                Messages toAddObj = new Messages();

                toAddObj.ChatId = savedMessageChatId;
                toAddObj.IsSavedMessage = true;
                toAddObj.MessageQuote = toAdd.RepliedQuote;
                toAddObj.BandId = -1;

                if (toAdd is TextMessage text)
                {
                    toAddObj.Message = text.Text;
                    toAddObj.ReplyId = text.RepliedMessageId;
                }

                if (toAdd is MediaAction media)
                {
                    toAddObj.ImageId = media.IsImage() ? GetChatImageIdByName(media.MediaName) : (int?)null;
                    toAddObj.StickerId = media.IsSticker ? GetStickerIdByName(media.MediaName) : (int?)null;
                    toAddObj.GifId = media.IsGif() ? GetChatGifIdByName(media.MediaName) : (int?)null;
                    toAddObj.VideoId = media.IsVideo() ? GetVideoIdByName(media.MediaName) : (int?)null;
                    toAddObj.BandId = media.BandId;
                }

                toAddObj.SentDate = toAdd.SentTime;
                //toAddObj. = DateTime.Now;

                if (toAdd is TelegramLib.MainClasses.Messages.ShareContactMessage share)
                {
                    toAddObj.ShareContactId = share.SharedUser.Id;
                }

                toAddObj.IsRead = toAdd.IsRead;
                toAddObj.IsPinned = toAdd.IsPinned;

                toAddObj.ForwardedFrom = toAdd.ForwardedFromId;

                if (toAdd is StaticMessage statMes)
                {
                    toAddObj.MessageRefference = statMes.MessageReferenceId;
                    toAddObj.StatDate = statMes.Date;
                }

                model.Messages.Add(toAddObj);
                model.SaveChanges();
            }
        }

        public static mainClass.SavedMessagesChat GetSavedMessageChat(int userId)
        {
            mainClass.SavedMessagesChat res = new mainClass.SavedMessagesChat();
            res.PinnedMessages = GetPinnedSavedChatMessages(res.Id);
            using (var model = new TelegramModel())
            {
                Models.Chat chat =
                    model.Chat.FirstOrDefault(x => x.UserId == userId && x.IsSavedMessagesChat);
                if (chat is null) return null;

                List<mainClass.Messages.Message> messages = GetSavedMessages(chat.Id);

                res.Id = chat.Id;
                res.ChatBg = GetChatBgById((int)chat.BgImageId);
                res.Messages = messages;

                res.IsPinned = chat.IsPinned is null ? false : (bool)chat.IsPinned;
                res.IsMarked = chat.IsRead is null ? false : (bool)chat.IsRead;

                res.ScheduleMessages = GetSchedMessagesForSavedMesChat(userId);
            }
            return res;
        }

        private static List<TelegramLib.MainClasses.Messages.Message> GetSchedMessagesForSavedMesChat(int userId)
        {
            using(var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.UserId == userId && x.IsSavedMessagesChat);
                if (chat is null) return null;

                return GetMessagesByChatId(chat.Id, true);
            }
        }

        private static List<mainClass.Messages.Message> GetPinnedSavedChatMessages(int savedChatId)
        {
            List<TelegramLib.MainClasses.Messages.Message> res = new List<mainClass.Messages.Message>();
            using (var model = new TelegramModel())
            {
                List<Messages> messes = model.Messages.Where(
                    x => x.ChatId == savedChatId && (bool)x.IsPinned && x.IsSavedMessage).ToList();

                foreach (var mes in messes)
                {
                    res.Add(GetSavedMessageByMessages(mes));
                }
            }
            return res;
        }

        public static void ClearSaveChatById(int chatId)
        {
            using(var model = new TelegramModel())
            {
                foreach(var mes in model.Messages)
                {
                    if(mes.ChatId == chatId && mes.IsSavedMessage)
                    {
                        ChangeForRepPointers(mes.Id, model);
                    }
                }

                model.Messages.RemoveRange(
                    model.Messages.Where(x => x.ChatId == chatId && x.IsSavedMessage));

                model.SaveChanges();
            }
        }

        public static List<TelegramLib.MainClasses.Messages.Message> GetSavedMessages(int savedMessageChatId)
        {
            List<TelegramLib.MainClasses.Messages.Message> res = new List<mainClass.Messages.Message>();

            using (var model = new TelegramModel())
            {
                List<Messages> messages = model.Messages.Where(x => x.ChatId == savedMessageChatId && x.IsSavedMessage).ToList();

                for (int i = 0; i < messages.Count; i++)
                {
                    res.Add(GetSavedMessageByMessages(messages[i]));
                }
            }

            return res;
        }

        public static TelegramLib.MainClasses.Messages.Message GetLastSavedMessage(int chatId)
        {
            using (var model = new TelegramModel())
            {
                var message = model.Messages
                    .Where(x => x.ChatId == chatId && x.IsSavedMessage)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                return GetSavedMessageByMessages(message);
            }
        }

        public static void RemoveSavedMessage(int savedChatId, List<int> messageIds)
        {
            using (var model = new TelegramModel())
            {
                foreach(var messageId in messageIds){

                    Messages toRemove = model.Messages.
                        FirstOrDefault(x => x.ChatId == savedChatId && x.Id == messageId);

                    ChangeForRepPointers(messageId, model);

                    if (toRemove is null) continue;
                    model.Messages.Remove(toRemove);
                }
               
                model.SaveChanges();
            }
        }

        private static TelegramLib.MainClasses.Messages.Message GetSavedMessageByMessages(Messages mes)
        {
            TelegramLib.MainClasses.Messages.Message toAdd;
            if (!(mes.MessageRefference is null) ||
                !(mes.StatDate is null)) toAdd = new mainClass.Messages.StaticMessage();

            else if (mes.Message is null) toAdd = new MediaAction();
            else if (!(mes.ShareContactMessage is null)) toAdd =
                    new TelegramLib.MainClasses.Messages.ShareContactMessage();
            else toAdd = new TextMessage();

            toAdd.Id = mes.Id;
            toAdd.RepliedQuote = mes.MessageQuote;
            toAdd.SenderUserId = 1;

            toAdd.SentTime = mes.SentDate is null ? DateTime.Now : (DateTime)mes.SentDate;
            toAdd.IsRead = mes.IsRead;

            if (toAdd is MediaAction media) media.BandId = mes.BandId is null ? -1 : (int)mes.BandId;

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

            if (toAdd is TelegramLib.MainClasses.Messages.ShareContactMessage share &&
                !(mes.ShareContactId is null))
            {
                model.ShareContactMessage message = GetShareModelById((int)mes.ShareContactId);

                share.SharedName = message.Name;
                share.SharedUser = GetUserById((int)message.UserId);
            }

            toAdd.IsPinned = mes.IsPinned is null ? false : (bool)mes.IsPinned;
            toAdd.ForwardedFromId = mes.ForwardedFrom;

            if (toAdd is TextMessage text)
            {
                text.RepliedMessageId = mes.ReplyId;
                text.IsEdited = mes.IsEdited;
            }

            if (toAdd is StaticMessage statMessage)
            {
                statMessage.MessageReferenceId =
                    mes.MessageRefference is null ? -1 : mes.MessageRefference;

                statMessage.Date = mes.StatDate;
            }
            return toAdd;
        }

        public static void SetMaskImage(UserContactcs contact, int loggedUserId)
        {
            using (var model = new TelegramModel())
            {
                ContactImageMask mask = model.ContactImageMask.
                    FirstOrDefault(x => x.UserId == loggedUserId &&
                    x.FriendId == contact.ContactUserId);

                if (!(mask is null))
                {
                    if (contact.MaskImage is null) model.ContactImageMask.Remove(mask);
                    else mask.ImageName = contact.MaskImage.Name;
                }
                else
                {
                    ContactImageMask toAdd = new ContactImageMask();
                    toAdd.UserId = loggedUserId;
                    toAdd.FriendId = contact.ContactUserId;
                    toAdd.ImageName = contact.MaskImage.Name;

                    model.ContactImageMask.Add(toAdd);
                }
                model.SaveChanges();
            }
        }

        public static TelegramLib.MainClasses.UserParams.UserImage GetContactMaskByContactUserId(int loggedUserId, int contactUserId)
        {
            using (var model = new TelegramModel())
            {
                ContactImageMask mask = model.ContactImageMask.
                    FirstOrDefault(x => x.UserId == loggedUserId && x.FriendId == contactUserId);

                if (mask is null) return null;

                return new TelegramLib.MainClasses.UserParams.UserImage(mask.ImageName, DateTime.Now);

            }
        }

        public static TelegramLib.MainClasses.Messages.Message AddAndGetSchedMessage(UserChat chat, 
            TelegramLib.MainClasses.Messages.Message mes)
        {
            mes.IsSchedule = true;
            AddMessage(chat, mes);

            return GetLastChatMessage(chat.Id);
        }

        public static void UpdateDateInSchedMessageById(int mesId, DateTime newDate)
        {
            using (var model = new TelegramModel())
            {
                Messages toUpdate = model.Messages.FirstOrDefault(x => x.Id == mesId);

                if (toUpdate is null) return;
                if (toUpdate.IsInSchedule) toUpdate.SentDate = newDate;

                model.SaveChanges();
            }
        }

        public static (HashSet<int>, HashSet<int>) GetUserIdsSentSchedMessages()
        {
            HashSet<int> userIds = new HashSet<int>();
            HashSet<int> chatIds = new HashSet<int>();
            using(var model = new TelegramModel())
            {
                IQueryable<Messages> toSendQuery = model.Messages.Where(
                    x => x.IsInSchedule &&
                    x.SentDate.HasValue &&
                    x.SentDate.Value < DateTime.Now);

                List<Messages> toSend = toSendQuery.ToList();

                for(int i = 0; i < toSend.Count; i++)
                {
                    bool isInSavedChat = IsChatIsSavedChat((int)toSend[i].ChatId);
                    AddChatUserAndChatterInSet(userIds, (int)toSend[i].ChatId);
                    toSend[i].IsInSchedule = false;

                    if (isInSavedChat) toSend[i].IsSavedMessage = true;

                    if (!isInSavedChat)
                    {
                        var id = toSend[i].Id;
                        var copy = model.Messages
                            .AsNoTracking()
                            .First(x => x.Id == id);

                        copy.IsInSchedule = false;
                        copy.Id = default;

                        int pairChatId = GetPairChatIdByChatId((int)toSend[i].ChatId);

                        if (pairChatId != -1)
                        {
                            copy.ChatId = pairChatId;

                            chatIds.Add(pairChatId);
                            model.Messages.Add(copy);
                        }
                    }
                    chatIds.Add((int)toSend[i].ChatId);
                }

                model.SaveChanges();
            }
            return (userIds, chatIds);
        }

        private static bool IsChatIsSavedChat(int chatId)
        {
            using(var model = new TelegramModel())
            {
                return model.Chat.Any(x => x.Id == chatId && x.IsSavedMessagesChat);
            }
        }

        private static void AddChatUserAndChatterInSet(HashSet<int> set, int chatId)
        {
            using(var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.Id == chatId);
                if (chat is null) return;

                int chatterId = chat.ChatterId is null ? -1 : (int)chat.ChatterId;

                set.Add((int)chat.UserId);
                if(chatterId != -1)set.Add(chatterId);
            }
        }

        private static int GetPairChatIdByChatId(int chatId)
        {
            using(var model = new TelegramModel())
            {
                Chat chat = model.Chat.FirstOrDefault(x => x.Id == chatId);
                
                if (chat is null) return -1;

                Chat pair = model.Chat.FirstOrDefault(
                    x => x.UserId == chat.ChatterId && 
                    x.ChatterId == chat.UserId);

                return pair is null ? -1 : pair.Id;
            }
        }

        public static int GetLastMessageBandId()
        {
            using(var model = new TelegramModel())
            {
                var maxMessage = model.Messages
                        .OrderByDescending(m => m.BandId)
                        .FirstOrDefault();

                if (!(maxMessage is null && !(maxMessage.BandId is null))) return (int)maxMessage.BandId;
                return -1;
            }
        }
    }
}
