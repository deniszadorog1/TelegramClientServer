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
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramVisualPart.Enums;
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
        private TelegramLib.MainClasses.User _user;

        public UserContact()
        {
            InitializeComponent();

            //Update contact vis state
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
            _user = user;

            InitializeComponent();

            SetBasicIamge();
            SetUserImage();

            //HelperService.SetOnlineStatusInTextBox(LastSennOnline, user.IsOnline, user.LastSeenOnline);
            SetBasicParams();

            //last seen row
            SignalRService.SetContactLastSeenVisStateDel += SetLastVisState;

            //photo allowence
            SignalRService.UpdateContactPhotoDel += AddedUserImage;
        }

        public async Task SetBasicParams()
        {
            await SetOnlineStatus(_user);
            await SetActivePhotoImage();
        }

        public async Task SetActivePhotoImage()
        {
            await SignalRHelperService.SetPhotoInEllipse(_user,
                ImgBrushSource, UserImage);
        }

        public void AddedUserImage(TelegramLib.MainClasses.User user)
        {
            Dispatcher.Invoke(async () =>
            {
                if (_user is null || user is null || 
                _user.Id != user.Id) return;
                await SignalRHelperService.SetPhotoInEllipse(user,
                    ImgBrushSource, UserImage);
            });
        }

        public void SetLastVisState(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await SetOnlineStatus(user);
            });
        }

        public async Task SetOnlineStatus(TelegramLib.MainClasses.User user)
        {
            if (user is null || _user.Id != user.Id) return;
            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);

            await SignalRHelperService.SetLastSeenStatus(user, shareType, LastSennOnline);
        }

        public void SetUserImage()
        {
            ImgBrushSource.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_contactImgName), UriKind.Absolute));
        }

        public void SetBasicIamge()
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
