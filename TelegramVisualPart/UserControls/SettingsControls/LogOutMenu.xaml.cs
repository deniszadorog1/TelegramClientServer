using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;

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
            Pages.MyProfile.MyProfileSettings settingsPage =
                new Pages.MyProfile.MyProfileSettings(_system.LoggedUser, _system, new Pages.Settings.SettingsPage(_system)); ;

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(settingsPage);
        }

    }
}
