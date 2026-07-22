using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;

namespace TelegramVisualPart.Services
{
    public static class SaveElements
    {
        public static void SaveImageAs(Image img)
        {
            if (img.Source is BitmapImage bitmapImage)
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg",
                    FileName = "image.png"
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

            if (!File.Exists(absolutePath))
            {
                MessageBox.Show("File was not found:\n" + absolutePath);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save gif as...",
                Filter = "GIF files (*.gif)|*.gif",
                FileName = Path.GetFileName(absolutePath)
            };

            if (dialog.ShowDialog() == true)
            {
                File.Copy(absolutePath, dialog.FileName, overwrite: true);
            }
        }

        public static void SaveVideoAs(MediaElement element)
        {
            string originalPath = element.Source.LocalPath; 
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save Video as...",
                Filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*",
                FileName = Path.GetFileName(originalPath)
            };

            if (dialog.ShowDialog() == true)
            {
                File.Copy(originalPath, dialog.FileName, overwrite: true);
            }
        }
    }
}
