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

        // Рекурсивный поиск дочернего элемента в визуальном дереве
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

    }
}
