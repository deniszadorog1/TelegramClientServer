using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace TelegramVisualPart.Services
{
    public static class SaveElements
    {
        public static void SaveImageAs(Image img)
        {
            const string filter = "PNG Image|*.png|JPEG Image|*.jpg";
            const string fileName = "image.png";

            if (img.Source is BitmapImage bitmapImage)
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = filter,
                    FileName = fileName
                };

                if (dialog.ShowDialog() == true)
                {
                    BitmapEncoder encoder = dialog.FilterIndex == 2
                        ? new JpegBitmapEncoder()
                        : new PngBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                    using var fileStream = new FileStream(dialog.FileName, FileMode.Create);
                    encoder.Save(fileStream);
                }
            }
        }

        public static void SaveGifAs(string gifPath)
        {
            string relativePath = gifPath.Replace("pack://siteoforigin:,,,", "").TrimStart('/');

            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            const string title = "Save gif as...";
            const string filter = "GIF files (*.gif)|*.gif";

            if (!File.Exists(absolutePath))
            {
                MessageBox.Show("File was not found:\n" + absolutePath);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = title,
                Filter = filter,
                FileName = Path.GetFileName(absolutePath)
            };

            if (dialog.ShowDialog() == true)
            {
                File.Copy(absolutePath, dialog.FileName, overwrite: true);
            }
        }

        public static void SaveVideoAs(MediaElement element)
        {
            const string title = "Save Video as...";
            const string filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*";

            string originalPath = element.Source.LocalPath; 
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = title,
                Filter = filter,
                FileName = Path.GetFileName(originalPath)
            };

            if (dialog.ShowDialog() == true)
            {
                File.Copy(originalPath, dialog.FileName, overwrite: true);
            }
        }
    }
}
