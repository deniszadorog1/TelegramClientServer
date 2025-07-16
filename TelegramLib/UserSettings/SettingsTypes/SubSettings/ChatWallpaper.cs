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
        public int Id { get; set; }
        public string WallpaperPath { get; set; } //Set wallpaper settings
        public bool IsBlurred { get; set; }

        public ChatWallpaper(int id, string wallpaperPath, bool isBlurred)
        {
            Id = id;
            WallpaperPath = wallpaperPath;
            IsBlurred = isBlurred;
        }

        public ChatWallpaper()
        {
            Id = -1;
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
