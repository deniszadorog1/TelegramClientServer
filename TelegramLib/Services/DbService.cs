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
            system.Chats = ;
            system.Contacts = ;
            system.Folders = ;

            return system;
        }

        public static List<mainClass.User> GetAllUsers()
        {
            List<mainClass.User> res = new List<mainClass.User>();
            using (var model = new TelegramModel())
            {
                foreach (var user in model.User)
                {
                    // ADD BIO, COLOR, USERNAME, BLOCKEDUsers, USERIMAGES
                    res.Add(new mainClass.User(user.Id, user.Login, user.Password, user.Name, user.Surname, "ADD BIO",
                        new Helpers.ColorHelper(), user.PhoneNumber, "ADD USERNAME", user.Birthday, GetBlockedUserIdsByUserId() user.BlockedUsers, GetUserImagesByUserId(user.Id)))
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
                user.LastOnline = DateTime.Now;

                model.User.Add(user);

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

                model.SaveChanges();
            }
        }

        //Contacts

        public static void GetUsersContacts(int userId)
        {
            using (var model = new TelegramModel())
            {
                List<Contacts> contacts = model.Contacts.Where(x => x.UserId == userId).ToList();

                List<mainClass.UserContactcs> resContacts = new List<mainClass.UserContactcs>();
                foreach (var tempContact in contacts)
                {
                    mainClass.UserContactcs toAdd = new mainClass.UserContactcs();

                    toAdd.Id = tempContact.Id;
                    toAdd.Name = tempContact.Name;
                    toAdd.UserName = tempContact.User.Name;
                    toAdd.BirthDate = tempContact.User.Birthday;
                    toAdd.BIO = tempContact.User.BIO;
                    toAdd.PhoneNumber = tempContact.User.PhoneNumber;
                    toAdd.LastSeen = tempContact.User.LastOnline;
                    toAdd.IsNotificationsIsOn = false;

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




        private static List<UserContactcs> GetBlockedContactsByUserId(int userId)
        {
            List<UserContactcs> res = new List<UserContactcs>();
            using (var model = new TelegramModel())
            {
                foreach (var blockedItem in model.BlockedUsers)
                {
                    if (blockedItem.User.Id == userId)
                    {
                        res.Add(GetContactById((int)blockedItem.BlockedUserId));   //BlockedCONTATCTid                    
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
                res.IsBlockedUserBlocked = contact.Is
            }

            return res;
        }
        //Contacts



        // SETTINGS OPTIONS

        public static MainSettings GetSettingsByUserId(int userId)
        {
            MainSettings res = new MainSettings();

            using (var model = new TelegramModel())
            {
                Settings setting = model.Settings.Where(x => x.UserId == userId).FirstOrDefault();
                if (setting is null) return null;

                res.Id = setting.Id;
                res.NotSettings = GetNotifSettingsBySettingsId(setting.Id);
                res.ChatsSettings = GetChatSettingsBySettingsId(setting.Id);
                res.AdvSettings = GetAdvansedSettingsById(setting.Id);
                res.PrivacySettings = GetPrivacySettings(setting.Id);
            }

            return res;
        }

        private static PrivAndSecSettings GetPrivacySettings(int settingId)
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

            using(var model = new TelegramModel())
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

            using(var model = new TelegramModel())
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

            using(var model = new TelegramModel())
            {
                ProfilePhotoSettings photo = model.ProfilePhotoSettings.Where(x => x.Id == id).FirstOrDefault();
                if (photo is null) return null;

                res.PublicPhotoPath = photo.PublicPhotoId;

                res.ShareType = GetShareWithById((int)photo.WhoSeeId);
                res.ShareWithExps = GetChosenShareContacts(true, settingId, SubSettingType.PhoneNumber);
                res.NeverShareExps = GetChosenShareContacts(false, settingId, SubSettingType.PhoneNumber);
            }
            return res;
        }

        private static LastSeenSub GetLastSeenSubById(int id, int settingsId)
        {
            LastSeenSub res = new LastSeenSub();

            using(var model = new TelegramModel())
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


        private static UserSettings.SettingsTypes.AdvancedSettings GetAdvansedSettingsById(int settingsId)
        {
            UserSettings.SettingsTypes.AdvancedSettings res = new UserSettings.SettingsTypes.AdvancedSettings();

            using (var model = new TelegramModel())
            {
                AdvancedSettings settings = model.AdvancedSettings.Where(x => x.SettingId == settingsId).FirstOrDefault();
                if (settings is null) return null;

                res.Id = settings.Id;
                res.IsAskDownloadPath = settings.Dow;
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

        private static UserSettings.SettingsTypes.ChatSettings GetChatSettingsBySettingsId(int settingsId)
        {
            UserSettings.SettingsTypes.ChatSettings res = new UserSettings.SettingsTypes.ChatSettings();

            using (var model = new TelegramModel())
            {
                ChatSettings settings = model.ChatSettings.Where(x => x.SettingId == settingsId).FirstOrDefault();
                if (settings is null) return null;

                res.Id = settings.Id;
                res.Theme = GetThemeById((int)settings.ThemeId);
                //res.ChosenColor = new ChosenColor;
                res.NightMode = GetNightModeById((int)settings.AutoNightId);
                res.FontName = settings.Font;
                res.IsSendWithEnter = (bool)settings.IsSentWithEnter;
                res.Wallpaper = GetChatWallpaperById(settings.Bg);
                res.PossibleWallpapers =
            }
            return res;
        }

        private static NotificationSettings GetNotifSettingsBySettingsId(int settingId)
        {
            NotificationSettings res = new NotificationSettings();

            using (var model = new TelegramModel())
            {
                NotificatioonsAndSound settings = model.NotificatioonsAndSound.Where(
                    x => x.SettingId == settingId).FirstOrDefault();
                if (settings is null) return null;

                res.Id = settings.Id;
                res.IsDesktopNotifications = (bool)settings.DesktopNotification;
                res.IsFlashTaskBar = (bool)settings.FlashTaskBar;
                res.IsAllowSounds = (bool)settings.AllowSound;
                res.IsPrivateChats = (bool)settings.PrivateChat;
                res.IsPinnedMessages = (bool)settings.PinnedMessage;
            }

            return res;
        }


        public static void AddSettings(int userId)
        {
            int newSettingId;
            using (var model = new TelegramModel())
            {
                Settings settings = new Settings();

                settings.UserId = userId;
                model.Settings.Add(settings);

                model.SaveChanges();

                newSettingId = model.Settings.Last().Id;
            }

            //Privacy settings
            AddPrivacySettings(newSettingId);

            //Chat settings
            AddChatSettings(newSettingId);

            //Advanced settings
            AddAdvancedSettings(newSettingId);

            //Notifications and sounds settings
            AddNotificationSettings(newSettingId);
        }

        //Chat settings
        private static void AddChatSettings(int newSettingId)
        {
            using (var model = new TelegramModel())
            {
                ChatSettings settings = new ChatSettings();

                settings.SettingId = newSettingId;
                settings.ThemeId = 1;
                settings.Color = "#22B14C";
                settings.AutoNightId = 1;
                settings.Font = "Time New Roman";
                settings.BgName = "WHAT IS IT?"; ///
                settings.IsSentWithEnter = true;

                model.SaveChanges();
            }
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

                model.SaveChanges();
            }
        }
        public static void UpdateNotificationSoundsSettings(int settingsId,
            UserSettings.SettingsTypes.NotificationSettings newSettings)
        {
            using (var model = new TelegramModel())
            {
                NotificatioonsAndSound settings =
                    model.NotificatioonsAndSound.Where(x => x.SettingId == settingsId).FirstOrDefault();

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
                settings.AwayForType = null;

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
                model.SaveChanges();
            }
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
        private static int GetUserImageByName(string name)
        {
            using (var model = new TelegramModel())
            {
                foreach (var img in model.UserImage)
                {
                    if (img.Name == name) return img.Id;
                }
            }
            return 1;
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
                        toAdd.Date = DateTime.Now;

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
                        return new ColorHelper((byte)color.R, (byte)color.G, (byte)color.B);
                    }
                }
            }
            return new ColorHelper();
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
                res.IsBlurred = bg.
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

            using(var model = new TelegramModel())
            {
                foreach(var contact in model.ChosenPrivacyContacts)
                {
                    if(contact.IsShare == isShare && 
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
            using(var model = new TelegramModel())
            {
                PrivacySettingType res =  model.PrivacySettingType.Where(
                    x => x.Name == type.ToString()).FirstOrDefault();
                if (res is null) return 1;
                return res.Id;
                
            }

            return 1;
        }

            
            
            //SETTINGS OPTIONS


        //Chats



        public static void AddMessageInChat(int chatIndex, int userId,
            int toSendId, Message message)
        {
            using (var model = new TelegramModel())
            {
                Messages toAdd = new Messages();

                toAdd.UserId = userId;
                toAdd.FriendId = toSendId;

                toAdd.Message = message is TextMessage text ? text.Text : null;

                toAdd.ImageId = null;// to set
                toAdd.StickerId = message is MediaAction media && media.IsSticker ? GetStickerIdByName(media.MediaName) : null;

                to

                model.SaveChanges();

            }
        }

        private static int? GetImageIdByMediaAction(MediaAction media)
        {
            if (media.IsSticker) return GetStickerIdByName(media.MediaName);



            return null;
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

    }
}
