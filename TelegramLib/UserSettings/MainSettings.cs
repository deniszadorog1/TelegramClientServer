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
        public NotificationSettings NotSettings { get; set; }
        public ChatSettings ChatsSettings { get; set; }

        public MainSettings(NotificationSettings notSettings, 
            ChatSettings chatSettings)
        {
            NotSettings = notSettings;
            ChatsSettings = chatSettings;
        }

        public MainSettings()
        {
            NotSettings = new NotificationSettings();
            ChatsSettings = new ChatSettings();
        }

        public NotificationSettings GetNotSettings()
        {
            return NotSettings;
        }

        public ChatSettings GetChatSettings()
        {
            return ChatsSettings;
        }
    }
}
