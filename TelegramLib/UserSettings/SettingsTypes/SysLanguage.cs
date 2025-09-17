using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.Language;

namespace TelegramLib.UserSettings.SettingsTypes
{
    public class SysLanguage
    {
        public int Id { get; set; }
        public LanguageType Type { get; set; }

        public SysLanguage()
        {
            Id = -1;
            Type = LanguageType.English;
        }

        public SysLanguage(int id, LanguageType type)
        {
            Id = id;
            Type = type;
        }
    }
}
