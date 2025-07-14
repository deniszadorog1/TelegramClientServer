using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class LastSeenSub : PrivacySub
    {
        public bool IsHideReadAction { get; set; } 
        public LastSeenSub(ShareWith type, List<UserContactcs> shareWithExps,
               List<UserContactcs> neverShareExps, bool isHideReadAction) :
               base(type, shareWithExps, neverShareExps)
        {
            IsHideReadAction = isHideReadAction;
        }

        public LastSeenSub() : base()
        {
            IsHideReadAction = false;
        }
    }
}
