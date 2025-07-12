using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.UserSettings.SettingsTypes
{
    public class NotificationSettings
    {
        public bool IsDesktopNotifications { get; set; }
        public bool IsFlashTaskBar { get; set; }
        public bool IsAllowSounds { get; set; }
        public bool IsPrivateChats { get; set; }
        public bool IsPinnedMessages { get; set; }

        public NotificationSettings(bool desktop, bool taskBar, bool allowSounds,
                                    bool privateChats, bool pinnedMessages)
        {
            IsDesktopNotifications = desktop;
            IsFlashTaskBar = taskBar;
            IsAllowSounds = allowSounds;
            IsPrivateChats = privateChats;
            IsPinnedMessages = pinnedMessages;
        }

        public NotificationSettings()
        {
            IsDesktopNotifications = false;
            IsFlashTaskBar = false;
            IsAllowSounds = false;
            IsPrivateChats = false;
            IsPinnedMessages = false;
        }

    }
}
