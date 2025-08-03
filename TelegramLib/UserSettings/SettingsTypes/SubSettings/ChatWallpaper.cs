using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TestThing;

namespace TelegramLib.UserSettings.SettingsTypes.SubSettings
{
    public class ChatWallpaper
    {
        public int Id { get; set; }
        public string WallpaperName { get; set; } //Set wallpaper settings or if should only file name here
        public bool IsBlurred { get; set; }

        public ChatWallpaper(int id, string wallpaperPath, bool isBlurred)
        {
            Id = id;
            WallpaperName = wallpaperPath;
            IsBlurred = isBlurred;
        }

        public ChatWallpaper()
        {
            Id = -1;
            WallpaperName = "fray.jpg"; // GetTestParams.GetTestFryImagePath();
            IsBlurred = true;
        }

        public void SetBlurParam(bool isBlur)
        {
            IsBlurred = isBlur;
        }

        public void SetWallpaperPath(string path)
        {
            WallpaperName = path;
        }
    }
}
