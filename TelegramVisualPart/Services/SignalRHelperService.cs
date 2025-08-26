using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Enums;
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
            else if(settingType == PrivacySettingType.DateBirth)
            {
                return settings.PrivacySettings.DateBirthPrivacy.ShareWithExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.Share :
                     settings.PrivacySettings.DateBirthPrivacy.NeverShareExps.Select(x => x.Id).Contains(user.Id) ? IsPrivacyException.NeverShare :
                     IsPrivacyException.Null;
            }
            return IsPrivacyException.Null;
        }

        public static async Task SetLastSeenString(TelegramLib.MainClasses.User user,
        IsPrivacyException type, TelegramLib.MainClasses.UserChat chat, TextBlock textBlock)
        {
            if (chat is null || chat.GetChatter().ContactUserId != user.Id) return;

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
                textBlock.Text = "You cant see this LOOOOLL";
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
                textBlock.Text = "You cant see this LOOOOLL";
                return;
            }
            textBlock.Text = user.PhoneNumber.ToString();
        }

        public static async Task SetBirthDate(TelegramLib.MainClasses.User user,
            UserChat _chat, TextBlock textBlock)
        {
            if (_chat.GetChatter().ContactUserId != user.Id) return;

            IsPrivacyException shareType =
                await GetTypeByUser(user, Enums.PrivacySettingType.DateBirth);

            MainSettings settings = await ApiService.GetSettingsByUserId(user.Id);

            string birthString =  user.BirthDay is null ? "Not born yeat" :
                $"{user.BirthDay.Value.Day}.{user.BirthDay.Value.Month}.{user.BirthDay.Value.Year}";

            if (shareType == IsPrivacyException.Share)
            {
                textBlock.Text = birthString;
                return;
            }

            if (settings.PrivacySettings.DateBirthPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                textBlock.Text = "You cant see this LOOOOLL";
                return;
            }
            textBlock.Text = birthString;
        }
    }
}
