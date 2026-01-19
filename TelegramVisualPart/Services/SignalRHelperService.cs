using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.MyProfile.SetInformation;
using PrivacySettingType = TelegramVisualPart.Enums.PrivacySettingType;

namespace TelegramVisualPart.Services
{
    public static class SignalRHelperService
    {
        private static TelSystem _system;

        public static void SetStatSystem(TelSystem system)
        {
            _system = system;
        }

        public static async Task<IsPrivacyException> GetTypeByUser(TelegramLib.MainClasses.User user,
        Enums.PrivacySettingType settingType, MainSettings settings = null)
        {
            //Test method
            if (settings is null) settings = await ApiService.GetSettingsByUserId(user.Id);

            if (settingType == Enums.PrivacySettingType.LastSeen)
            {
                return settings.PrivacySettings.LastSeenPrivacy.ShareWithExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.LastSeenPrivacy.NeverShareExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            else if (settingType == PrivacySettingType.PhoneNumber)
            {
                return settings.PrivacySettings.PhonePrivacy.ShareWithExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.PhonePrivacy.NeverShareExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            else if (settingType == PrivacySettingType.DateBirth)
            {
                return settings.PrivacySettings.DateBirthPrivacy.ShareWithExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.DateBirthPrivacy.NeverShareExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            else if (settingType == PrivacySettingType.ProfilePhotos)
            {
                return settings.PrivacySettings.ProfPhotoPrivacy.ShareWithExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.ProfPhotoPrivacy.NeverShareExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            else if (settingType == PrivacySettingType.Bio)
            {
                return settings.PrivacySettings.BioPrivacy.ShareWithExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.BioPrivacy.NeverShareExps.Select(x => x.Id).Contains(_system.LoggedUser.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }

            return IsPrivacyException.Null;
        }

        public static async Task<ShareWith> GetShareType(TelegramLib.MainClasses.User user,
        Enums.PrivacySettingType settingType, MainSettings settings = null)
        {
            if (settings is null) settings = await ApiService.GetSettingsByUserId(user.Id);

            switch (settingType)
            {
                case PrivacySettingType.PhoneNumber:
                    return settings.PrivacySettings.PhonePrivacy.ShareType;
                case PrivacySettingType.LastSeen:
                    return settings.PrivacySettings.LastSeenPrivacy.ShareType;
                case PrivacySettingType.ProfilePhotos:
                    return settings.PrivacySettings.ProfPhotoPrivacy.ShareType;
                case PrivacySettingType.ForwardedMessages:
                    return settings.PrivacySettings.ForwardMesPrivacy.ShareType;
                case PrivacySettingType.Messages:
                    return settings.PrivacySettings.MessagesPrivacy.WhoCanSend;
                case PrivacySettingType.DateBirth:
                    return settings.PrivacySettings.DateBirthPrivacy.ShareType;
                case PrivacySettingType.Bio:
                    return settings.PrivacySettings.BioPrivacy.ShareType;
            }
            return ShareWith.Everybody;
        }

        public static async Task SetLastSeenString(TelegramLib.MainClasses.User user,
            IsPrivacyException type, TelegramLib.MainClasses.UserChat chat, TextBlock textBlock,      
            MainSettings settings = null)
        {
            if (chat is not TelegramLib.MainClasses.SavedMessagesChat &&
                (chat is null || chat.Chatter is null || chat.GetChatter().Id != user.Id)) return;
            await SetLastSeenStatus(user, type, textBlock, settings: settings);
        }

        public static async Task SetLastSeenStatus(TelegramLib.MainClasses.User user,
        IsPrivacyException type, TextBlock textBlock, 
            MainSettings settings = null)
        {
            if(settings is null) settings = await ApiService.GetSettingsByUserId(user.Id);

            bool isStop = await IsAndSetStopPath(PrivacySettingType.LastSeen, user, 
                settings: settings);

            if (type == IsPrivacyException.Share)
            {
                HelperService.SetOnlineStatusInTextBox(
                    textBlock, user.IsOnline, user.LastSeenOnline);
                return;
            }

            if (settings.PrivacySettings.LastSeenPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                type == IsPrivacyException.NeverShare)
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                textBlock.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            if (isStop)
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Gray);
                textBlock.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            HelperService.SetOnlineStatusInTextBox(
                textBlock, user.IsOnline, user.LastSeenOnline);
        }

        public static async Task<bool> IsCanBeAddedByShareType(TelegramLib.MainClasses.User user,
        IsPrivacyException type)
        {
            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            bool isStop = await IsAndSetStopPath(PrivacySettingType.LastSeen, user,
                settings: settings);

            if (isStop) return false;

            if (type == IsPrivacyException.Share) return true;

            if (settings.PrivacySettings.LastSeenPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                type == IsPrivacyException.NeverShare)
            {
                return false;
            }
            return true;
        }

        public static async Task SetPhoneNumber(TelegramLib.MainClasses.User user,
            IsPrivacyException type, TelegramLib.MainClasses.UserChat chat,
            TextBlock textBlock, MainSettings settings = null)
        {
            if (chat is not TelegramLib.MainClasses.SavedMessagesChat && 
               (chat is null || chat is TelegramLib.MainClasses.SavedMessagesChat || chat.GetChatter().Id != user.Id)) return;

            if(settings is null) settings = await ApiService.GetSettingsByUserId(user.Id);

            bool isStop = await IsAndSetStopPath(PrivacySettingType.PhoneNumber, user, 
                settings:settings);

            if (type == IsPrivacyException.Share)
            {
                textBlock.Text = user.PhoneNumber.ToString();
                return;
            }

            if (settings.PrivacySettings.PhonePrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                type == IsPrivacyException.NeverShare)
            {
                textBlock.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            if (isStop)
            {
                textBlock.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            textBlock.Text = user.PhoneNumber.ToString();
        }

        public static async Task SetBirthDate(TelegramLib.MainClasses.User user,
            UserChat chat, TextBlock textBlock, MainSettings settings = null)
        {
            if (chat is not TelegramLib.MainClasses.SavedMessagesChat && 
                (chat is TelegramLib.MainClasses.SavedMessagesChat || chat.GetChatter().Id != user.Id)) return;

            IsPrivacyException shareType =
                await GetTypeByUser(user, Enums.PrivacySettingType.DateBirth, settings: settings);

            if(settings is null) settings = await ApiService.GetSettingsByUserId(user.Id);

            string yearStr = user.BirthDay is null ? " " :
                user.BirthDay.Value.Year == 1 ? " " : user.BirthDay.Value.Year.ToString();

            string birthString = user.BirthDay is null ? VisConstParamsJsonService.GetStringByName("BirthDatNotSet") :
                $"{user.BirthDay.Value.Day}.{user.BirthDay.Value.Month}.{yearStr}";

            bool isStop = await IsAndSetStopPath(PrivacySettingType.DateBirth, user,
                settings: settings);

            if (shareType == IsPrivacyException.Share)
            {
                textBlock.Text = birthString;
                return;
            }

            if (settings.PrivacySettings.DateBirthPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                textBlock.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            if (isStop)
            {
                textBlock.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            textBlock.Text = birthString;
        }

        public static async Task SetContactPhoto(TelegramLib.MainClasses.User user,
            UserChat chat, ImageBrush brush, Ellipse ellipse, MainSettings settings = null)
        {
            if (chat is not TelegramLib.MainClasses.SavedMessagesChat && chat.GetChatter().Id != user.Id) return;

            await SetPhotoInEllipse(user, brush, ellipse, settings: settings);
        }

        public static async Task SetPhotoInEllipse(TelegramLib.MainClasses.User user,
            ImageBrush brush, Ellipse ellipse, MainSettings settings = null)
        {
            if (user is null) return;
            if(settings is null) settings = await ApiService.GetSettingsByUserId(user.Id);

            IsPrivacyException shareType =
                await GetTypeByUser(user, Enums.PrivacySettingType.ProfilePhotos, settings: settings);

            bool isStop = await IsAndSetStopPath(Enums.PrivacySettingType.ProfilePhotos, user,
                settings: settings);

            string stopSignPath = FilesAction.GetSystemImagePath("StopSign.png");

            TelegramLib.MainClasses.UserParams.UserImage mask =
                await ApiService.GetContactMask(_system.LoggedUser.Id, user.Id);

            if (mask is not null &&

                (user.UserImages.Count > 0 &&
                System.IO.Path.GetFileName(mask.Name) != System.IO.Path.GetFileName(user.UserImages.First().Name)))
            {
                user.UserImages.Insert(0,
                    new TelegramLib.MainClasses.UserParams.UserImage(System.IO.Path.GetFileName(mask.Name), mask.Date));
            }

            BitmapImage image;

            if (shareType == IsPrivacyException.Share ||
                mask is not null)
            {
                image = LoadBitmap(FilesAction.GetUserImagePath(user.GetFirstImageNameInString()));
                brush.ImageSource = image;

/*                brush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
                UriKind.Absolute));*/
                ellipse.IsHitTestVisible = true;
                return;
            }

            if (settings.PrivacySettings.ProfPhotoPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                image = LoadBitmap(stopSignPath);
                brush.ImageSource = image;

                //brush.ImageSource = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
                //ellipse.IsHitTestVisible = false;
                return;
            }

            if (isStop)
            {
                brush.ImageSource = _photoBitmapImg;
                return;
            };

            image = LoadBitmap(FilesAction.GetUserImagePath(user.GetFirstImageNameInString()));
            brush.ImageSource = image;
            ellipse.Visibility = System.Windows.Visibility.Visible;

/*            brush.ImageSource = new BitmapImage(new Uri
            (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
            UriKind.Absolute));*/
            ellipse.IsHitTestVisible = true;
        }

        private static BitmapImage LoadBitmap(string path)
        {
            using var fs = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad; 
            bmp.StreamSource = fs;
            bmp.EndInit();
            bmp.Freeze();

            return bmp;
        }

        public static async Task<string> GetUserPhotoToSet(TelegramLib.MainClasses.User user)
        {
            IsPrivacyException shareType =
               await GetTypeByUser(user, Enums.PrivacySettingType.ProfilePhotos);

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            if (shareType == IsPrivacyException.Share ||
                user.ImageMask is not null)
            {
                return user.GetFirstImageNameInString();
            }

            if (settings.PrivacySettings.ProfPhotoPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                return null;// FilesAction.GetSystemImagePath("StopSign.png");
            }
            return user.GetFirstImageNameInString();
        }

        private static bool _isPhotoCanBePushed;
        private static BitmapImage _photoBitmapImg;
        public static async Task SetFastParamsForPhotoUpdate(TelegramLib.MainClasses.User user)
        {
            //user - logged
            //system - system

            IsPrivacyException shareType =
                    await GetTypeByUser(user, Enums.PrivacySettingType.ProfilePhotos);

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            bool isStop = await IsAndSetStopPath(Enums.PrivacySettingType.ProfilePhotos, user);

            if (shareType == IsPrivacyException.Share)
            {
                _photoBitmapImg = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
                UriKind.Absolute));
                _isPhotoCanBePushed = true;
                return;
            }

            if (settings.PrivacySettings.ProfPhotoPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                SetStopSign();
                /*string stopSignPath = FilesAction.GetSystemImagePath("StopSign.png");
                _photoBitmapImg = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));*/
                _isPhotoCanBePushed = false;
                return;
            }

            if (isStop)
            {
                return;
            };

            _photoBitmapImg = new BitmapImage(new Uri
            (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
            UriKind.Absolute));
            _isPhotoCanBePushed = true;
        }

        public static async Task<bool> IsAndSetStopPath(PrivacySettingType type,
            TelegramLib.MainClasses.User user, MainSettings settings = null)
        {
/*            //Need to make it better somehow
            TelegramLib.MainClasses.TelSystem system = 
                await ApiService.GetTelSystem(user.Login, user.Password);*/

            ShareWith shareType = await GetShareType(user, type, settings);

            switch (shareType)
            {
                case ShareWith.Everybody:
                    return false;
                case ShareWith.Contacts:

                    bool isContact = (bool)await ApiService.IsChatterIdIsContact(user.Id, _system.LoggedUser.Id);
                    // system.IsChatterIdIsContact(_system.LoggedUser.Id);

                    if (!isContact)
                    {
                        SetStopSign();
                    }
                    else
                    {
                        SetBasicImage(user);
                        return false;
                    }
                    return true;
                case ShareWith.Nobody:
                    SetStopSign();
                    return true;
            }
            return false;
        }

        private static void SetBasicImage(TelegramLib.MainClasses.User user)
        {
            _photoBitmapImg = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
                UriKind.Absolute));
        }

        private static void SetStopSign()
        {
            string stopSignPath = FilesAction.GetSystemImagePath("StopSign.png");
            _photoBitmapImg = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
        }

        public static void FastSetContactPhoto(TelegramLib.MainClasses.User user,
            UserChat chat, ImageBrush brush, Ellipse ellipse)
        {
            brush.ImageSource = _photoBitmapImg;



            //ellipse.IsHitTestVisible = true;
            //ellipse.IsHitTestVisible = _isPhotoCanBePushed;
        }
    }
}
