using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TelegramLib.Interfaces;


namespace TelegramLib.Decorators
{
    public class ImageCachingDecorator : IApiService
    {
        private readonly IApiService _innerService;

        private readonly Dictionary<string, (BitmapImage Bitmap, string Path)> _imageCache = new();

        private readonly string _localCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TelegramCache");

        public ImageCachingDecorator(IApiService innerService)
        {
            _innerService = innerService;
            if (!Directory.Exists(_localCachePath)) Directory.CreateDirectory(_localCachePath);
        }

        public async Task<(BitmapImage, string)> GetImageAsync(string fileName)
        {
            if (_imageCache.TryGetValue(fileName, out var cachedData))
            {
                return cachedData;
            }

            string fullPath = Path.Combine(_localCachePath, fileName);
            if (File.Exists(fullPath))
            {
                var bitmap = LoadBitmapFromFile(fullPath);
                _imageCache[fileName] = (bitmap, fullPath);
                return (bitmap, fullPath);
            }

            byte[] data = await _innerService.GetFileBytesAsync(fileName);
            if (data == null) return (null, null);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                byte[] bytes = data;
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }

            //await File.WriteAllBytesAsync(fullPath, data);

            var newBitmap = LoadBitmapFromFile(fullPath);
            _imageCache[fileName] = (newBitmap, fullPath);

            return (newBitmap, fullPath);
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
