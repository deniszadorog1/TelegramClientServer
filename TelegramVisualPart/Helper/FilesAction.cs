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
using MaterialDesignThemes.Wpf;
using System.Configuration;
using System.Windows.Automation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        public static bool IsFileIsImage(string fileName)
        {
            return GetMediaTypeFromFilename(fileName) == MediaType.Image;
        }

        private static string GetVisualPath()
        {
            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
            string tempPath = Path.Combine(parentPath, "TelegramVisualPart");
            return Path.Combine(tempPath, "Visuals");
        }

        private static string GetImagesPath()
        {
            return Path.Combine(GetVisualPath(), "Images");
        }

        public static string GetStickerPathObjByName(string fileName)
        {
            string stickerPath = Path.Combine(GetImagesPath(), "Stickers");
            return Path.Combine(stickerPath, fileName);
        }

        public static string GetWallpaperPathByName(string fileName)
        {
            string wallPaperPath = Path.Combine(GetImagesPath(), "Wallpapers");
            return Path.Combine(wallPaperPath, fileName);
        }

        public static string GetUserImagePath(string fileName)
        {
            string userImage = Path.Combine(GetImagesPath(), "UserImages");
            return Path.Combine(userImage, fileName);
        }

        public static string GetGifsPath()
        {
            return Path.Combine(GetVisualPath(), "Gifs");
        }

        public static string GetChatImageFolderPath()
        {
            return Path.Combine(GetImagesPath(), "ChatImages");
        }

        public static string GetVideosPath()
        {
            return Path.Combine(GetVisualPath(), "Videos");
        }

        public static string GetStickerPath()
        {
            return Path.Combine(GetImagesPath(), "Stickers");
        }


        /// <summary>
        /// Is media file is exist in specific folder (chat image folder) 
        /// </summary>
        /// <param name="mediaName"></param>
        /// <returns></returns>
        public static bool IsUserChatMediaIsExist(string mediaName)
        {
            string chatImagePath = GetChatImageFolderPath();
            return File.Exists(Path.Combine(chatImagePath, mediaName));
        }

        public static void CopyImageToImageFolder(string filePath)
        {
            string copyToDirectory = GetChatImageFolderPath();

            string fileName = Path.GetFileName(filePath);
            string destinationPath = Path.Combine(copyToDirectory, fileName);

            File.Copy(filePath, destinationPath, overwrite: true);
        }

        public static Image GetImageFromChatImageFolder(string fileName)
        {
            string path = Path.Combine(GetChatImageFolderPath(), fileName);

            return new Image()
            {
                Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute)),
                Stretch = Stretch.Fill
            };
        }

        public static bool IsVideoIsExistInSecFolder(string videoName)
        {
            string vidFolderPath = GetVideosPath();
            return File.Exists(Path.Combine(vidFolderPath, videoName));
        }

        public static void CopyVideoToVideoFolder(string filePath)
        {
            string copyToDirectory = GetVideosPath();

            string fileName = Path.GetFileName(filePath);
            string destinationPath = Path.Combine(copyToDirectory, fileName);

            File.Copy(filePath, destinationPath, overwrite: true);
        }


        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }

        public static FrameworkElement FindParentByName(DependencyObject child, string name)
        {
            while (child != null)
            {
                if (child is FrameworkElement fe && fe.Name == name)
                    return fe;

                child = VisualTreeHelper.GetParent(child);
            }

            return null;
        }

        public static PackIconKind GetIconTypeByString(string iconName)
        {
            if (Enum.TryParse<PackIconKind>(iconName, out var kind))
            {
                return kind;
            }
            return PackIconKind.Folder;
        }

        public static string GetFilePathByMediaType(MediaType type, string name)
        {
            switch (type)
            {
                case MediaType.Unknown:
                    {
                        return string.Empty;
                    }
                case MediaType.Image:
                    {
                        return Path.Combine(GetChatImageFolderPath(), name);
                    }
                case MediaType.Gif:
                    {
                        return Path.Combine(GetGifsPath(), name);
                    }
                case MediaType.Video:
                    {
                        return Path.Combine(GetVideosPath(), name);
                    }
                case MediaType.Sticker:
                    {
                        return Path.Combine(GetStickerPath(), name);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }

        }
    }
}
