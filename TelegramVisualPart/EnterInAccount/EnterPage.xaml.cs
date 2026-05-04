using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
using TelegramLib.Services;
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

            string token = await ApiService.GetToken(LoginBox.Text, PasswordBox.Text);

            if (!string.IsNullOrEmpty(token))
            {
                ApiService.SetAuthToken(token);
                _system = /*await ApiService.GetTelSystemSinglton(); //*/ await ApiService.GetTelSystem();

                _system.Token = token;

                if(_system is null)
                {
                    MessageBox.Show("Bruh...");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Wrong login or password");
                return;
            }


            //_system = await ApiService.GetTelSystem(LoginBox.Text, PasswordBox.Text);
            
            SignalRHelperService.SetStatSystem(_system);

/*
            UserChat chat = _system.Chats.First(x => x.Id == 4);
            for (int i = 0; i < 100000; i++)
            {
                DbService.AddMessage(chat, new TelegramLib.MainClasses.Messages.TextMessage(-1, 1, DateTime.Now, "asd", false, -1, false, null, false));
            }
*/
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

            //MessageBox.Show("1.I am online. Starting to set basic params");//

            _system.Settings.ChatsSettings.SetBasicThemes();

            _system.Settings.ChatsSettings.PossibleWallpapers = 
                FilesAction.GetAllWallpaperNames(_system.Settings.ChatsSettings.PossibleWallpapers);

            //MessageBox.Show("2. Set basic wallpapers!");

            _system.Settings.ChatsSettings.Theme = 
                TelegramLib.Enums.Settings.ChatSettings.ThemeType.Night;

            Application.Current.Resources["AppFont"] =
              new FontFamily(_system.Settings.ChatsSettings.FontName); 

            await SetOnlineStatus();

            if (_system.Settings.GetChatSettings().Wallpaper is null)
                _system.Settings.GetChatSettings().Wallpaper =
                    new TelegramLib.UserSettings.SettingsTypes.SubSettings.ChatWallpaper();

            Application.Current.Resources["TempActiveTextColor"] =
                new SolidColorBrush(Color.FromRgb(_system.LoggedUser.MainColor.R,
                _system.LoggedUser.MainColor.G, _system.LoggedUser.MainColor.B));
             

            //MessageBox.Show("3. Eneded of Setting basic params! going to create main page");

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
