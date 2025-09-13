using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ContactInfo.xaml
    /// </summary>
    public partial class ContactInfo : UserControl
    {
        private TelegramLib.MainClasses.UserChat _chat;
        private TelSystem _system;
        private UserContactcs _contact;

        public ContactInfo()
        {
            InitializeComponent();
            SetIconsSize();

            this.Visibility = Visibility.Hidden;
        }

        public event Action LoadEnd;
        public async Task SetContactInfo(TelegramLib.MainClasses.UserChat chat,
            TelSystem system, UserContactcs contact)
        {
            _system = system;
            _chat = chat;
            _contact = contact;

            await SetUserParams();

            SignalRService.UpdateContactDel += UpdateContactParams;
            SignalRService.UpdateOnlineStatusDel += UpdateOnlineStatus;
            //SignalRService.SetContactPhoneNumberVisibilityDel += SetPhoneNumberVisAction;

            SignalRService.SetContactLastSeenVisStateDel += SetLastSeenState;
            SignalRService.SetPhoneNumVisByExpsDel += SetPhoneNumberVisByExps;
            SignalRService.UpdateBirthDateDel += UpdateBirthDate;
            SignalRService.UpdateContactPhotoDel += UpdateContactPhoto;

            this.Visibility = Visibility.Visible;
            LoadEnd?.Invoke();
        }

        public void UpdateContactPhoto(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;

                Console.WriteLine(_system.Settings.PrivacySettings.ProfPhotoPrivacy);
                
                await SignalRHelperService.SetContactPhoto(user, 
                    _chat, ContactImgBrush, UserIcon);
            });
        }

        public void UpdateBirthDate(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;
                await SignalRHelperService.SetBirthDate(user, _chat, Birthdate.UpperText);
            });
        }

        public void SetPhoneNumberVisByExps(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;
                await SetUserPhoneNumber(user);
            });
        }

        public async Task SetUserPhoneNumber(TelegramLib.MainClasses.User contactUser)
        {
            if (_chat.GetChatter().ContactUserId != contactUser.Id) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(contactUser, Enums.PrivacySettingType.PhoneNumber);

            await SignalRHelperService.SetPhoneNumber(contactUser, shareType, _chat, MobileNumber.UpperText);
        }

        public void SetLastSeenState(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;
                if (_chat.GetChatter().ContactUserId != user.Id) return;

                IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);

                await SignalRHelperService.SetLastSeenString(user, shareType, _chat, LastSeenOnline);
            });
        }

        public void SetPhoneNumberVisAction(bool isVis, TelegramLib.MainClasses.User updatedUser)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await SetUserPhoneNumber(updatedUser);
                /*                if (_chat is null || _chat.GetChatter().ContactUserId != updatedUser.Id) return;
                                MobileNumber.UpperText.Text = isVis ? _contact.PhoneNumber : "Its hidden LOOOOLL";*/
            });
        }

        public void UpdateOnlineStatus(TelegramLib.MainClasses.User toUpdate)
        {
            Dispatcher.Invoke(() =>
            {
                if (_chat is null || _chat.GetChatter().ContactUserId != toUpdate.Id) return;
                HelperService.SetOnlineStatusInTextBox(LastSeenOnline, toUpdate.IsOnline, toUpdate.LastSeenOnline);
            });
        }

        private void UpdateContactParams(TelegramLib.MainClasses.User updated)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_contact.ContactUserId != updated.Id) return;

                Username.Text = updated.Login;

                await SetUserPhoneNumber(updated);
                //MobileNumber.UpperText.Text = updated.PhoneNumber;


                UserName.UpperText.Text = updated.Name;
                Birthdate.UpperText.Text = updated.BirthDay is null ? "Never been" :
                $"{updated.BirthDay.Value.Day}.{updated.BirthDay.Value.Month}.{updated.BirthDay.Value.Year}";
            });
        }

        private async Task SetUserParams()
        {
            Username.Text = _chat.GetChatter().Name;

            await SetOnlineStatus();
            //SetLastSeenOnline();
            await SetMobilePhoneNumber();
            await SetBirtDate();


            UserName.SetUpperText(_chat.GetChatter().GetUserName());
            UserName.UpperText.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
            UserName.SetBottomText("Username");

            /*Birthdate.SetUpperText(_chat.GetChatter().GetBirthDate());
            Birthdate.SetBottomText("Date of Birth");*/

            NotificationToggle.IsChecked = _chat.GetChatter().GetNotifsState();

            SentObjsParams();


            await SetContactPhoto();
         /*   UserContactcs contact = _chat.GetChatter();
            ContactImgBrush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(contact.GetFirstImageName().Name), UriKind.Absolute));*/
        }

        public async Task SetContactPhoto()
        {
            TelegramLib.MainClasses.User? user = await GetChatterUser();
            if (user is null) return;
            
            await SignalRHelperService.SetContactPhoto(user,
                _chat, ContactImgBrush, UserIcon);
        }

        public async Task SetBirtDate()
        {
            TelegramLib.MainClasses.User? user = await GetChatterUser(); 
            if (user is null) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.PhoneNumber);

            await SignalRHelperService.SetBirthDate(user, _chat, Birthdate.UpperText);

            Birthdate.SetBottomText("Date of Birth");
        }

        public async Task SetMobilePhoneNumber()
        {
            //Is its can be seen
            //if (_chat is null) return;
            TelegramLib.MainClasses.User? user = await GetChatterUser(); //await ApiService.GetUserById(_chat.GetChatter().ContactUserId);
            if (user is null) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.PhoneNumber);

            await SignalRHelperService.SetPhoneNumber(user, shareType, _chat, MobileNumber.UpperText);

            MobileNumber.SetBottomText("Mobile");
        }

        public async Task SetOnlineStatus()
        {
            //if (_chat is null) return;
            TelegramLib.MainClasses.User? user = await GetChatterUser(); await ApiService.GetUserById(_chat.GetChatter().ContactUserId);
            if (user is null) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);
            await SignalRHelperService.SetLastSeenString(user, shareType, _chat, LastSeenOnline);
        }

        public async Task<TelegramLib.MainClasses.User?> GetChatterUser()
        {
            if (_chat is null) return null;
            TelegramLib.MainClasses.User user = 
                await ApiService.GetUserById(_chat.GetChatter().ContactUserId);
            return user;
        }

        private void SetOfflineStatus()
        {
            LastSeenOnline.Text = _chat.GetChatter().GetLastSeen();
            LastSeenOnline.Foreground = new SolidColorBrush(Colors.Gray);
        }

        public void SentObjsParams()
        {
            List<MediaAction> medias = _chat.GetMediaMessages();

            SetTextForTextBlock(AmountOfPhotosTextBlock,
                FilesAction.GetImagesFromMediaAction(medias),
                "Amount of photos");

            SetTextForTextBlock(AmountOfVideosTextBlock,
                FilesAction.GetVideosAmount(medias),
                "Amount of videos");

            SetTextForTextBlock(AmountOfGifsTextBlock,
                FilesAction.GetGifsAmount(medias),
                "Amount of GIFs");
        }

        private void SetTextForTextBlock(TextBlock block, int amount, string baseString)
        {
            string amountStr = amount > 0 ? amount.ToString() + " " : string.Empty;
            block.Text = $"{amountStr}{baseString}";
        }

        private void SetIconsSize()
        {
            SetIconSize(InfoIcon);
            SetIconSize(BellIcon);

            SetIconSize(ImageIcon);
            SetIconSize(VideoIcon);
            //SetIconSize(FileIcon);
            //SetIconSize(LinkIcon);
            SetIconSize(GifIcon);

            SetIconSize(SendIcon);
            SetIconSize(PenIcon);
            SetIconSize(CanIcon);
            SetIconSize(HandIcon);

            ContactMenu.Margin = new Thickness(
                0,
                UpperRow.Height.Value + 10,
                20,
                0
                );
        }

        private const int _iconWidth = 30;
        private const int _iconHeight = 30;
        private void SetIconSize(PackIcon icon)
        {
            icon.Width = _iconWidth;
            icon.Height = _iconHeight;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is Grid grid)
            {
                grid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
            }
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid)
            {
                grid.Background = Brushes.Transparent;
            }
            Cursor = null;
        }

        private void SendMessageBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = Brushes.White;
        }

        private void CloseButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBut.Foreground = Brushes.Gray;
        }

        private void CloseButGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void MenuButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            MoreInfoBut.Foreground = Brushes.White;
        }

        private void MenuButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            MoreInfoBut.Foreground = Brushes.Gray;
        }

        private void BlockLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                new Pages.UserInfoContact.ActionsFolder.BlockContact(_system, _contact));
        }

        private void DeleteLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                new Pages.UserInfoContact.ActionsFolder.DeleteContact(_contact, _system));
        }

        private void EditContactLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame
                (new Pages.UserInfoContact.ActionsFolder.EditUserContact(_system.LoggedUser, _contact));
        }

        private void ShareLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                new Pages.UserInfoContact.ActionsFolder.ShareContact(_system, _contact));
        }

        private void Line_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_chat is null) return;
            if (sender is Grid grid)
            {
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.
                    SentObjectsUserInfo.SentItemsUserContact(
                    ((MainWindow)Window.GetWindow(this)).GetSystem(),
                    GetItemType(grid.Name), _chat));
            }
        }

        private Enums.SentItemsTypes GetItemType(string name)
        {
            return name == PhotosLine.Name.ToString() ? Enums.SentItemsTypes.Photos :
                name == VideosLine.Name.ToString() ? Enums.SentItemsTypes.Video :
                name == FilesLine.Name.ToString() ? Enums.SentItemsTypes.File :
                name == LinksLine.Name.ToString() ? Enums.SentItemsTypes.SharedLinks :
                name == GIFsLine.Name.ToString() ? Enums.SentItemsTypes.GIFs :
                Enums.SentItemsTypes.Photos;
        }

        private bool _isMenuOpen = false;

        private void MenuButGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMenuOpen = !_isMenuOpen;

            if (_isMenuOpen)
            {
                ContactMenu.Visibility = Visibility.Visible;
                ContactMenu.SetTelSystemParam(_system, _chat);
            }
            else
            {
                ContactMenu.Visibility = Visibility.Hidden;
            }
        }

        private async void NotificationToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_chat is null) return;
            _chat.GetChatter().SetNotifState(true);

            await ApiService.UpdateContact(_system.LoggedUser.Id, _contact);
        }

        private async void NotificationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_chat is null) return;
            _chat.GetChatter().SetNotifState(false);

            await ApiService.UpdateContact(_system.LoggedUser.Id, _contact);
        }

        private void UserIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UserContactcs contact = _chat.GetChatter();

            string firstImage = contact.GetFirstImageName().Name;
            Image chosen = FilesAction.GetUserImage(firstImage);

            List<Image> imgs = FilesAction.GetUserImages(contact.GetImagesNames());

            VisualActionPage page = new VisualActionPage(chosen, imgs);
            page.SetUserImages(contact.UserImages, _system, contact.Name, false, null);

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);
        }


        private void UserIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
