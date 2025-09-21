using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class ForwardedMessagesSub : PrivacySub
    {
        public ForwardedMessagesSub(ShareWith type, List<User> shareWithExps,
            List<User> neverShareExps) : base(type, shareWithExps, neverShareExps)
        {

        }

        public ForwardedMessagesSub() : base()
        {

        }
    }
}
