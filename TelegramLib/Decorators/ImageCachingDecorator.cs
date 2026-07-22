using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TelegramLib.Interfaces;
using TelegramLib.UserSettings;


namespace TelegramLib.Decorators
{
    public class ImageCachingDecorator : IApiService
    {
        private readonly IApiService _innerService;

        private readonly Dictionary<string, (BitmapImage Bitmap, string Path)> _imageCache = new();
        private readonly List<MainSettings> _cachedSettings = new List<MainSettings>();
        private readonly List<TelegramLib.MainClasses.User> _cachedUsers = new List<MainClasses.User>();

        public ImageCachingDecorator(IApiService innerService)
        {
            _innerService = innerService;
        }

        public void AddCashedParams(string fullPath, BitmapImage bitmap)
        {
            string name = Path.GetFileName(fullPath);
            _imageCache[name] = (bitmap, fullPath);
        }

        public void AddPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return;

            string name = Path.GetFileName(fullPath);
            _imageCache[name] = (null, fullPath);
        }

        public void AddBitMap(string name, BitmapImage bitmap)
        {
            string fileName = Path.GetFileName(name);

            if (_imageCache.ContainsKey(fileName))
            {
                var current = _imageCache[fileName];
                _imageCache[fileName] = (bitmap, current.Path);
            }
            else
            {
                _imageCache[fileName] = (bitmap, null);
            }
        }

        public string GetPath(string mediaName)
        {
            string name = Path.GetFileName(mediaName);

            if (_imageCache.TryGetValue(name, out var value))
            {
                return value.Path;
            }
            return null;
        }

        public BitmapImage GetBitmap(string mediaName)
        {
            string name = Path.GetFileName(mediaName);

            if (_imageCache.TryGetValue(name, out var value))
            {
                return value.Bitmap;
            }
            return null;
        }


        public void SetSettings(MainSettings settings)
        {
            if (settings is null) return;

            int index = _cachedSettings.FindIndex(x => x.Id == settings.Id);
            if (index != -1)
            {
                _cachedSettings[index] = settings;
                return;
            }
            _cachedSettings.Add(settings);
        }

        public MainSettings GetSettings(int id)
        {
            return _cachedSettings.FirstOrDefault(x => x.Id == id);
        }

        public void SetUser(TelegramLib.MainClasses.User user)
        {
            if (user is null) return;

            int index = _cachedUsers.FindIndex(x => x.Id == user.Id);
            if (index != -1)
            {
                _cachedUsers[index] = user;
                return;
            }
            _cachedUsers.Add(user);
        }

        public TelegramLib.MainClasses.User GetUser(int userId)
        {
            return _cachedUsers.FirstOrDefault(x => x.Id == userId);
        }


        private BitmapImage LoadBitmapFromFile(string path)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public Task<byte[]> GetFileBytesAsync(string fileName) => _innerService.GetFileBytesAsync(fileName);
    }
}
