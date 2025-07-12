using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestThing;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings
{
    public class ChatWallpaper
    {
        public string WallpaperPath { get; set; } //Set wallpaper settings
        public bool IsBlurred { get; set; }

        public ChatWallpaper(string wallpaperPath, bool isBlurred)
        {
            WallpaperPath = wallpaperPath;
            IsBlurred = isBlurred;
        }

        public ChatWallpaper()
        {
            WallpaperPath = GetTestParams.GetTestFryImagePath();
            IsBlurred = true;
        }

        public void SetBlurParam(bool isBlur)
        {
            IsBlurred = isBlur;
        }

        public void SetWallpaperPath(string path)
        {
            WallpaperPath = path;
        }
    }
}
