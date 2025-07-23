using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses.Messages;

namespace TelegramVisualPart.Helper
{
    public static class FilesAction
    {
        public static MediaType GetMediaTypeFromFilename(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return MediaType.Unknown;

            string extension = Path.GetExtension(path).ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".webp":
                    {
                        return MediaType.Image;
                    }
                case ".gif":
                    {
                        return MediaType.Gif;
                    }
                case ".mp4":
                case ".avi":
                case ".mov":
                case ".webm":
                case ".mkv":
                case ".wmv":
                    {
                        return MediaType.Video;
                    }
                default:
                    {
                        return MediaType.Unknown;
                    }
            }
        }

        public static string GetWallpaperPathByName(string fileName)
        {
            // 'B:\GitHub\TelegramClientServer\TelegramVisualPart\Visuals\Images\UserImages\Wallpapers\Monkey.jpg'."
            
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
            string tempPath = Path.Combine(parentPath, "TelegramVisualPart");
            string visPath = Path.Combine(tempPath, "Visuals");
            string imgsPath = Path.Combine(visPath, "Images");
            string wallPaperPath = Path.Combine(imgsPath, "Wallpapers");
            string resPath = Path.Combine(wallPaperPath, fileName);

            return resPath;
        }

        public static string GetUserImagePath(string fileName)
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
            string tempPath = Path.Combine(parentPath, "TelegramVisualPart");
            string visPath = Path.Combine(tempPath, "Visuals");
            string imgsPath = Path.Combine(visPath, "Images");
            string userImage = Path.Combine(imgsPath, "UserImages");
            string resPath = Path.Combine(userImage, fileName);

            return resPath;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }
    }
}
