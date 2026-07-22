using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages
{
    /// <summary>
    /// Логика взаимодействия для ChatToApply.xaml
    /// </summary>
    public partial class ChatToApply : UserControl
    {
        private UserContactcs _contact;
        private TelegramLib.MainClasses.User _user;
/*        public ChatToApply()
        {
            InitializeComponent();

            SetActions();
        }*/

/*        public ChatToApply(UserContactcs contact)
        {
            _contact = contact;
            InitializeComponent();

            //SetAutoDeleteParams();

            SetActions();
        }*/

        public ChatToApply(TelegramLib.MainClasses.User user)
        {
            _user = user;
            InitializeComponent();

            //SetAutoDeleteParams();
            //SetActions();
        }

        public async Task SetActions()
        {
            if (_contact is null) return;
            await SetBasicLastSeenState();
            await SetActivePhotoImage();

            SignalRService.SetContactLastSeenVisStateDel += SetLastVisState;
            SignalRService.UpdateContactPhotoDel += AddedUserImage;
        }

        public async Task SetActivePhotoImage()
        {
            int userId = _contact is null ? _user.Id : _contact.Id;

            TelegramLib.MainClasses.User user =
                await ApiService.GetUserById(userId);
            if (user is null) return;

            await SignalRHelperService.SetPhotoInEllipse(user,
                UserImageBrush, UserImageEllipse);
        }

        public void AddedUserImage(TelegramLib.MainClasses.User user)
        {
            Dispatcher.Invoke(async () =>
            {
                int id = GetUserId();
                if (user is null || id != user.Id) return;
                await SignalRHelperService.SetPhotoInEllipse(user,
                    UserImageBrush, UserImageEllipse);
            });
        }

        public int GetUserId()
        {
            return _contact is null ? _user.Id : _contact.Id;
        }

        public async Task SetBasicLastSeenState()
        {
            await SetLastSeenText(await ApiService.GetUserById(GetUserId()));
        }

        public void SetLastVisState(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await SetLastSeenText(user);
            });
        }

        public async Task SetLastSeenText(TelegramLib.MainClasses.User user)
        {
            int id = GetUserId();
            if (user is null || id != user.Id) return;
            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);

            await SignalRHelperService.SetLastSeenStatus(user, shareType, AutoDeletionType);
        }

        public async Task SetParams(string imgName, string upperText, string bottomText)
        {
            string fullPath = await FilesAction.GetUserImagePath(imgName);
            BitmapImage bitmap = ApiService.GetCachedBitmap(imgName);

            UserImageBrush.ImageSource = bitmap is not null ? bitmap :
                await SignalRHelperService.LoadBitmap(fullPath);

            //new BitmapImage(new Uri(FilesAction.GetUserImagePath(imgName), UriKind.Absolute));
            TypeName.Text = upperText;
            AutoDeletionType.Text = bottomText;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeDeviderField"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Background = new SolidColorBrush(Colors.Transparent);
        }

        private bool _isClicked = false;
        public void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isClicked = !_isClicked;

            if (_isClicked)
            {
                ChosenChatIconBorder.Background = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
                return;
            }
            ChosenChatIconBorder.Background = Brushes.Transparent;
        }

        public bool GetIdClicked()
        {
            return _isClicked;
        }

        public void DiscardChat()
        {
            _isClicked = false;
            ChosenChatIconBorder.Background = Brushes.Transparent;
        }

        public string GetTypeName()
        {
            return TypeName.Text;
        }

        public void SetChatParams(TelegramLib.MainClasses.User user)
        {
            SetActivePhotoImage();
        }

        public void SetSavedMesChatGrid()
        {
            SavedChatGrid.Visibility = Visibility.Visible;
            UserImageEllipse.Visibility = Visibility.Hidden;

            TypeName.Text = "Saved messages";
            AutoDeletionType.Text = "Forward messages here";
        }
    }
}
