using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class PhoneNumberSub : PrivacySub
    {
        public AllOrNone WhoCanSearch { get; set; }

        public PhoneNumberSub(ShareWith type, List<UserContactcs> shareWithExps,
            List<UserContactcs> neverShareExps, AllOrNone whoSearch) : 
            base(type, shareWithExps, neverShareExps)
        {
            WhoCanSearch = whoSearch;
        }

        public PhoneNumberSub() : base()
        {
            WhoCanSearch = AllOrNone.Contact;
        }
    }
}
