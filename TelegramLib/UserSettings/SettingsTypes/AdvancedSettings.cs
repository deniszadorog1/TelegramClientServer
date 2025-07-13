using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.UserSettings.SettingsTypes
{
    public class AdvancedSettings
    {
        public bool IsAskDownloadPath { get; set; }

        public bool IsShowChatName { get; set; }
        public bool IsShowTotalUnReads { get; set; }
        public bool IsUserWindowSysFrame { get; set; }

        public bool IsShowTrayIcon { get; set; }
        public bool IsShowTaskbarIcon { get; set; }
        public bool IsCloseToTaskbar { get; set; }
        public bool LaunchTelegram { get; set; }

        public bool IsUpdateAutomatically { get; set; }
        public bool IsInstallBetaVersion { get; set; }


        public AdvancedSettings(bool downloadPath, bool showChatNames, bool totalUnReads, bool windowsSysFrame, 
            bool trayIcon, bool showTaskbarIcon, bool closeToTaskbar, bool launchTg, 
            bool updateAuto, bool betaVersion)
        {
            IsAskDownloadPath = downloadPath;

            IsShowChatName = showChatNames;
            IsShowTotalUnReads = totalUnReads;
            IsUserWindowSysFrame = windowsSysFrame;

            IsShowTrayIcon = trayIcon;
            IsShowTaskbarIcon = showTaskbarIcon;
            IsCloseToTaskbar = closeToTaskbar;
            LaunchTelegram = launchTg;

            IsUpdateAutomatically = updateAuto;
            IsInstallBetaVersion = betaVersion;
        }

        public AdvancedSettings()
        {
            IsAskDownloadPath = false;

            IsShowChatName = true;
            IsShowTotalUnReads = false;
            IsUserWindowSysFrame = true;

            IsShowTrayIcon = false;
            IsShowTaskbarIcon = true;
            IsCloseToTaskbar = false;
            LaunchTelegram = true;

            IsUpdateAutomatically = false;
            IsInstallBetaVersion = true;
        }
    }
}
