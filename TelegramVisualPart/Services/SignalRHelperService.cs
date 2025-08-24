using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
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

            if (settings.PrivacySettings.LastSeenPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                type == IsPrivacyException.NeverShare)
            {
                textBlock.Text = "You cant see this LOOOOLL";
                return;
            }
            textBlock.Text = user.PhoneNumber.ToString();
        }
    }
}
