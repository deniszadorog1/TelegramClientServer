using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.UserSettings.SettingsTypes;

namespace TelegramLib.UserSettings
{
    public class MainSettings
    {
        public int Id { get; set; }
        public NotificationSettings NotSettings { get; set; }
        public ChatSettings ChatsSettings { get; set; }
        public AdvancedSettings AdvSettings { get; set; }
        public PrivAndSecSettings PrivacySettings { get; set; }

        public MainSettings(int id, NotificationSettings notSettings,
            ChatSettings chatSettings, AdvancedSettings advSettings,
            PrivAndSecSettings privacySettings)
        {
            Id = id;
            NotSettings = notSettings;
            ChatsSettings = chatSettings;
            AdvSettings = advSettings;
            PrivacySettings = privacySettings;
        }

        public MainSettings()
        {
            Id = -1;
            NotSettings = new NotificationSettings();
            ChatsSettings = new ChatSettings();
            AdvSettings = new AdvancedSettings();
            PrivacySettings = new PrivAndSecSettings();
        }

        public NotificationSettings GetNotSettings()
        {
            return NotSettings;
        }

        public ChatSettings GetChatSettings()
        {
            return ChatsSettings;
        }

        public AdvancedSettings GetAdvSettings()
        {
            return AdvSettings;
        }

        public PrivAndSecSettings GetPrivacySettings()
        {
            return PrivacySettings;
        }
    }
}
