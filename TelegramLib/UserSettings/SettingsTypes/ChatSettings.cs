using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Enums.Settings.ChatSettings;
using TelegramLib.Helpers;
using TelegramLib.UserSettings.SettingsTypes.SubSettings;
using TelegramLib;


namespace TelegramLib.UserSettings.SettingsTypes
{
    public class ChatSettings
    {
        public int Id { get; set; }
        public ThemeType Theme { get; set; }
        public ColorHelper ChosenColor { get; set; }
        public AutoNightMode NightMode { get; set; }
        public string FontName { get; set; }
        public bool IsSendWithEnter { get; set; }
        public ChatWallpaper Wallpaper { get; set; }
        public List<string> PossibleWallpapers { get; set; }

        public ChatSettings(int id, ThemeType theme, ColorHelper color, AutoNightMode nightMode,
                            string fontName, ChatWallpaper wallpaper, bool isSentWithEnter,
                            List<string> wallpapers)
        {
            Id = id;
            Theme = theme;
            ChosenColor = color;
            NightMode = nightMode;
            FontName = fontName;
            IsSendWithEnter = isSentWithEnter;
            Wallpaper = wallpaper;
            PossibleWallpapers = wallpapers;
        }

        public ChatSettings()
        {
            Id = -1;
            Theme = ThemeType.Tinted;
            ChosenColor = new ColorHelper(210, 117, 112); //Test seventh color
            NightMode = AutoNightMode.System;
            FontName = "Times New Roman";
            IsSendWithEnter = true;
            Wallpaper = new ChatWallpaper();
            PossibleWallpapers = new List<string>()
            {
                "Monkey.jpg",
                "Pineapple.jpg",
                "Snowman.jpg"
            };
        }

        public string GetWallpaperName()
        {
            return Wallpaper.WallpaperName;
        }

        public bool IsWallpaperBlurred()
        {
            return Wallpaper.IsBlurred;
        }
    }
}
