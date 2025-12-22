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
        public int Id { get; set; }
        public List<User> BlockedUsers { get; set; }

        public AwayForTime SelfDeleteTime { get; set; }

        public PhoneNumberSub PhonePrivacy { get; set; }
        public LastSeenSub LastSeenPrivacy { get; set; }
        public ProfilePhotosSub ProfPhotoPrivacy { get; set; }
        public ForwardedMessagesSub ForwardMesPrivacy { get; set; }
        public MessagesSub MessagesPrivacy { get; set; }
        public DateOfBirthSub DateBirthPrivacy { get; set; }
        public BioSub BioPrivacy { get; set; }
        public PasscodeSettings PassCode { get; set; }

        public PrivAndSecSettings(int id, AwayForTime destructTime,
            List<User> blocked,
            PhoneNumberSub phonePrivacy,
            LastSeenSub lastSeenPrivacy,
            ProfilePhotosSub profPhotoPrivacy,
            ForwardedMessagesSub forwardMesPrivacy,
            MessagesSub messPrivacy,
            DateOfBirthSub birthDatePrivacy,
            BioSub bioPrivacy,
            PasscodeSettings passCodeSet)
        {
            Id = id;
            SelfDeleteTime = destructTime;
            BlockedUsers = blocked;

            PhonePrivacy = phonePrivacy;
            LastSeenPrivacy = lastSeenPrivacy;
            ProfPhotoPrivacy = profPhotoPrivacy;
            ForwardMesPrivacy = forwardMesPrivacy;
            MessagesPrivacy = messPrivacy;
            DateBirthPrivacy = birthDatePrivacy;
            BioPrivacy = bioPrivacy;
            PassCode = passCodeSet;
        }

        public PrivAndSecSettings()
        {
            Id = 1;
            SelfDeleteTime = AwayForTime.SixMonths;
            BlockedUsers = new List<User>();

            PhonePrivacy = new PhoneNumberSub();
            LastSeenPrivacy = new LastSeenSub();
            ProfPhotoPrivacy = new ProfilePhotosSub();
            ForwardMesPrivacy = new ForwardedMessagesSub();
            MessagesPrivacy = new MessagesSub();
            DateBirthPrivacy = new DateOfBirthSub();
            BioPrivacy = new BioSub();
            PassCode = null;
        }

        public void SetBlockParams(bool isBlock, User user)
        {
            if (isBlock)
            {
                if(!LastSeenPrivacy.NeverShareExps.Any(x => x.Id == user.Id)) LastSeenPrivacy.NeverShareExps.Add(user);
                LastSeenPrivacy.ShareWithExps.Remove(LastSeenPrivacy.ShareWithExps.FirstOrDefault(x => x.Id == user.Id));

                if (!ProfPhotoPrivacy.NeverShareExps.Any(x => x.Id == user.Id)) ProfPhotoPrivacy.NeverShareExps.Add(user);
                ProfPhotoPrivacy.ShareWithExps.Remove(ProfPhotoPrivacy.ShareWithExps.FirstOrDefault(x => x.Id == user.Id));
                return;
            }

            LastSeenPrivacy.NeverShareExps.Remove(LastSeenPrivacy.NeverShareExps.FirstOrDefault(x => x.Id == user.Id));
            ProfPhotoPrivacy.NeverShareExps.Remove(ProfPhotoPrivacy.NeverShareExps.FirstOrDefault(x => x.Id == user.Id));
        }        
    }
}
