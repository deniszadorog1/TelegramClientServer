using MaterialDesignThemes.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.Enums.Messages;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Enums.MediaShow;
using TelegramVisualPart.Services;
using Path = System.IO.Path;

namespace TelegramVisualPart.Helper
{
    public static class FilesAction
    {
        private static readonly List<string> _imgsExt = new List<string>()
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp",
            ".webp"
        };

        private static readonly List<string> _gifExt = new List<string>()
        {
            ".gif"
        };


        private static readonly List<string> _videoExt = new List<string>()
        {
            ".mp4",
            ".avi",
            ".mov",
            ".webm",
            ".mkv",
            ".wmv"
        };

        private const string _baseImgName = "Minato";

        public static MediaType GetMediaTypeFromFilename(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return MediaType.Unknown;

            string extension = Path.GetExtension(path).ToLowerInvariant();

            switch (extension)
            {
                case var _ when _imgsExt.Contains(extension):
/*                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".webp":*/
                    {
                        return MediaType.Image;
                    }
                case var _ when _gifExt.Contains(extension):
                //case ".gif":
                    {
                        return MediaType.Gif;
                    }
                case var _ when _videoExt.Contains(extension):
/*                case ".mp4":
                case ".avi":
                case ".mov":
                case ".webm":
                case ".mkv":
                case ".wmv":*/
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
            var pseudoPath = Task.Run(async () => await ApiService.GetPathOnMediaServer(fileName)).Result;

            if (pseudoPath is not null)
            {
                string res = /*MediaServerUrl.Url +*/ pseudoPath;
                return res;
            }

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
            /*            DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                        string parentPath = baseDirectoryInfo.Parent.Parent.Parent.Parent.FullName;
                        string tempPath = Path.Combine(parentPath, "TelegramVisualPart");
                        return Path.Combine(tempPath, "Visuals");*/

            return Path.Combine(AppContext.BaseDirectory, "Visuals");
        }

        public static string GetImagesPath()
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
                foreach (string name in resNames)
                {
                    if (!names.Contains(name)) toRemove.Add(name);
                }

                for (int i = 0; i < toRemove.Count; i++)
                {
                    resNames.Remove(toRemove[i]);
                }

            }
            return resNames;
        }


        public static async Task<Image> GetUserImage(string path)
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri(await GetUserImagePath(path), UriKind.Absolute)),
                Stretch = Stretch.Fill
            };
        }

        public static async Task<string> GetUserImagePath(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            if (!fileName.Contains(_baseImgName))
            {
                //MessageBox.Show("Getting pseudo path");
                var pseudoPath = await ApiService.GetPathOnMediaServer(fileName);
                string res = pseudoPath.Contains(MediaServerUrl.Url) ? pseudoPath : MediaServerUrl.Url + pseudoPath;

                //MessageBox.Show(res);               
                return res;
            }
            //Get from images 

            string userImage = Path.Combine(GetImagesPath(), "UserImages");
            return Path.Combine(userImage, fileName);
        }

        public static string GetPathByPseudoPath(string pseudoPath)
        {
            if (pseudoPath is null || pseudoPath.Contains(_baseImgName))
            {
                return string.Empty;
            }
            string res = pseudoPath.Contains(MediaServerUrl.Url) ? pseudoPath : MediaServerUrl.Url + pseudoPath;
            return res;
        }

        public static string GetPathByName(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            var pseudoPath = Task.Run(async () => await ApiService.GetPathOnMediaServer(fileName)).Result;

            if (pseudoPath is null) return null;
            return GetPathByPseudoPath(pseudoPath);
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
            string name = Path.GetFileName(fileName);
            string path = FilesAction.GetPathByName(fileName);
            if (path is null) return null;


            //string path = Path.Combine(GetChatImageFolderPath(), fileName);

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

        public static async Task<string> GetFilePathByMediaType(MediaType type, string name)
        {
            switch (type)
            {
                case MediaType.Unknown:
                    {
                        return string.Empty;
                    }
                case MediaType.Image:
                    {
                        return await ApiService.GetPathOnMediaServer(name);
                        return Path.Combine(GetChatImageFolderPath(), name);
                    }
                case MediaType.Gif:
                    {
                        return await ApiService.GetPathOnMediaServer(name);
                        return Path.Combine(GetGifsPath(), name);
                    }
                case MediaType.Video:
                    {
                        return await ApiService.GetPathOnMediaServer(name);
                        return Path.Combine(GetVideosPath(), name);
                    }
                case MediaType.Sticker:
                    {
                        //return await ApiService.GetPathOnMediaServer(name);
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

        public static async Task<Image> GetImagePreviewForVideo(string videoName)
        {
            Size baseVideoSize = new Size(320, 240);
            Size imgSize = new Size(225, 205);
            const int dpi = 96;
            const int delay = 7000;
            const int baseDelay = 300;
            const double timeSpan = 0.5;

            var pseudoPath = await ApiService.GetVideoPreviewPath(videoName);
            if (string.IsNullOrEmpty(pseudoPath)) return new Image();

            string fullUrl = FilesAction.GetPathByPseudoPath(pseudoPath);

            try
            {
                var mediaPlayer = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };

                var tcs = new TaskCompletionSource<bool>();

                mediaPlayer.MediaOpened += (s, e) => tcs.TrySetResult(true);
                mediaPlayer.MediaFailed += (s, e) => tcs.TrySetResult(false);

                // Открываем URL видео
                mediaPlayer.Open(new Uri(fullUrl, UriKind.Absolute));

                var delayTask = Task.Delay(delay);
                var completedTask = await Task.WhenAny(tcs.Task, delayTask);

                if (completedTask == delayTask || !await tcs.Task)
                {
                    mediaPlayer.Close();
                    return new Image(); 
                }

                mediaPlayer.Position = TimeSpan.FromSeconds(timeSpan);

                await Task.Delay(baseDelay);

                double width = mediaPlayer.NaturalVideoWidth > 0 ? mediaPlayer.NaturalVideoWidth : baseVideoSize.Width;// 320;
                double height = mediaPlayer.NaturalVideoHeight > 0 ? mediaPlayer.NaturalVideoHeight : baseVideoSize.Height;// 240;

                var drawingVisual = new DrawingVisual();
                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawVideo(mediaPlayer, new Rect(0, 0, width, height));
                }

                var renderTargetBitmap = new RenderTargetBitmap(
                    (int)width, (int)height,
                    dpi, dpi, PixelFormats.Pbgra32);

                renderTargetBitmap.Render(drawingVisual);

                if (renderTargetBitmap.CanFreeze)
                    renderTargetBitmap.Freeze(); 

                mediaPlayer.Close();

                return new Image { Source = renderTargetBitmap, Height = imgSize.Height, Width = imgSize.Width };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Mistake in img generation: {ex.Message}");
                return new Image();
            }
        }

        public static MediaElement GetMediaElementByVideoName(string name)
        {
            Size medisSize = new Size(300, 200);
            //string videoPath = Path.Combine(GetVideosPath(), name);
            string videoPath = GetPathByName(name);

            MediaElement res = new MediaElement()
            {
                Source = new Uri(videoPath, UriKind.Absolute),
                Width = medisSize.Width,
                Height = medisSize.Height,
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

        public static BitmapImage ToBitmapImage(BitmapSource bitmapSource)
        {
            if (bitmapSource is BitmapImage bitmapImage)
            {
                return bitmapImage;
            }

            using (var ms = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(ms);

                ms.Seek(0, SeekOrigin.Begin);

                var result = new BitmapImage();
                result.BeginInit();
                result.StreamSource = ms;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.EndInit();
                result.Freeze();

                return result;
            }
        }

        public static bool IsRealMedia(string path)
        {
            try
            {
                byte[] buffer = new byte[12];
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(buffer, 0, buffer.Length);
                }

                // PNG: 89 50 4E 47
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47) return true;

                // JPEG: FF D8 FF
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF) return true;

                // GIF: 47 49 46 (GIF87a / GIF89a)
                if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46) return true;

                // MP4 (и другие контейнеры): Байты 'ftyp' на позициях 4-7 (66 74 79 70)
                if (buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70) return true;

                // WEBP: 'RIFF' на позициях 0-3 (52 49 46 46) И 'WEBP' на позициях 8-11 (57 45 42 50)
                if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 && // RIFF
                    buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50)  // WEBP
                {
                    return true;
                }

                return false;
            }
            catch { return false; }
        }
    }
}
