using Microsoft.Identity.Client;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.MainClasses.FolderObjs;
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
        public SoundSettings SoundNotifSettings { get; set; }
        public SysLanguage LanguageSettings { get; set; }
        public bool IsTabsOnTheLeft { get; set; }
        public int ChosenFolderId { get; set; }
        
        public MainSettings(int id, NotificationSettings notSettings,
            ChatSettings chatSettings, AdvancedSettings advSettings,
            PrivAndSecSettings privacySettings)
        {
            Id = id;
            NotSettings = notSettings;
            ChatsSettings = chatSettings;
            AdvSettings = advSettings;
            PrivacySettings = privacySettings;
            SoundNotifSettings = new SoundSettings();
            LanguageSettings = new SysLanguage();
        }

        public MainSettings()
        {
            Id = -1;
            NotSettings = new NotificationSettings();
            ChatsSettings = new ChatSettings();
            AdvSettings = new AdvancedSettings();
            PrivacySettings = new PrivAndSecSettings();
            IsTabsOnTheLeft = true;
            SoundNotifSettings = new SoundSettings();
            LanguageSettings = new SysLanguage();
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
