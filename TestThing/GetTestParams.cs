using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestThing
{
    public static class GetTestParams
    {
        public static string GetTestFryImagePath()
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
            string tempPath = Path.Combine(parentPath, "TelegramVisualPart");
            string visPath = Path.Combine(tempPath, "Visuals");
            string imagesPath = Path.Combine(visPath, "Images");
            string userImagesPath = Path.Combine(imagesPath, "UserImages");
            string fryPath = Path.Combine(userImagesPath, "fray.jpg");

            return fryPath;
        }

        public static string GetWallpaperPath(string wallpaperName)
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
            string tempPath = Path.Combine(parentPath, "TelegramVisualPart");
            string visPath = Path.Combine(tempPath, "Visuals");
            string imagesPath = Path.Combine(visPath, "Images");
            string wallpaperPath = Path.Combine(imagesPath, "Wallpapers");
            string resPath = Path.Combine(wallpaperPath, wallpaperName);

            return resPath;
        }

        
        
    }
}
