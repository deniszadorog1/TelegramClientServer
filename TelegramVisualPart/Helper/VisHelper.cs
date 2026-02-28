using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using TelegramVisualPart.Services;
using TelegramLib.MainClasses;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.IO;
using FFMpegCore;

namespace TelegramVisualPart.Helper
{
    public static class VisHelper
    {
        private static MediaPlayer _player = new MediaPlayer();

        public static void PlaySound(string path, double volume)
        {
            StopSound();
            _player.Open(new Uri(path, UriKind.Absolute));

            _player.Volume = volume;
            _player.Play();
        }

        public static void StopSound()
        {
            _player.Stop();
        }

        //Get controls which are seen in list box (vertical scroll stuff)
        public static IEnumerable<object> GetVisibleItems(ListBox listBox)
        {
            var sv = GetScrollViewer(listBox);
            if (sv == null)
                yield break;

            VirtualizingStackPanel vsp = FindVisualChild<VirtualizingStackPanel>(listBox);
            if (vsp == null)
                yield break;

            int firstVisibleIndex = (int)Math.Floor(vsp.VerticalOffset);
            int lastVisibleIndex = (int)Math.Ceiling(vsp.VerticalOffset + vsp.ViewportHeight);

            for (int i = firstVisibleIndex; i <= lastVisibleIndex && i < listBox.Items.Count; i++)
            {
                yield return listBox.Items[i];
            }
        }

        private static ScrollViewer GetScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer sv)
                return sv;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                return Application.Current.Dispatcher.Invoke(() => FindVisualChild<T>(parent));
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private const int _correctTimeLength = 2;
        public static string GetCorrectTimeParamVis(string timeParam)
        {
            return timeParam.Count() == _correctTimeLength ?
                timeParam : timeParam.Insert(0, "0");
        }

        public static bool IsLink(string text)
        {
            return Uri.TryCreate(text, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        public static async Task UpdateStatesWithSignalR(TelSystem system)
        {
            await SignalRService.SetPhoneNumVisByExps(system.LoggedUser);

            await SignalRService.UpdateBirtDate(system.LoggedUser);

            await SignalRService.SetContactLastSeenVisState(system.LoggedUser);

            await SignalRService.UpdateContactPhotoVis(system.LoggedUser);

            await SignalRService.UpdateContactForwardStatus(system.LoggedUser);

            await SignalRService.UpdateContactBioVis(system.LoggedUser);

            await SignalRService.UpdateOnlineStatus(system.LoggedUser);
        }

        public static string CleanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim();
            input = Regex.Replace(input, @"\s{2,}", " ");
            return input;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);
        }
       
        public static async Task<Image> GetFirstFrameAsync(string fileName)
        {
            string videoPath = FilesAction.GetFullVideoPath(Path.GetFileName(fileName));
            if (!File.Exists(videoPath))
                throw new FileNotFoundException("Видео не найдено", videoPath);

            string tempImage = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");

            try
            {
                // Берём кадр
                await FFMpeg.SnapshotAsync(videoPath, tempImage, null, TimeSpan.FromSeconds(0));

                if (!File.Exists(tempImage))
                    throw new Exception("FFMpeg не создал скриншот");
            }
            catch (Exception ex)
            {
                throw new Exception("FFMpeg не смог создать скриншот: " + ex.Message, ex);
            }

            BitmapImage bitmap;

            using (var stream = new FileStream(tempImage, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            File.Delete(tempImage);

            return new Image
            {
                Source = bitmap,
                Stretch = Stretch.UniformToFill
            };
        }
    }
}
