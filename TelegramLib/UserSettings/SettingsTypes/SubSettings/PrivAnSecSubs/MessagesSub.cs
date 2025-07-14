using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class MessagesSub
    {
        public ShareWith WhoCanSend { get; set; }

        public MessagesSub(ShareWith whoCanSend)
        {
            WhoCanSend = whoCanSend;
        }

        public MessagesSub()
        {
            WhoCanSend = ShareWith.Everybody;
        }
    }
}
