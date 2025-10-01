using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
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
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages.PasscodePages
{
    /// <summary>
    /// Логика взаимодействия для PasscodePage.xaml
    /// </summary>
    public partial class PasscodePage : Page
    {
        private TelSystem _system;
        public PasscodePage(TelSystem system)
        {
            _system = system;

            InitializeComponent();

            SetTimeInBlock();
        }

        private void BackGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            BackBut.Foreground = new SolidColorBrush(Colors.White);
            Cursor = Cursors.Hand;
        }

        private void BackGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            BackBut.Foreground = new SolidColorBrush(Colors.Gray);
            Cursor = null;
        }

        private void BackGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this))
                .ClearTempPageFrame(this);

            ((MainWindow)Window.GetWindow(this))
                .SetSecondaryFrame(new PrivacyAndSecurity(_system));
        }

        private void CloseGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this))
                .ClearTempPageFrame(this);
        }

        private void CloseGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = new SolidColorBrush(Colors.White);
            Cursor = Cursors.Hand;
        }

        private void CloseGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = new SolidColorBrush(Colors.Gray);
            Cursor = null;
        }

        public void Repaint_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not Grid grid) return;

            grid.Background =
                (SolidColorBrush)Application.Current
                .Resources["DarkThemeProfileButEnter"];

            Cursor = Cursors.Hand;
        }

        public void Repaint_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not Grid grid) return;

            grid.Background = new SolidColorBrush(Colors.Transparent);

            Cursor = null;
        }

        private void AutoLockTimeGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AutoLock toLock = new AutoLock(_system);

            toLock.UpdateTime += () =>
            {
                SetTimeInBlock();
                ((MainWindow)Window.GetWindow(this)).SetTimer();

                ApiService.UpdatePasscode(_system.Settings.PrivacySettings.PassCode);
            };
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(toLock);
        }

        private void ChangePasscodeGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _system.Settings.PrivacySettings.PassCode = null;
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SetLocalCode(_system));
        }

        public void SetTimeInBlock()
        {
            ChosenTime.Text = string.Empty;
            int minutes = _system.Settings.PrivacySettings.PassCode.MinutesTimer;
            //if hours 
            if(minutes > 60)
            {
                int hours = minutes / 60;
                ChosenTime.Text += $"{hours} : ";
                minutes -= hours * 60;
            }
            //if minutes
            ChosenTime.Text += $"{minutes}"; 
        }

        private void DisableCodeGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTimer();
            _system.Settings.PrivacySettings.PassCode.MinutesTimer = -1;
           

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SetLocalCode(_system));
        }

        private void WIndowHelloToggle_Checked(object sender, RoutedEventArgs e)
        {
            _system.Settings.PrivacySettings.PassCode.IsWinUnLock = true;
        }

        private void WIndowHelloToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _system.Settings.PrivacySettings.PassCode.IsWinUnLock = false;
        }
    }
}
