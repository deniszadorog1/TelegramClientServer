using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Models;

using model = TelegramLib.Models;
using mainClass = TelegramLib.MainClasses;
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

namespace TelegramLib.Services
{
    public static class DbService
    {
        public static List<mainClass.User> GetAllUsers()
        {
            List<mainClass.User> res = new List<mainClass.User>();
            using (var model = new TelegramModel())
            {
                foreach (var user in model.User)
                {
                    // ADD BIO, COLOR, USERNAME, BLOCKEDUsers, USERIMAGES
                    res.Add(new mainClass.User(user.Id, user.Login, user.Password, user.Name, user.Surname, "ADD BIO",
                        new Helpers.ColorHelper(), user.PhoneNumber, "ADD USERNAME", user.Birthday, user.BlockedUsers, new List<UserImage>()))
                }
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
                User user = new User();

                user.Name = name;
                user.Surname = surname;
                user.PhoneNumber = phoneNumber;
                user.Birthday = birthdate;
                user.Login = login;
                user.LastOnline = DateTime.Now;

                model.SaveChanges();
            }
        }
        public static void UpdateUser(mainClass.User user)
        {
            using(var model = new TelegramModel())
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
            using(var model = new TelegramModel())
            {
                List<Contacts> contacts = model.Contacts.Where(x => x.UserId == userId).ToList();

                List<mainClass.UserContactcs> resContacts = new List<mainClass.UserContactcs>();
                foreach(var tempContact in contacts)
                {
                    mainClass.UserContactcs toAdd = new mainClass.UserContactcs();

                    toAdd.Id = tempContact.Id;
                    toAdd.Name = tempContact.Name;
                    toAdd.UserName = tempContact.User.Name;
                    toAdd.BirthDate = tempContact.User.Birthday;
                    //toAdd.BIO = tempContact.User.
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

        private static void GetUsersBlockedIds(int userid)
        {

        }

        //Contacts



        // SETTINGS OPTIONS
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
                foreach(var type in model.AwayForType)
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
                foreach(var type in model.AwayForType)
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
            using(var model = new TelegramModel())
            {
                foreach(var img in model.UserImage)
                {
                    if (img.Name == name) return img.Id;
                }
            }
            return 1;
        }

        //SETTINGS OPTIONS

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
