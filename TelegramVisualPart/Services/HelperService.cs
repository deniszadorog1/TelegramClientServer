using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Controls;

namespace TelegramVisualPart.Services
{
    public static class HelperService
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }

        public static void SetOnlineStatusInTextBox(TextBlock block, bool isOnline, DateTime? lastSeenOnline)
        {
            if (isOnline)
            {
                block.Text = VisConstParamsJsonService.GetStringByName("OnlineStat");
                block.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
                return;
            }

            block.Foreground = new SolidColorBrush(Colors.Gray);
            block.Text = lastSeenOnline is null ? VisConstParamsJsonService.GetStringByName("RecentlyStat") :  
                $"{lastSeenOnline.Value.Day}.{lastSeenOnline.Value.Month}.{lastSeenOnline.Value.Year}";
        }
    }
}
