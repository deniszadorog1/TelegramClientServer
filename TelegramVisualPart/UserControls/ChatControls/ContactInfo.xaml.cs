using MaterialDesignThemes.Wpf;
using System.CodeDom;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Xml.Linq;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Pages.UserInfoContact.ActionsFolder;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls.UserContactControls;
using TelegramVisualPart.UserControls.ChatsControls;
using TelegramVisualPart.Windows;
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
        public event Action SendMesPressed;

        public ContactInfo()
        {
            InitializeComponent();
            SetIconsSize();

            this.Visibility = Visibility.Hidden;
            SetMenuVisibility(Visibility.Hidden);
        }

        public void SetMenuVisibility(Visibility vis)
        {
            MenuButGrid.Visibility = vis;
        }

        public event Action LoadEnd;
        private bool _isSetMaxHeight;

        public async Task SetContactInfo(TelegramLib.MainClasses.UserChat chat,
            TelSystem system, TelegramLib.MainClasses.UserContactcs contact, bool isSetMaxHeight = true)
        {
            _system = system;
            _chat = chat;
            _contact = contact;
            _isSetMaxHeight = isSetMaxHeight;

            if (!_isSetMaxHeight) MaxHeight = int.MaxValue;

            await SetInfoVisibility();

            SetBioRow(_chat.Chatter);

            SignalRService.UpdateContactDel += UpdateContactParams;
            SignalRService.UpdateOnlineStatusDel += UpdateOnlineStatus;
            //SignalRService.SetContactPhoneNumberVisibilityDel += SetPhoneNumberVisAction;

            SignalRService.SetContactLastSeenVisStateDel += SetLastSeenState;
            SignalRService.SetPhoneNumVisByExpsDel += SetPhoneNumberVisByExps;
            SignalRService.UpdateBirthDateDel += UpdateBirthDate;
            SignalRService.UpdateContactPhotoDel += UpdateContactPhoto;

            this.Visibility = Visibility.Visible;

            SetLanguageText.SetContactInfo(this);

            BlockButVisibility();
            SetStartToggleState();

            LoadEnd?.Invoke();
        }

        public void SetStartToggleState()
        {
            NotificationToggle.IsChecked = _chat.GetNotificationStatus();
        }

        public async Task SetInfoVisibility()
        {
            SetBlocksVisibility();
            await SetUserParams();
        }
        public void BlockButVisibility()
        {
            bool isBlocked = _system.LoggedUser.IsUserIsBlockedById(_chat.Chatter.Id);
            BlockContactBlock.Text = isBlocked ? "Unblock contact" : "Block contact";
        }

        public void SetBlocksVisibility()
        {
            SetIsContactRemovedVis();
            SetMediasGridsVisibility();
        }

        public void SetInfoLinesVisibility()
        {
            if (Birthdate.UpperText.Text ==
                VisConstParamsJsonService.GetStringByName("CantSeeStuff") ||

                Birthdate.UpperText.Text ==
                VisConstParamsJsonService.GetStringByName("BirthDatNotSet"))
            {
                InfoRow.Height = new GridLength(
                    InfoRow.Height.Value -
                    BirthdatRow.Height.Value);

                MaxHeight -= BirthdatRow.Height.Value;
                BirthdatRow.Height = new GridLength(0);
            }
        }

        public void SetMediasGridsVisibility()
        {
            List<MediaAction> medias = _chat.GetMediaMessages();

            //is photo amount == 0
            if (FilesAction.GetImagesFromMediaAction(medias) == 0)
            {
                PhotosLine.Visibility = Visibility.Hidden;
                MaxHeight -= PhotoRow.Height.Value;
                PhotoRow.Height = new GridLength(0);
            }

            //is videos amount == 0
            if (FilesAction.GetVideosAmount(medias) == 0)
            {
                VideosLine.Visibility = Visibility.Hidden;
                MaxHeight -= VideosRow.Height.Value;
                VideosRow.Height = new GridLength(0);
            }

            //is gifs amount == 0
            if (FilesAction.GetGifsAmount(medias) == 0)
            {
                GifLine.Visibility = Visibility.Hidden;
                MaxHeight -= GifRow.Height.Value;
                GifRow.Height = new GridLength(0);
            }

            if (GifLine.Visibility == Visibility.Hidden &&
                VideosLine.Visibility == Visibility.Hidden &&
                PhotosLine.Visibility == Visibility.Hidden)
            {
                BottomDivideLine.Visibility = Visibility.Hidden;
            }
            else BottomDivideLine.Visibility = Visibility.Visible;

            SetMediasRowVisibility();
            SetAddContactVisibility();
        }

        public void SetAddContactVisibility()
        {
            bool isContact = _system.IsChatterIdIsContact(_chat.Chatter.Id);

            if (isContact)
            {
                AddContactRow.Height = new GridLength(0);
                //AddContactBlock.Visibility = Visibility.Hidden;
                //MaxHeight -= InfoRow.Height.Value - 260;
                InfoRow.Height = new GridLength(280);
                return;
            }
            AddContactRow.Height = new GridLength(50);
            //AddContactBlock.Visibility = Visibility.Visible;
            //MaxHeight += 330 - InfoRow.Height.Value;
            InfoRow.Height = new GridLength(330);
        }

        public void SetMediasRowVisibility()
        {
            MediasRow.Height = new GridLength(
                PhotoRow.Height.Value + VideosRow.Height.Value + GifRow.Height.Value + 10);

            MaxHeight += 5;

            if (BottomDivideLine.Visibility == Visibility.Hidden)
            {
                MaxHeight -= 10;
                DivRow.Height = new GridLength(0);
            }
            else DivRow.Height = new GridLength(10);

            if (MediasRow.Height.Value == 0) DivRow.Height = new GridLength(0);
        }

        public const int _hiddenParasHeight = 150;
        public void SetIsContactRemovedVis()
        {
            /*            if (!_isSetMaxHeight)
                        {
                            return;
                        }*/
            if (_contact is null)
            {
                //Hide lines
                ShareRow.Height = new GridLength(0);
                EditRow.Height = new GridLength(0);
                DeleteRow.Height = new GridLength(0);

                //Set page height
                ToBeHiddenButs.Height = new GridLength(50);

                MaxHeight -= _hiddenParasHeight;
            }
            else if (ToBeHiddenButs.Height.Value != 200) //lines are not hidden
            {
                ShareRow.Height = new GridLength(50);
                EditRow.Height = new GridLength(50);
                DeleteRow.Height = new GridLength(50);

                //Set page height
                ToBeHiddenButs.Height = new GridLength(200);

                MaxHeight += _hiddenParasHeight;
            }

        }

        public int GetHiddenParamsHeight()
        {
            return _hiddenParasHeight;
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

                _system.UpdateUserBirthdate(user);
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
                if (_contact is null || _contact.ContactUserId != updated.Id) return;

                //Username.Text = updated.Login;
                ContName.Text = updated.Name;
                ContSurname.Text = updated.Surname;

                await SetUserPhoneNumber(updated);
                //MobileNumber.UpperText.Text = updated.PhoneNumber;

                UserName.UpperText.Text = updated.Name;

                SetBioRow(updated);

                Birthdate.UpperText.Text = updated.BirthDay is null ? VisConstParamsJsonService.GetStringByName("BirthdayNeverBeen") :
                $"{updated.BirthDay.Value.Day}.{updated.BirthDay.Value.Month}.{updated.BirthDay.Value.Year}";
            });
        }

        private const int _addInfoRowHeight = 55;
        private const int _baseInfoRowHeight = 280;

        public void SetBioRow(TelegramLib.MainClasses.User toUpdate)
        {
            if (toUpdate.BIO == string.Empty)
            {
                BioRow.Height = new GridLength(0);
                UpdateSizeWithBioRow(toUpdate);
                return;
            }
            else BioRow.Height = new GridLength(_addInfoRowHeight);

            UpdateSizeWithBioRow(toUpdate);
            Bio.UpperText.Text = "Bio";
            Bio.BottomText.Text = $"{toUpdate.BIO}";
        }

        public void UpdateSizeWithBioRow(TelegramLib.MainClasses.User user)
        {
            if (user.BIO == string.Empty)
            {
                InfoRow.Height = new GridLength(_baseInfoRowHeight);
            }
            else InfoRow.Height = new GridLength(_baseInfoRowHeight + _addInfoRowHeight);
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

            SentObjsParams();

            await SetContactPhoto();

            AddFoldersSubMenu();
        }

        public void AddFoldersSubMenu()
        {
            ContactMenu.AddToFolder.MouseEnter += (sender, e) =>
            {
                Point relativePoint = e.GetPosition(this);
                Point point = new Point(relativePoint.X, relativePoint.Y - 15);

                UserContactMenu foldMenu = new UserContactMenu();
                foldMenu.SetTelSystemParam(_system, _chat);

                foldMenu.SetFoldersParams();

                ContactMenusCanvas.Children.Add(foldMenu);

                foldMenu.MouseLeave += (sender, e) =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                ContactMenusCanvas.MouseLeave += (sender, e) =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                ContactMenu.ClearThis += () =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                foldMenu.ClearThis += () =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                Canvas.SetLeft(foldMenu, Canvas.GetLeft(ContactMenu) - foldMenu.Width);
                Canvas.SetTop(foldMenu, Canvas.GetTop(ContactMenu) + ContactMenu.GetAddFolderButPos());
            };
        }

        public void AddMenuElement(UserChatMenu menu, Point cordPoint)
        {
            //MenusCan.Children.Add(menu);

            /*            Window window = Window.GetWindow(menu);
                        if (window is null ||
                            window is not MainWindow) throw new Exception("Its should be Main Window");

                        menu.SetWindow(window as MainWindow);*/

            Canvas.SetLeft(menu, cordPoint.X + 100);
            Canvas.SetTop(menu, cordPoint.Y);
        }



        public void SetNameSurnameParams()
        {
            if (_contact is null)
            {
                ContName.Text = _chat.Chatter.Name;
                ContSurname.Text = _chat.Chatter.Surname;
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

        public event Action UpdateAction;

        public async Task SetBirtDate()
        {
            TelegramLib.MainClasses.User? user = _chat.Chatter;//  await GetChatterUser();
            if (user is null) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.PhoneNumber);

            await SignalRHelperService.SetBirthDate(user, _chat, Birthdate.UpperText);

            SetInfoLinesVisibility();

            UpdateAction?.Invoke();
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
                0,
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
                el == GifIcon || el == AmountOfGifsTextBlock ? GifLine :
                el == SendMesBlock ? SendMessageBut : null;

        }

        public void SetBgToGrid(Grid grid)
        {
            Cursor = Cursors.Hand;
            grid.Background =
            (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        public void ClearGridBg(Grid grid)
        {
            Cursor = null;
            grid.Background = Brushes.Transparent;
        }

        private void SendMessageBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set User chat 
            if (_chat is null) return;

            ((MainWindow)Window.GetWindow(this))
                .SetOtherChatByUserId(_chat.GetChatter().Id);

            SendMesPressed?.Invoke();
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();            
        }

        private void CloseButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            CloseBut.Foreground = Brushes.White;
        }

        private void CloseButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
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
            bool isBlocked = _system.LoggedUser.IsUserIsBlockedById(_chat.Chatter.Id);

            if (isBlocked)//unblock action
            {
                // unblock in db
                ApiService.RemoveBlockedContact(_system.LoggedUser.Id, _chat.Chatter.Id);

                //unblock in system
                _system.LoggedUser.UnblockUserById(_chat.Chatter.Id);
            }
            else
            {
                //Set to block page
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                    new Pages.UserInfoContact.ActionsFolder.BlockContact(_system, _chat.GetChatter()));
            }
            ((MainWindow)Window.GetWindow(this)).SetFramesAfterBlockingContact();
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
                name == GifLine.Name.ToString() ? Enums.SentItemsTypes.GIFs : 
                //name == FilesLine.Name.ToString() ? Enums.SentItemsTypes.File :
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
                ContactMenu.UpdateParamsIsChatterIsNotContact();


                //Set 
                SetMenuPosition(e.GetPosition(this));
            }
            else
            {
                ContactMenu.Visibility = Visibility.Hidden;
                RemoveSubMenus();
            }
        }

        public void RemoveSubMenus()
        {
            List<UIElement> menus = new List<UIElement>();

            for (int i = 0; i < ContactMenusCanvas.Children.Count; i++)
            {
                if (ContactMenusCanvas.Children[i] != ContactMenu)
                {
                    menus.Add(ContactMenusCanvas.Children[i]);
                }
            }

            foreach (var el in menus)
            {
                ContactMenusCanvas.Children.Remove(el);
            }
        }

        public void SetMenuPosition(Point relativePoint)
        {
            Point point = new Point(relativePoint.X, relativePoint.Y);

            Canvas.SetLeft(ContactMenu, point.X - ContactMenu.Width / 1.3);
            Canvas.SetTop(ContactMenu, point.Y - 50);
        }

        private async void NotificationToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_chat is null) return;
            await ApiService.ChangeNotificationState(_chat.Id, true);
            _chat.ChangeNotificationStatus(true);

            //_chat.GetChatter().SetNotifState(true);

            //await ApiService.UpdateContact(_system.LoggedUser.Id, _contact);
        }

        private async void NotificationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_chat is null) return;
            await ApiService.ChangeNotificationState(_chat.Id, false);
            _chat.ChangeNotificationStatus(false);

            //UserContactcs contact = _system.GetContactByUserId(_chat.GetChatter().Id);
            //if (contact is not null) contact.SetNotifState(false);

            //await ApiService.UpdateContact(_system.LoggedUser.Id, _contact);
        }

        private void UserIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set window here
            MediaWindow mediaWindow = new MediaWindow(
                _chat.GetChatter(), (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.OtherUserImages, _system);

           
            //Is exist
            if (((MainWindow)Window.GetWindow(this))
                .IsMediaWindowIsExistByUserId(_chat.GetChatter().Id)) return;

            mediaWindow.Show();

/*            TelegramLib.MainClasses.User contact = _chat.GetChatter();

            string firstImage = contact.GetFirstImageName().Name;
            Image chosen = FilesAction.GetUserImage(firstImage);

            List<Image> imgs = FilesAction.GetUserImages(contact.GetImagesNames());

            VisualActionPage page = new VisualActionPage(chosen, imgs);
            page.SetUserImages(contact.UserImages, _system, contact.Name, false, null);

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);*/
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

        public void ContactRemovedAction()
        {
            ContName.Text = _chat.Chatter.Name;
            ContSurname.Text = _chat.Chatter.Surname;

            //Set hide action params
        }

        private void AddContactGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid) SetBgToGrid(grid);
        }

        private void AddContactGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid) ClearGridBg(grid);
        }

        private void AddContactGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set Add Contact Page
            EditUserContact contact = 
                new EditUserContact(_chat.Chatter, _system);
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(contact);
        }
    }
}
