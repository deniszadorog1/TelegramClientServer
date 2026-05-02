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
using System.Net.Http;

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
            if (sv == null) yield break;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                var container = listBox.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (container == null) continue;

                if (listBox.Items[i] is ListBoxItem item)
                {
                    Console.WriteLine(item.Content);
                }

                if (IsElementVisibleInContainer(container, sv))
                {
                    yield return listBox.Items[i];
                }
            }
        }

        private static bool IsElementVisibleInContainer(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible) return false;

            Rect bounds = element.TransformToAncestor(container)
                                 .TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));

            Rect viewport = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return viewport.IntersectsWith(bounds);
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

        public static object GlobalFFMpegConfig { get; private set; }

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
            await SignalRService.UpdateCachedSettings(system.LoggedUser.Id);

            await Task.Delay(150);

            await SignalRService.UpdateLittlePhotoVisInChat(system.LoggedUser);

            await SignalRService.UpdatePagePhoto(system.LoggedUser);

            await SignalRService.UpdateContactPhotoVis(system.LoggedUser);

            await SignalRService.SetPhoneNumVisByExps(system.LoggedUser);

            await SignalRService.UpdateBirtDate(system.LoggedUser);

            await Task.Delay(150);

            await SignalRService.SetContactLastSeenVisState(system.LoggedUser);

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

        /*                //Upload medias
                        for (int i = 0; i < names.Length; i++)
                        {
                            
                        }*/

        public static async Task<Image> GetFirstFrameAsync(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            string videoPath = FilesAction.GetPathByName(fileName);
            if (videoPath is null || videoPath == string.Empty) return null;

            if (!videoPath.StartsWith("http") && !File.Exists(videoPath))
                throw new FileNotFoundException("Video not Found", videoPath);


            var mediaPlayer = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };
            mediaPlayer.Open(new Uri(videoPath, UriKind.RelativeOrAbsolute));
            mediaPlayer.Position = TimeSpan.FromSeconds(0);
            System.Threading.Thread.Sleep(500);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawVideo(mediaPlayer, new Rect(0, 0, 320, 240)); // Размер превью
            }

            var rtb = new RenderTargetBitmap(320, 240, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                mediaPlayer.Close();
            }

            string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");


            using (var fileStream = new FileStream(tempPath, FileMode.Create))
            {
                BitmapEncoder temp = new PngBitmapEncoder();
                temp.Frames.Add(BitmapFrame.Create(bitmapImage));
                temp.Save(fileStream);
            }

            string path = await ApiService.UploadMediaAsync(tempPath);
            path = FilesAction.GetPathByPseudoPath(path);

            ApiService.AddCashParams(path, bitmapImage);

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            };

            return new Image() { Source = bitmapImage };



            


            /*            



            string tempImage = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
            string tempVideoFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".mp4");

            using (var client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(videoPath);
                await File.WriteAllBytesAsync(tempVideoFile, bytes);
            }



            await FFMpeg.SnapshotAsync(tempVideoFile, tempImage, null, TimeSpan.FromSeconds(0));

            tempImage = await ApiService.UploadMediaAsync(tempImage);
            tempImage = FilesAction.GetPathByPseudoPath(tempImage);

            File.Delete(tempVideoFile);

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
            };*/
        }
    }
}
