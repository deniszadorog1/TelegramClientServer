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
        public int Id { get; set; }
        public ThemeType Type { get; set; }
        public ColorHelper Color { get; set; }

        public Theme(int id, ThemeType type, ColorHelper color)
        {
            Id = id;
            Type = type;
            Color = color;
        }

        public Theme()
        {
            Id = -1;
            Type = ThemeType.Night;
            Color = new ColorHelper();
        }
    }
}
