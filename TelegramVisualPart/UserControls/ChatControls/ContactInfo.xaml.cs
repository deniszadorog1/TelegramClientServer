using MaterialDesignThemes.Wpf;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ContactInfo.xaml
    /// </summary>
    public partial class ContactInfo : UserControl
    {
        private TelegramLib.MainClasses.UserChat _chat;
        private TelSystem _system;
        public TelegramLib.MainClasses.UserContactcs _contact;

        public ContactInfo()
        {
            InitializeComponent();
            SetIconsSize();

            this.Visibility = Visibility.Hidden;
        }

        public event Action LoadEnd;
        public async Task SetContactInfo(TelegramLib.MainClasses.UserChat chat,
            TelSystem system, TelegramLib.MainClasses.UserContactcs contact)
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

            SetLanguageText.SetContactInfo(this);
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
            if (_chat.GetChatter().Id != contactUser.Id) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(contactUser, Enums.PrivacySettingType.PhoneNumber);

            await SignalRHelperService.SetPhoneNumber(contactUser, shareType, _chat, MobileNumber.UpperText);
        }

        public void SetLastSeenState(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;
                if (_chat.GetChatter().Id != user.Id) return;

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
                if (_chat is null || _chat.GetChatter().Id != toUpdate.Id) return;
                HelperService.SetOnlineStatusInTextBox(LastSeenOnline, toUpdate.IsOnline, toUpdate.LastSeenOnline);
            });
        }

        private void UpdateContactParams(TelegramLib.MainClasses.User updated)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_contact.Id != updated.Id) return;

                //Username.Text = updated.Login;
                ContName.Text = updated.Name;
                ContSurname.Text = updated.Surname;

                await SetUserPhoneNumber(updated);
                //MobileNumber.UpperText.Text = updated.PhoneNumber;


                UserName.UpperText.Text = updated.Name;
                Birthdate.UpperText.Text = updated.BirthDay is null ? VisConstParamsJsonService.GetStringByName("BirthdayNeverBeen") :
                $"{updated.BirthDay.Value.Day}.{updated.BirthDay.Value.Month}.{updated.BirthDay.Value.Year}";
            });
        }

        private async Task SetUserParams()
        {
            //Username.Text = _chat.GetChatter().Name;

            SetNameSurnameParams();

            await SetOnlineStatus();
            //SetLastSeenOnline();
            await SetMobilePhoneNumber();
            await SetBirtDate();

            UserName.SetUpperText(_chat.GetChatter().Login);
            UserName.UpperText.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
            //UserName.SetBottomText("Username");

            /*Birthdate.SetUpperText(_chat.GetChatter().GetBirthDate());
            Birthdate.SetBottomText("Date of Birth");*/

            //NotificationToggle.IsChecked = _chat.GetChatter().GetNotifsState();

            SentObjsParams();

            await SetContactPhoto();
         /*   UserContactcs contact = _chat.GetChatter();
            ContactImgBrush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(contact.GetFirstImageName().Name), UriKind.Absolute));*/
        }


        public void SetNameSurnameParams()
        {
            if (_contact is null)
            {
                ContName.Text = _chat.Chatter.Name;
                ContSurname.Text =_chat.Chatter.Surname;
            }
            else
            {
                ContName.Text = _contact.Name;
                ContSurname.Text = _contact.Surname;
            }
        }


        public string GetChatterName(int userId)
        {
            UserContactcs contact = _system.GetContactByUserId(_chat.GetChatter().Id);
            return contact is not null ? contact.Name : _chat.GetChatter().Name;
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

            //Birthdate.SetBottomText("Date of Birth");
        }

        public async Task SetMobilePhoneNumber()
        {
            //Is its can be seen
            //if (_chat is null) return;
            TelegramLib.MainClasses.User? user = await GetChatterUser(); //await ApiService.GetUserById(_chat.GetChatter().ContactUserId);
            if (user is null) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.PhoneNumber);

            await SignalRHelperService.SetPhoneNumber(user, shareType, _chat, MobileNumber.UpperText);

            //MobileNumber.SetBottomText("Mobile");
        }

        public async Task SetOnlineStatus()
        {
            //if (_chat is null) return;
            TelegramLib.MainClasses.User? user = await GetChatterUser(); await ApiService.GetUserById(_chat.GetChatter().Id);
            if (user is null) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);
            await SignalRHelperService.SetLastSeenString(user, shareType, _chat, LastSeenOnline);
        }

        public async Task<TelegramLib.MainClasses.User?> GetChatterUser()
        {
            if (_chat is null) return null;
            TelegramLib.MainClasses.User user = 
                await ApiService.GetUserById(_chat.GetChatter().Id);
            return user;
        }

        private void SetOfflineStatus()
        {
            LastSeenOnline.Text = _chat.GetLastSeen();
            LastSeenOnline.Foreground = new SolidColorBrush(Colors.Gray);
        }

        public void SentObjsParams()
        {
            List<MediaAction> medias = _chat.GetMediaMessages();

            SetTextForTextBlock(AmountOfPhotosTextBlock,
                FilesAction.GetImagesFromMediaAction(medias),
                VisConstParamsJsonService.GetStringByName("AmountOfPhotosTextBlock"));

            SetTextForTextBlock(AmountOfVideosTextBlock,
                FilesAction.GetVideosAmount(medias),
                VisConstParamsJsonService.GetStringByName("AmountOfVideosTextBlock"));

            SetTextForTextBlock(AmountOfGifsTextBlock,
                FilesAction.GetGifsAmount(medias),
                VisConstParamsJsonService.GetStringByName("AmountOfGifsTextBlock"));
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
                SetBgToGrid(grid);
            }
            else
            {
                Grid el = GetGrid(sender as FrameworkElement);
                if (el is not null) SetBgToGrid(el);
            }
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid)
            {
                ClearGridBg(grid);
            }
            else
            {
                Grid el = GetGrid(sender as FrameworkElement);
                if (el is not null) ClearGridBg(el);
            }

            Cursor = null;
        }

        public Grid? GetGrid(FrameworkElement el)
        {
            return
                el == SendIcon || el == ShareContactBlock ? ShareLine :
                el == PenIcon || el == EditContactBlock ? EditContactLine :
                el == CanIcon || el == DeleteContactBlock ? DeleteLine :
                el == HandIcon || el == BlockContactBlock ? BlockLine :

                el == ImageIcon || el == AmountOfPhotosTextBlock ? PhotosLine :
                el == VideoIcon || el == AmountOfVideosTextBlock ? VideosLine :
                el == GifIcon || el == AmountOfGifsTextBlock ? FilesLine : 
                el == SendMesBlock ? SendMessageBut : null;

        }

        public void SetBgToGrid(Grid grid)
        {
            grid.Background =
            (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        public void ClearGridBg(Grid grid)
        {
            grid.Background = Brushes.Transparent;
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
                new Pages.UserInfoContact.ActionsFolder.BlockContact(_system, _chat.GetChatter()));
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
            //_chat.GetChatter().SetNotifState(true);

            await ApiService.UpdateContact(_system.LoggedUser.Id, _contact);
        }

        private async void NotificationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_chat is null) return;
            UserContactcs contact = _system.GetContactByUserId(_chat.GetChatter().Id);
            if(contact is not null) contact.SetNotifState(false);

            await ApiService.UpdateContact(_system.LoggedUser.Id, _contact);
        }

        private void UserIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TelegramLib.MainClasses.User contact = _chat.GetChatter();

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

        public void UpdateParams(UserContactcs contact)
        {
            ContName.Text = contact.Name;
            ContSurname.Text = contact.Surname;
        }
    }
}
