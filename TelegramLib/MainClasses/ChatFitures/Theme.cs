using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.ChatSettings;
using TelegramLib.Helpers;

namespace TelegramLib.MainClasses.ChatFitures
{
    public class Theme
    {
        public ThemeType Type { get; set; }
        public ColorHelper Color { get; set; }
        public bool IsChosen { get; set; }

        public Theme(ThemeType type, ColorHelper color, bool isChosen)
        {
            Type = type;
            Color = color;
            IsChosen = isChosen;
        }

        public Theme()
        {
            Type = ThemeType.Night;
            Color = new ColorHelper();
            IsChosen = false;
        }
    }
}
