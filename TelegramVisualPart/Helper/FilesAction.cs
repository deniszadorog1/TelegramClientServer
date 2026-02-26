using FFMpegCore;
using FFMpegCore.Pipes;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Enums.MediaShow;
using Path = System.IO.Path;

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

        public static int GetImagesFromMediaAction(List<MediaAction> medias)
        {
            int res = 0;
            for (int i = 0; i < medias.Count; i++)
            {
                if (IsFileIsImage(medias[i].MediaName)) res++;
            }
            return res;
        }

        public static int GetVideosAmount(List<MediaAction> medias)
        {
            int res = 0;
            for (int i = 0; i < medias.Count; i++)
            {
                if (IsFileIsVideo(medias[i].MediaName)) res++;
            }
            return res;
        }

        public static int GetGifsAmount(List<MediaAction> medias)
        {
            int res = 0;
            for (int i = 0; i < medias.Count; i++)
            {
                if (IsFileIsGif(medias[i].MediaName)) res++;
            }
            return res;
        }


        public static List<string> GetFullGifPaths(List<string> gifNames)
        {
            List<string> res = new List<string>();

            for (int i = 0; i < gifNames.Count; i++)
            {
                res.Add(GetFullGifPath(gifNames[i]));
            }

            return res;
        }

        public static string GetFullGifPath(string gifName)
        {
            string baseName = Path.GetFileName(gifName);
            return Path.Combine(GetGifsPath(), baseName);
        }

        public static string GetFullUserImagePath(string imgName)
        {
            string userImgsFolderPath = Path.Combine(GetImagesPath(), "UserImages");
            return Path.Combine(userImgsFolderPath, imgName);
        }

        public static List<string> GetFullPathForVideos(List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                paths[i] = GetFullVideoPath(paths[i]);
            }
            return paths;
        }

        public static string GetFullVideoPath(string fileName)
        {
            return Path.Combine(GetVideosPath(), fileName);
        }

        public static List<MediaAction> GetMediaElementsFromListByType(List<MediaAction> medias, MediaType type)
        {
            List<MediaAction> toRemove = new List<MediaAction>();

            for (int i = 0; i < medias.Count; i++)
            {
                if (GetMediaTypeFromFilename(medias[i].MediaName) != type)
                {
                    toRemove.Add(medias[i]);
                }
            }

            foreach (MediaAction action in toRemove)
            {
                medias.Remove(action);
            }

            return medias;
        }

        public static bool IsFileIsGif(string fileName)
        {
            return GetMediaTypeFromFilename(fileName) == MediaType.Gif;
        }


        public static bool IsFileIsVideo(string fileName)
        {
            return GetMediaTypeFromFilename(fileName) == MediaType.Video;
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

        public static List<string?> GetAllWallpaperNames(List<string> names)
        {
            List<string?> resNames = new List<string?>();
            string wallPaperPath = Path.Combine(GetImagesPath(), "Wallpapers");

            if (Directory.Exists(wallPaperPath))
            {
                var files = Directory.GetFiles(wallPaperPath);
                resNames = files.Select(Path.GetFileName).ToList();

                List<string> toRemove = new List<string>();
                foreach(string name in resNames)
                {
                    if (!names.Contains(name)) toRemove.Add(name);
                }

                for(int i = 0; i < toRemove.Count; i++)
                {
                    resNames.Remove(toRemove[i]);
                }

            }
            return resNames;
        }

        public static List<Image> GetUserImages(List<string> names)
        {
            List<Image> res = new List<Image>();

            for (int i = 0; i < names.Count; i++)
            {
                res.Add(GetUserImage(names[i]));
            }
            return res;
        }

        public static Image GetUserImage(string path)
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri(GetUserImagePath(path), UriKind.Absolute)),
                Stretch = Stretch.Fill
            };
        }

        public static string GetUserImagePath(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            string userImage = Path.Combine(GetImagesPath(), "UserImages");
            return Path.Combine(userImage, fileName);
        }

        public static string GetSystemImagePath(string fileName)
        {
            fileName = Path.GetFileName(fileName);


            string sysImage = Path.Combine(GetImagesPath(), "SystemImages");
            return Path.Combine(sysImage, fileName);
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

        public static bool IsGifNameIsExist(string fileName)
        {
            return File.Exists(Path.Combine(GetGifsPath(), fileName));
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

        public static void AddNewUserImage(string path)
        {
            string userImagePath = Path.Combine(GetImagesPath(), "UserImages");

            CopyFileToAnotherFolder(path, userImagePath);
        }

        public static void AddNewNotifSound(string path)
        {
            string notifPath = Path.Combine(GetSoundsPath(), "NotifSounds");

            CopyFileToAnotherFolder(path, notifPath);
            /*            string fileName = Path.GetFileName(path);
                        string destinationPath = Path.Combine(notifPath, fileName);
                        if (Path.Exists(destinationPath)) return;
                        File.Copy(path, destinationPath, overwrite: true);*/
        }

        public static string GetSoundPath(string fileName)
        {
            string notifPath = Path.Combine(GetSoundsPath(), "NotifSounds");

            return Path.Combine(notifPath, fileName);
        }

        private static void CopyFileToAnotherFolder(string path, string destFolderPath)
        {
            string fileName = Path.GetFileName(path);
            string destinationPath = Path.Combine(destFolderPath, fileName);

            if (Path.Exists(destinationPath)) return;

            File.Copy(path, destinationPath, overwrite: true);
        }

        private static string GetSoundsPath()
        {
            return Path.Combine(GetVisualPath(), "Sounds");
        }

        public static void AddNewWallpaper(string path)
        {
            string wallPaperPath = Path.Combine(GetImagesPath(), "Wallpapers");

            string fileName = Path.GetFileName(path);
            string destinationPath = Path.Combine(wallPaperPath, fileName);

            if (Path.Exists(destinationPath)) return;

            File.Copy(path, destinationPath, overwrite: true);
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

        public static string GetFullChatImagePath(string fileName)
        {
            string chatImages = GetChatImageFolderPath();
            return Path.Combine(chatImages, fileName);
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

        /*        public static string GetFullVideoPath(string vidName)
                {
                    return Path.Combine(GetVideosPath(), name);
                }*/

        public static Image GetImagePreviewForVideo(string videoName)
        {
            GlobalFFOptions.Configure(new FFOptions
            {
                BinaryFolder = Path.Combine(AppContext.BaseDirectory, "ffmpeg"),
                TemporaryFilesFolder = Path.GetTempPath()
            });


            string videoPath = Path.Combine(GetVideosPath(), videoName);

            using var ms = new MemoryStream();

            FFMpegArguments
                .FromFileInput(videoPath)
                .OutputToPipe(new StreamPipeSink(ms), options => options
                    .WithVideoCodec("png")
                    .WithFrameOutputCount(1)
                    .ForceFormat("image2pipe"))
                .ProcessSynchronously();


            ms.Seek(0, SeekOrigin.Begin);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            bitmap.Freeze();

            return new Image { Source = bitmap };
        }

        public static MediaElement GetMediaElementByVideoName(string name)
        {
            string videoPath = Path.Combine(GetVideosPath(), name);

            MediaElement res = new MediaElement()
            {
                Source = new Uri(videoPath, UriKind.Absolute),
                Width = 300,
                Height = 200,
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };
            res.Play();

            return res;
        }

        public static BitmapSource GetFirstImageFromGif(string gifPath)
        {
            if (!File.Exists(gifPath)) return null;

            using (var stream = new FileStream(gifPath, FileMode.Open, FileAccess.Read))
            {
                var decoder = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);

                // Первый кадр всегда по индексу 0
                return decoder.Frames[0];
            }
        }
        public static string GetFullPath(string fileName, MediaShowType type)
        {
            if (fileName is null) return string.Empty;
            switch (type)
            {
                case MediaShowType.ChatImages:
                    {
                        return FilesAction.GetFullChatImagePath(fileName);
                    }
                case MediaShowType.UserImages:
                    {
                        return FilesAction.GetFullUserImagePath(fileName);
                    }
                case MediaShowType.Videos:
                    {
                        return FilesAction.GetFullVideoPath(fileName);
                    }
                case MediaShowType.OtherUserImages:
                    {
                        return FilesAction.GetFullChatImagePath(fileName);
                    }
                case MediaShowType.Gif:
                    {
                        return FilesAction.GetFullGifPath(fileName);
                    }
                default:
                    {
                        return string.Empty;
                    }
            }
        }
    }
}
