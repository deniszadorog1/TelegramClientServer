using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class PasscodeSettings
    {
        public int Id { get; set; }
        public int MinutesTimer { get; set; }
        public bool IsWinUnLock { get; set; }
        public string PassCode { get; set; }
    
        public PasscodeSettings()
        {
            Id = -1;
            MinutesTimer = 0;
            IsWinUnLock = false;
            PassCode = string.Empty;
        }

        public PasscodeSettings(int minTimer, bool isWinLock,
            string passCode, int id)
        {
            Id = id;
            MinutesTimer = minTimer;
            IsWinUnLock = isWinLock;
            PassCode = passCode;
        }
    }
}
