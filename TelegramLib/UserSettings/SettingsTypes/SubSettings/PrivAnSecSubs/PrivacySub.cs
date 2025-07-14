using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class PrivacySub
    {
        public ShareWith ShareType { get; set; }
        public List<UserContactcs> ShareWithExps { get; set; }
        public List<UserContactcs> NeverShareExps { get; set; }
    
        public PrivacySub(ShareWith type, List<UserContactcs> shareWith,
            List<UserContactcs> neverShare)
        {
            ShareType = type;
            ShareWithExps = shareWith;
            NeverShareExps = neverShare;
        }

        public PrivacySub()
        {
            ShareType = ShareWith.Contacts;
            ShareWithExps = new List<UserContactcs>();
            NeverShareExps = new List<UserContactcs>();
        }
    }
}
