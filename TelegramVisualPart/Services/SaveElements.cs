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
    }
}
