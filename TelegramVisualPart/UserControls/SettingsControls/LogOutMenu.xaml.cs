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
using TelegramLib.Models;
using TelegramVisualPart.Pages;

namespace TelegramVisualPart.UserControls.SettingsControls
{
    /// <summary>
    /// Логика взаимодействия для LogOutMenu.xaml
    /// </summary>
    public partial class LogOutMenu : UserControl
    {
        private TelSystem _system;

        public LogOutMenu()
        {
            InitializeComponent();
        }

        public void SettSystem(TelSystem system)
        {
            _system = system;
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not StackPanel panel) return;
            panel.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not StackPanel panel) return;
            panel.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeOne"];
        }

        private void LogOutBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).LogOut();
        }

        private void EdiProfBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_system is null) return;
            Pages.MyProfile.MyProfileSettings settingsPage = new Pages.MyProfile.MyProfileSettings(_system.LoggedUser, _system);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(settingsPage);
        }

    }
}
