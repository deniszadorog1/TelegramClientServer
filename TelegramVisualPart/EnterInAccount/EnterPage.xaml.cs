using Microsoft.AspNetCore.Mvc;
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
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.EnterInAccount
{
    /// <summary>
    /// Логика взаимодействия для EnterPage.xaml
    /// </summary>
    public partial class EnterPage : Page
    {
        public event EventHandler SetSystemPage;
        public TelSystem _system;

        public EnterPage()
        {
            InitializeComponent();
        }


        private void RegistrationGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = null;
        }

        private void RegistrationGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = TextDecorations.Underline;
        }

        private void RegistrationGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set registration page
            ((MainWindow)Window.GetWindow(this)).SetMainFrameContent(new RegistrationPage());
        }

        private async void EnterBut_Click(object sender, RoutedEventArgs e)
        {
            //Is field are empty 
            if (string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Text)) return;

            _system = await ApiService.GetTelSystem(LoginBox.Text, PasswordBox.Text);
            SignalRHelperService.SetStatSystem(_system);

            if (_system is null)
            {
                MessageBox.Show("No user with such params");
                ClearBoxes();
                return;
            }
            _system.SetEmptyUserImages();

            bool isOnline = await ApiService.IsUserOnline(_system.LoggedUser.Id);
            if (isOnline)
            {
                MessageBox.Show("User is already online");
                ClearBoxes();
                return;
            }

            _system.Settings.ChatsSettings.SetBasicThemes();

            _system.Settings.ChatsSettings.PossibleWallpapers = 
                FilesAction.GetAllWallpaperNames(_system.Settings.ChatsSettings.PossibleWallpapers);
            _system.Settings.ChatsSettings.Theme = 
                TelegramLib.Enums.Settings.ChatSettings.ThemeType.Night;

            await SetOnlineStatus();

            if (_system.Settings.GetChatSettings().Wallpaper is null)
                _system.Settings.GetChatSettings().Wallpaper =
                    new TelegramLib.UserSettings.SettingsTypes.SubSettings.ChatWallpaper();

            Application.Current.Resources["TempActiveTextColor"] =
                new SolidColorBrush(Color.FromRgb(_system.LoggedUser.MainColor.R,
                _system.LoggedUser.MainColor.G, _system.LoggedUser.MainColor.B));

            ((MainWindow)Window.GetWindow(this)).SetMainPage(_system);
        }

        private async Task SetOnlineStatus()
        {
            _system.LoggedUser.IsOnline = true;
            await ApiService.SetUserOnlineStatus(_system.LoggedUser.Id, true);
        }

        private void ClearBoxes()
        {
            LoginBox.Text = string.Empty;
            PasswordBox.Text = string.Empty;
        }


    }
}
