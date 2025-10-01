using FFMpegCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.MainClasses;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages.PasscodePages
{
    /// <summary>
    /// Логика взаимодействия для AutoLock.xaml
    /// </summary>
    public partial class AutoLock : Page
    {
        private TelSystem _system;
        public event Action UpdateTime;
        public AutoLock(TelSystem system)
        {
            _system = system;
            InitializeComponent();
        }

        public void ChangeCursor_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        public void ChangeCursor_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void SetTime_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int minutes = 0;
            if (sender == OneMin) minutes = 1;
            if (sender == FiveMin) minutes = 5;
            if (sender == OneHour) minutes = 60;
            if (sender == FiveHours) minutes = 300;

            SetNewPassCode(minutes);

            UpdateTime?.Invoke();
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void Custom_Checked(object sender, RoutedEventArgs e)
        {
            int.TryParse(HoursBox.Text, out int hours);
            int.TryParse(MinutesBox.Text, out int minutes);

            SetNewPassCode(hours * 60 + minutes);

            UpdateTime?.Invoke();
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        public void SetNewPassCode(int minutes)
        {
            if (_system.Settings.PrivacySettings.PassCode is null) return;
            _system.Settings.PrivacySettings.PassCode.MinutesTimer = 
                minutes;
        }

        private void CloseWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseWindow.Background =
                (SolidColorBrush)Application.Current
                .Resources["DarkThemeProfileButEnter"]; Cursor = Cursors.Hand;
            Cursor = Cursors.Hand;
        }

        private void CloseWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseWindow.Foreground = new SolidColorBrush(Colors.Transparent);
            Cursor = null;
        }

        private void CloseGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

    }
}
