using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;

namespace TelegramLib.UserSettings.SettingsTypes
{
    public class PrivAndSecSettings
    {
        public string? LocalPasscode { get; set; }
        public List<UserContactcs> BlockedUsers { get; set; }
        
        public AwayForTime SelfDeleteTime { get; set; }

        public PhoneNumberSub PhonePrivacy { get; set; }
        public LastSeenSub LastSeenPrivacy { get; set; }
        public ProfilePhotosSub ProfPhotoPrivacy { get; set; }
        public ForwardedMessagesSub ForwardMesPrivacy { get; set; }
        public MessagesSub MessagesPrivacy { get; set; }
        public DateOfBirthSub DateBirthPrivacy { get; set; }
        public BioSub BioPrivacy { get; set; }

        public PrivAndSecSettings(string? passCode, AwayForTime destructTime,
            List<UserContactcs> blocked,
            PhoneNumberSub phonePrivacy,
            LastSeenSub lastSeenPrivacy,
            ProfilePhotosSub profPhotoPrivacy,
            ForwardedMessagesSub forwardMesPrivacy,
            MessagesSub messPrivacy,
            DateOfBirthSub birthDatePrivacy,
            BioSub bioPrivacy)
        {
            LocalPasscode = passCode;
            SelfDeleteTime = destructTime;
            BlockedUsers = blocked;

            PhonePrivacy = phonePrivacy;
            LastSeenPrivacy = lastSeenPrivacy;
            ProfPhotoPrivacy = profPhotoPrivacy;
            ForwardMesPrivacy = forwardMesPrivacy;
            MessagesPrivacy = messPrivacy;
            DateBirthPrivacy = birthDatePrivacy;
            BioPrivacy = bioPrivacy;
        }

        public PrivAndSecSettings()
        {
            LocalPasscode = "passcodeTEST";
            SelfDeleteTime = AwayForTime.SixMonths;
            BlockedUsers = new List<UserContactcs>();

            PhonePrivacy = new PhoneNumberSub();
            LastSeenPrivacy = new LastSeenSub();
            ProfPhotoPrivacy = new ProfilePhotosSub();
            ForwardMesPrivacy = new ForwardedMessagesSub();
            MessagesPrivacy = new MessagesSub();
            DateBirthPrivacy = new DateOfBirthSub();
            BioPrivacy = new BioSub();
        }
    }
}
