using ControlzEx.Standard;
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
using TelegramLib.Models;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.UserControls.ContactsControls
{
    /// <summary>
    /// Логика взаимодействия для UserContact.xaml
    /// </summary>
    public partial class UserContact : UserControl
    {
        private string _imgSource;
        private string _login;
        private string _contactImgName;

        public UserContact()
        {
            InitializeComponent();
        }

/*        public UserContact(string imgSource, string login,
            DateTime? lastOnline, string contactImgName)
        {
            _imgSource = imgSource;
            _login = login;
            _contactImgName = contactImgName;

            InitializeComponent();

            SetParams();
            SetUserImage();
        }*/

        public UserContact(TelegramLib.MainClasses.User user)
        {
            _imgSource = string.Empty;
            _login = user.Login;
            _contactImgName = user.GetFirstImageName().Name;

            InitializeComponent();

            SetParams();
            SetUserImage();

            HelperService.SetOnlineStatusInTextBox(LastSennOnline, user.IsOnline, user.LastSeenOnline);
            SignalRService.UpdateOnlineStatusDel += UpdateOnlineStatus;
        }

        public void UpdateOnlineStatus(TelegramLib.MainClasses.User toUpdate)
        {
            Dispatcher.Invoke(() =>
            {
                if (toUpdate is null) return;
                HelperService.SetOnlineStatusInTextBox(LastSennOnline, toUpdate.IsOnline, toUpdate.LastSeenOnline);
            });
        }

        public void SetUserImage()
        {
            ImgBrushSource.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_contactImgName), UriKind.Absolute));
        }

        public void SetParams()
        {
            if (_imgSource != string.Empty)
            {
                ImgBrushSource.ImageSource = new BitmapImage(new Uri(_imgSource, UriKind.Absolute));
            }
            UserLogin.Text = _login;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = Brushes.Transparent;
        }
    }
}
