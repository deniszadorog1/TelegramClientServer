using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs
{
    public class ProfilePhotosSub : PrivacySub
    {
        public string PublicPhotoPath { get; set; }
        public ProfilePhotosSub(ShareWith type, List<User> shareWithExps,
            List<User> neverShareExps, string photoPath) : base(type, shareWithExps, neverShareExps)
        {
            PublicPhotoPath = photoPath;
        }

        public ProfilePhotosSub() : base()
        {
            PublicPhotoPath = string.Empty;
        }
    }
}
