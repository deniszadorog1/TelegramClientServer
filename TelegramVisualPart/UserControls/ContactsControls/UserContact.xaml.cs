using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        public TelegramLib.MainClasses.User _user;

        public UserContact()
        {
            InitializeComponent();

            //Update contact vis state
        }

        public UserContact(TelegramLib.MainClasses.User user)
        {
            _imgSource = string.Empty;
            _login = user.Login;
            _contactImgName = user.GetFirstImageName().Name;
            _user = user;

            InitializeComponent();

            SetLogin();

            SetBasicView();
            //Loaded += async (s, e) => await SetBasicParams();

            //last seen row
            SignalRService.SetContactLastSeenVisStateDel += SetLastVisState;

            //photo allowence
            SignalRService.UpdateContactPhotoDel += AddedUserImage;
        }

        public async void SetBasicView()
        {
            await SetBasicParams(); 
        }

        public async Task SetBasicParams()
        {
            await SetOnlineStatus(_user);
            await SetActivePhotoImage();
        }

        public event Action ImgSet;
        public async Task SetActivePhotoImage()
        {
            await SignalRHelperService.SetPhotoInEllipse(_user,
                ImgBrushSource, UserImage);

            ImgSet?.Invoke();
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

        public async Task SetUserImage()
        {
            ImgBrushSource.ImageSource = new BitmapImage(new Uri(
                await FilesAction.GetUserImagePath(_contactImgName), UriKind.Absolute));
        }

        public void SetLogin()
        {
            UserLogin.Text = _login;
        }

        public bool IsLoginContainsName(string name)
        {
            return UserLogin.Text.Contains(name);
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
