namespace TelegramLib.UserSettings.SettingsTypes
{
    public class NotificationSettings
    {
        public int Id { get; set; }
        public bool IsDesktopNotifications { get; set; }
        public bool IsFlashTaskBar { get; set; }
        public bool IsAllowSounds { get; set; }
        public bool IsPrivateChats { get; set; }
        public bool IsPinnedMessages { get; set; }

        public NotificationSettings(int id, bool desktop, bool taskBar, bool allowSounds,
                                    bool privateChats, bool pinnedMessages)
        {
            Id = id;
            IsDesktopNotifications = desktop;
            IsFlashTaskBar = taskBar;
            IsAllowSounds = allowSounds;
            IsPrivateChats = privateChats;
            IsPinnedMessages = pinnedMessages;
        }

        public NotificationSettings()
        {
            return;
            Id = -1;
            IsDesktopNotifications = false;
            IsFlashTaskBar = false;
            IsAllowSounds = false;
            IsPrivateChats = false;
            IsPinnedMessages = false;
        }

    }
}
