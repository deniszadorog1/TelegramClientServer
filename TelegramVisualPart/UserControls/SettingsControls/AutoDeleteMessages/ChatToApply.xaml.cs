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

namespace TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages
{
    /// <summary>
    /// Логика взаимодействия для ChatToApply.xaml
    /// </summary>
    public partial class ChatToApply : UserControl
    {
        private UserContactcs _contact;

        public ChatToApply()
        {
            InitializeComponent();

            SetActions();
        }

        public ChatToApply(UserContactcs contact)
        {
            _contact = contact;
            InitializeComponent();

            SetAutoDeleteParams();

            SetActions();
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
            TelegramLib.MainClasses.User user = 
                await ApiService.GetUserById(_contact.ContactUserId);
            if (user is null) return;

            await SignalRHelperService.SetPhotoInEllipse(user,
                UserImageBrush, UserImageEllipse);
        }

        public void AddedUserImage(TelegramLib.MainClasses.User user)
        {
            Dispatcher.Invoke(async () =>
            {
                if (_contact is null || user is null ||
                _contact.ContactUserId!= user.Id) return;
                await SignalRHelperService.SetPhotoInEllipse(user,
                    UserImageBrush, UserImageEllipse);
            });
        }

        public async Task SetBasicLastSeenState()
        {
            await SetLastSeenText(await ApiService.GetUserById(_contact.ContactUserId));
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
            if (user is null || _contact.ContactUserId != user.Id) return;
            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);

            await SignalRHelperService.SetLastSeenStatus(user, shareType, AutoDeletionType);

        }

        private void SetAutoDeleteParams()
        {
            UserImageBrush.ImageSource = new BitmapImage(new Uri(
                    FilesAction.GetUserImagePath(_contact.GetFirstImageName().Name), UriKind.Absolute));
            TypeName.Text = _contact.Name;

            //Set auto disable params

            string lowerText = (_contact.AutoDeletion is null ||
                _contact.AutoDeletion.Type == TelegramLib.Enums.Chat.AutoDeleteType.Nothing) ?
                "Auto-delete disabled" :
                $"auto - delete after{_contact.AutoDeletion.GetStringByType()}";

            AutoDeletionType.Text = lowerText;

            AutoDeletionType.Foreground = (_contact.AutoDeletion is null ||
                _contact.AutoDeletion.Type == TelegramLib.Enums.Chat.AutoDeleteType.Nothing) ?
                new SolidColorBrush(Colors.Gray) :
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
        }

        public void SetParams(string imgName, string upperText, string bottomText)
        {
            UserImageBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(imgName), UriKind.Absolute));

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
    }
}
