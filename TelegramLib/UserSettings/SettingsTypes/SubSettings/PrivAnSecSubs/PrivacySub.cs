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
        public List<User> ShareWithExps { get; set; }
        public List<User> NeverShareExps { get; set; }
    
        public PrivacySub(ShareWith type, List<User> shareWith,
            List<User> neverShare)
        {
            ShareType = type;
            ShareWithExps = shareWith;
            NeverShareExps = neverShare;
        }

        public PrivacySub()
        {
            ShareType = ShareWith.Contacts;
            ShareWithExps = new List<User>();
            NeverShareExps = new List<User>();
        }

        public int GetAmountOfSharedExps()
        {
            return ShareWithExps.Count();
        }

        public int GetAmountOfNeverSharedExps()
        {
            return NeverShareExps.Count();
        }

        public bool IsUserPageCanBeSeen(List<UserContactcs> contacts, int loggedUserId)
        {
            switch (ShareType)
            {
                case ShareWith.Everybody:
                    {
                        return true;
                    }
                case ShareWith.Contacts:
                    {
                        return contacts.Any(x => x.ContactUserId == loggedUserId);
                    }
                case ShareWith.Nobody:
                    {
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
}
}
