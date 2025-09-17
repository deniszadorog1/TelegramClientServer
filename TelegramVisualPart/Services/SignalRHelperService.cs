using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        public static async Task<IsPrivacyException> GetTypeByUser(TelegramLib.MainClasses.User user,
        Enums.PrivacySettingType settingType)
        {
            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            if (settingType == Enums.PrivacySettingType.LastSeen)
            {
                return settings.PrivacySettings.LastSeenPrivacy.ShareWithExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.LastSeenPrivacy.NeverShareExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            else if (settingType == PrivacySettingType.PhoneNumber)
            {
                return settings.PrivacySettings.PhonePrivacy.ShareWithExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.PhonePrivacy.NeverShareExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            else if (settingType == PrivacySettingType.DateBirth)
            {
                return settings.PrivacySettings.DateBirthPrivacy.ShareWithExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.DateBirthPrivacy.NeverShareExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            else if (settingType == PrivacySettingType.ProfilePhotos)
            {
                return settings.PrivacySettings.ProfPhotoPrivacy.ShareWithExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.ProfPhotoPrivacy.NeverShareExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            return IsPrivacyException.Null;
        }

        public static async Task SetLastSeenString(TelegramLib.MainClasses.User user,
        IsPrivacyException type, TelegramLib.MainClasses.UserChat chat, TextBlock textBlock)
        {
            if (chat is null || chat.GetChatter().ContactUserId != user.Id) return;
            await SetLastSeenStatus(user, type, textBlock);
        }

        public static async Task SetLastSeenStatus(TelegramLib.MainClasses.User user,
        IsPrivacyException type, TextBlock textBlock)
        {
            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

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
            HelperService.SetOnlineStatusInTextBox(
                textBlock, user.IsOnline, user.LastSeenOnline);
        }

        public static async Task SetPhoneNumber(TelegramLib.MainClasses.User user,
            IsPrivacyException type, TelegramLib.MainClasses.UserChat chat, TextBlock textBlock)
        {
            if (chat is null || chat.GetChatter().ContactUserId != user.Id) return;

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

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
            textBlock.Text = user.PhoneNumber.ToString();
        }

        public static async Task SetBirthDate(TelegramLib.MainClasses.User user,
            UserChat chat, TextBlock textBlock)
        {
            if (chat.GetChatter().ContactUserId != user.Id) return;

            IsPrivacyException shareType =
                await GetTypeByUser(user, Enums.PrivacySettingType.DateBirth);

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            string yearStr = user.BirthDay is null ? " " : 
                user.BirthDay.Value.Year == 1 ? " " : user.BirthDay.Value.Year.ToString();

            string birthString = user.BirthDay is null ? VisConstParamsJsonService.GetStringByName("BirthDatNotSet") :
                $"{user.BirthDay.Value.Day}.{user.BirthDay.Value.Month}.{yearStr}";

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
            textBlock.Text = birthString;
        }

        public static async Task SetContactPhoto(TelegramLib.MainClasses.User user,
            UserChat chat, ImageBrush brush, Ellipse ellipse)
        {
            if (chat.GetChatter().ContactUserId != user.Id) return;

            await SetPhotoInEllipse(user, brush, ellipse);
        }

        public static async Task SetPhotoInEllipse(TelegramLib.MainClasses.User user, 
            ImageBrush brush, Ellipse ellipse)
        {
            IsPrivacyException shareType =
                await GetTypeByUser(user, Enums.PrivacySettingType.ProfilePhotos);

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            string stopSignPath = FilesAction.GetSystemImagePath("StopSign.png");

            if (shareType == IsPrivacyException.Share)
            {
                brush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
                UriKind.Absolute));
                ellipse.IsHitTestVisible = true;
                return;
            }

            if (settings.PrivacySettings.ProfPhotoPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                brush.ImageSource = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
                ellipse.IsHitTestVisible = false;
                return;
            }
            brush.ImageSource = new BitmapImage(new Uri
            (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
            UriKind.Absolute));
            ellipse.IsHitTestVisible = true;
        }

        public static async Task<string> GetUserPhotoToSet(TelegramLib.MainClasses.User user)
        {
            IsPrivacyException shareType =
               await GetTypeByUser(user, Enums.PrivacySettingType.ProfilePhotos);

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            if (shareType == IsPrivacyException.Share)
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
            IsPrivacyException shareType =
                    await GetTypeByUser(user, Enums.PrivacySettingType.ProfilePhotos);

            if (shareType == IsPrivacyException.Share)
            {
                _photoBitmapImg = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
                UriKind.Absolute));
                _isPhotoCanBePushed = true;
                return;
            }

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            if (settings.PrivacySettings.ProfPhotoPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                string stopSignPath = FilesAction.GetSystemImagePath("StopSign.png");
                _photoBitmapImg = new BitmapImage(new Uri(stopSignPath, UriKind.Absolute));
                _isPhotoCanBePushed = false;
                return;
            }
            _photoBitmapImg = new BitmapImage(new Uri
            (FilesAction.GetUserImagePath(user.GetFirstImageNameInString()),
            UriKind.Absolute));
            _isPhotoCanBePushed = true;
        }

        public static void FastSetContactPhoto(TelegramLib.MainClasses.User user,
            UserChat chat, ImageBrush brush, Ellipse ellipse)
        {
            brush.ImageSource = _photoBitmapImg;
            ellipse.IsHitTestVisible = _isPhotoCanBePushed;
        }
    }
}
