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
        public NotificationSettings NotSettings { private get; set; }
        public ChatSettings ChatsSettings { private get; set; }
        public AdvancedSettings AdvSettings { private get; set; }
        public PrivAndSecSettings PrivacySettings { private get; set; }

        public MainSettings(NotificationSettings notSettings, 
            ChatSettings chatSettings, AdvancedSettings advSettings, 
            PrivAndSecSettings privacySettings)
        {
            NotSettings = notSettings;
            ChatsSettings = chatSettings;
            AdvSettings = advSettings;
            PrivacySettings = privacySettings;
        }

        public MainSettings()
        {
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
