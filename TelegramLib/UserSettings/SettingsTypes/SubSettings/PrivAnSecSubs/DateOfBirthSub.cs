using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class DateOfBirthSub : PrivacySub
    {
        public DateOfBirthSub(ShareWith type, List<UserContactcs> shareWithExps,
             List<UserContactcs> neverShareExps) : base(type, shareWithExps, neverShareExps)
        {

        }

        public DateOfBirthSub() : base()
        {

        }
    }
}
