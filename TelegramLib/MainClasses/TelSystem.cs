using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.UserSettings;
using TelegramLib.UserSettings.SettingsTypes;

namespace TelegramLib.MainClasses
{
    public class TelSystem
    {
        public User LoggedUser { get; set; }
        public MainSettings Settings { get; set; }

        public TelSystem(User user, MainSettings settings)
        {
            LoggedUser = user;
            Settings = settings;
        }
        
        //Test system
        public TelSystem()
        {
            LoggedUser = new User();
            Settings = new MainSettings();
        }
       
    }
}
