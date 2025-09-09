using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System.Security.Permissions;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls;
using TelegramVisualPart.UserControls.ChatsControls;
using TelegramVisualPart.UserControls.ChatsSearch;
using TelegramVisualPart.UserControls.ContactsControls;
using TelegramVisualPart.UserControls.DifferButs;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainChatPage.xaml
    /// </summary>
    public partial class MainChatPage : Page
    {
        private TelSystem _system;
        private TelegramLib.MainClasses.UserChat _chosenChat;

        public MainChatPage(TelSystem system)
        {
            //Folders
            //contacts
            //chats
            //messages

            _system = system;
            InitializeComponent();

            SetBasicParams();

            FolderSliderMenu.SetSliderWithFolders(_system.Folders, _system);
        }

        public async Task SetBasicParams()
        {
            /*            LeftButtons.OnMenuClick += LeftButtons_OnMenuClick;
                        LeftButtons.OnMenuClick += LeftButtons_OnMenuClick;*/

            SetDrawButsStyles();

            SetChatClick();

            LeftButtons.SetSystemParam(_system);

            UserChat.SetSystemParam(_system);
            UserChat.BackButton_MouseDown += BackButton_MouseDown;

            SetUserImage();

            //await UpdateUserChatsPanel();

            SetNoChatBg();

            await SetActiveChats();

            SetColorToSettingsButs();

            SliderLogin.Text = _system.LoggedUser.Login;

            SearchControl.SetSearchType += SetSearchedParams;

            SignalRService.UpdateUserImage += AddedUserImage;
            SignalRService.UpdateContactPhotoDel += AddedUserImage;

            //Check with last message
            SignalRService.ClearChatDel += ClearChatAction;

            SignalRService.TextMessageReceived += SentTextMessage;
            SignalRService.MediaMessageReceived += SetMediaMessage;
        }

        public void SetColorToSettingsButs()
        {
            List<MenuIconTextBut> buts = SlideMenuPanel.Children.OfType<MenuIconTextBut>().ToList();
            for (int i = 0; i < buts.Count(); i++)
            {
                SetColorToSettingBut(buts[i]);
            }
        }

        //private readonly SolidColorBrush _textColor = new SolidColorBrush(Colors.White);
        public void SetColorToSettingBut(MenuIconTextBut but)
        {
            const int iconSize = 27;

            /*            but.IconType.Foreground = _textColor;
                        but.ButName.Foreground = _textColor;
            */
            but.IconType.Width = iconSize;
            but.IconType.Height = iconSize;

            //but.ButName.FontWeight = FontWeights.SemiBold;
            but.Margin = new Thickness(0, 0, 0, 0);
            but.Height = 45;
            but.ButName.FontSize = 19;
            but.FontFamily = new FontFamily("Calibri");

        }
        public void SetMediaMessage(TelegramLib.MainClasses.User user,
            MediaAction media)
        {
            Dispatcher.Invoke(() =>
            {
                SetInUserTalkMessageLastMessage(user.Login, "media");
            });
        }

        public void SentTextMessage(TelegramLib.MainClasses.User user,
            TelegramLib.MainClasses.Messages.TextMessage textMes)
        {
            Dispatcher.Invoke(() =>
            {
                SetInUserTalkMessageLastMessage(user.Login, textMes.Text);
            });
        }

        public void SetMessageToUserTalkControl(UserTalkMessage control, string message)
        {
            control.LastMessage.Text = message;
        }

        public void ClearChatAction(TelegramLib.MainClasses.User user)
        {
            Dispatcher.Invoke(() =>
            {
                SetInUserTalkMessageLastMessage(user.Login, "No messages");
            });
        }

        public void SetInUserTalkMessageLastMessage(string userLogin, string message)
        {
            //Find correct item in chat box
            Dispatcher.Invoke(() =>
            {
                ListBoxItem? item = GetChatControlItemByUserLogin(userLogin);

                if (item is null || item.Content is not UserTalkMessage mesControl) return;
                SetMessageToUserTalkControl(mesControl, message);
            });
        }

        public ListBoxItem? GetChatControlItemByUserLogin(string login)
        {
            return ChatsBox.Items
            .OfType<ListBoxItem>()
            .FirstOrDefault(x => x.Content is UserTalkMessage control &&
                control.GetFriendName() == login);
        }

        public void AddedUserImage(TelegramLib.MainClasses.User user)
        {
            Dispatcher.Invoke(async () =>
            {
                ListBoxItem? boxItem = ChatsBox.Items
                    .OfType<ListBoxItem>()
                    .FirstOrDefault(x => x.Content is UserTalkMessage talkControl &&
                            talkControl.FriendLogin.Text == user.Name);

                if (boxItem is null || boxItem.Content is not UserTalkMessage talkControl) return;

                int.TryParse(boxItem.Tag.ToString(), out int chatId);

                TelegramLib.MainClasses.UserChat chat = await ApiService.GetChatByUserAndSenderId(_system.LoggedUser.Id, _system.Chats.First(x => x.Id == chatId).Chatter.Id);

                await SignalRHelperService.SetContactPhoto(user, chat,
                    talkControl.ImageIcon, talkControl.UserEllipseImage);
            });
        }

        private const int mediaSize = 85;
        public void SetSearchedParams(TelegramLib.Enums.Messages.MediaType type)
        {
            if (type == TelegramLib.Enums.Messages.MediaType.Unknown)
            {
                AllMediasElements.Children.Clear();
                //Set chats
            }
            else if (type == TelegramLib.Enums.Messages.MediaType.Image)
            {
                //set images
                SetAllImagesInPanel();
            }
            else if (type == TelegramLib.Enums.Messages.MediaType.Video)
            {
                SetVideosInPanel();
            }
        }

        private List<MediaAction> _mediasinSearhPanel;
        private List<string> _videoPaths;

        public void SetVideosInPanel()
        {
            AllMediasElements.Children.Clear();
            //Get paths for 
            _videoPaths = _system.GetAllVideoMessages().Select(x => x.MediaName).ToList();
            _searchGridImags.Clear();

            //Set preview image
            for (int i = 0; i < _videoPaths.Count; i++)
            {
                Image img = FilesAction.GetImagePreviewForVideo(_videoPaths[i]);

                img.Tag = _videoPaths[i];

                img.Stretch = Stretch.Fill;

                img.Width = mediaSize;
                img.Height = mediaSize;

                img.Margin = new Thickness(5);

                img.PreviewMouseDown += MediaVideos_PreviewMouseDown;

                img.MouseEnter += SearchMedia_MouseEnter;
                img.MouseLeave += SearchMedia_MouseLeave;

                _searchGridImags.Add(img);

                AllMediasElements.Children.Add(img);
            }
        }

        public void MediaVideos_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img ||
                img.Tag is not string tag) return;

            MediaElement videoElement = FilesAction.GetMediaElementByVideoName(tag);

            _videoPaths = FilesAction.GetFullPathForVideos(_videoPaths);

            //SetVideo Paths
            VisualActionPage page = new VisualActionPage(videoElement, _videoPaths);

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);

            List<MediaAction> videos = _system.GetAllVideoMessages();

            int chosenVideoIndex = GetImageIndex(img);// _videoPaths.IndexOf(tag);

            page.SetUserChat(_system, videos, chosenVideoIndex, null);
        }


        public int GetImageIndex(Image img)
        {
            return AllMediasElements.Children.IndexOf(img);
        }

        private List<Image> _searchGridImags = new List<Image>();
        private void SetAllImagesInPanel()
        {
            AllMediasElements.Children.Clear();
            _mediasinSearhPanel = _system.GetAllImageMessages();
            _searchGridImags = new List<Image>();
            for (int i = 0; i < _mediasinSearhPanel.Count; i++)
            {
                if (!FilesAction.IsUserChatMediaIsExist(_mediasinSearhPanel[i].MediaName)) continue;

                Image img = FilesAction.GetImageFromChatImageFolder(_mediasinSearhPanel[i].MediaName);

                img.Width = mediaSize;
                img.Height = mediaSize;

                img.Margin = new Thickness(5);

                img.PreviewMouseDown += MediaImages_PreviewMouseDown;

                img.MouseEnter += SearchMedia_MouseEnter;
                img.MouseLeave += SearchMedia_MouseLeave;

                _searchGridImags.Add(img);
                AllMediasElements.Children.Add(img);
            }
        }

        public void MediaImages_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img) return;

            VisualActionPage page = new VisualActionPage(img, _searchGridImags);

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);

            page.SetUserChat(_system, _mediasinSearhPanel, _searchGridImags.IndexOf(img), null);
        }

        public void SearchMedia_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        public void SearchMedia_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public async Task SetActiveChats()
        {
            if (ChatsBox.Visibility != Visibility.Visible) ChatsBox.Visibility = Visibility.Visible;
            for (int i = 0; i < _system.Contacts.Count; i++)
            {
                SetUserChat(_system.Contacts[i].UserName);
            }
            await UpdateUserChatsPanel();
        }

        public void SetUserImage()
        {
            string path = FilesAction.GetUserImagePath(_system.LoggedUser.GetFirstImageName().Name);
            UserImage.ImageSource = new BitmapImage(new Uri(path, UriKind.Absolute));
        }

        public void SetChatClick()
        {
            UserChat.FindMessageBut.PreviewMouseDown += Magnifier_PreviewMouseDown;
            LeftButtons.HamburgMenu.PreviewMouseDown += UpdateUserImage_MouseDown;
        }

        private void UpdateUserImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUserImage();
        }

        private void Magnifier_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideAllChatBlocks();
            SearchMessageGrid.Visibility = Visibility.Visible;

            SearchMessage.SetUserImage(_system.LoggedUser.GetFirstImageNameInString());

            _chosenChat = _system.GetChosenChat();
        }

        private void SarchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchMessageGrid.Visibility == Visibility.Visible)
            {
                SetMessagesForSearch();
            }
        }

        public void SetMessagesForSearch()
        {
            SearchedMessageslistBox.Items.Clear();

            NothingFoundSearch.Visibility = Visibility.Hidden;

            List<TextMessage> messages = _chosenChat.GetMessagesWithGivenText(SarchBox.Text);

            for (int i = 0; i < messages.Count; i++)
            {
                //If sender is null, check logged user
                UserContactcs sender = _system.GetContactById(messages[i].SenderId);

                UserTalkMessage message = new UserTalkMessage(sender.GetFirstImageName().Name)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                if (sender is not null) message.FriendLogin.Text = sender.Name;
                else if (_system.IsUserIsSameId(i) is not null) message.FriendLogin.Text = _system.LoggedUser.Name;

                message.LastMessage.Text = messages[i].Text;
                message.LastMessageTime.Text = messages[i].GetSentTimeInString();

                //SET ICON HERE 
                System.Windows.Controls.ListBoxItem item =
                    new System.Windows.Controls.ListBoxItem()
                    {
                        Content = message,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                item.PreviewMouseDown += SearchedMessage_PreviewMouseDown;
                SearchedMessageslistBox.Items.Add(item);
            }

            if (SearchedMessageslistBox.Items.Count == 0)
            {
                NothingFoundSearch.Visibility = Visibility.Visible;
            }
        }

        public void SearchedMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Controls.ListBoxItem item ||
                item.Content is not UserTalkMessage message) return;

            //Find Message index in chat
            //Scroll to this message
            //Show bgs for 2 seconds

            //message index
            int? messIndex =
                _chosenChat.GetMessageIndexByText(message.GetLastMessageText());
            if (messIndex is null) return;

            //scroll to the message
            UserChat.ScrollToChosenItem((int)messIndex);
        }

        public void HideAllChatBlocks()
        {
            ChatsBox.Visibility = Visibility.Hidden;
            SearchBoxGrid.Visibility = Visibility.Hidden;
            SearchMessageGrid.Visibility = Visibility.Hidden;
            NothingFoundSearch.Visibility = Visibility.Hidden;

            SarchBox.Text = string.Empty;
        }

        public void SetMessageGridMagnifier()
        {
            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;
        }

        private void LeftButtons_OnMenuClick(object sender, EventArgs e)
        {
            UserChat.Visibility = Visibility.Hidden;
            ChosoeChatBorder.Visibility = Visibility.Visible;
            DrawerHost.OpenDrawerCommand.Execute(Dock.Left, MainDrawerHost);
        }

        private void SetDrawButsStyles()
        {
            ClearTextBut.IconType.Kind = PackIconKind.Close;

            MyProfileDrawBut.IconType.Kind = PackIconKind.AccountCircleOutline;
            MyProfileDrawBut.ButName.Text = "My Profile";

            WalletDrawBut.IconType.Kind = PackIconKind.AccountBalanceWalletOutline;
            WalletDrawBut.ButName.Text = "Wallet";

            NewGroupDrawBut.IconType.Kind = PackIconKind.Users;
            NewGroupDrawBut.ButName.Text = "New Group";

            NewChannelDrawBut.IconType.Kind = PackIconKind.Megaphone;
            NewChannelDrawBut.ButName.Text = "New Channel";

            ContactsDrawBut.IconType.Kind = PackIconKind.Account;
            ContactsDrawBut.ButName.Text = "Contacts";

            CallsDrawBut.IconType.Kind = PackIconKind.TelephoneInTalk;
            CallsDrawBut.ButName.Text = "Calls";

            //SavedMessagesDrawBut.IconType.Kind = PackIconKind.ContentSaveOutline;
            //SavedMessagesDrawBut.ButName.Text = "Saved Messages";

            SettingsDrawBut.IconType.Kind = PackIconKind.SettingsOutline;
            SettingsDrawBut.ButName.Text = "Settings";
        }

        private void LeftBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is MenuIconTextBut icon)
            {
                Page page = GetPageByIcon(icon);

                if (page is MainContacts mainContact) SetMainContactEvents(mainContact);
                else if (page is null) return;

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
            }
        }

        public void SetMainContactEvents(MainContacts mainContacts)
        {
            mainContacts.ContactClicked += ContactsChatChosen_PreviewMouseDown;
        }

        private UserTalkMessage _chosenChatControl;
        private async void ContactsChatChosen_PreviewMouseDown(object sender, EventArgs e)
        {
            if (ChatsBox.Visibility != Visibility.Visible) ChatsBox.Visibility = Visibility.Visible;
            if (sender is not UserContact userControl) return;

            SetUserChat(userControl.UserLogin.Text);
            await UpdateUserChatsPanel();
        }

        public void SetUserChat(string userLogin)
        {
            //SET PAGE FILLING
            // Set chatter page
            _system.SetTempChatter(userLogin);
            //Check isf set
            if (!_system.IsChatterIsSet()) return;

            ChosoeChatBorder.Visibility = Visibility.Hidden;
            UserChat.Visibility = Visibility.Visible;

            //Set chat into
            UserChat.SetUserChat(_system.GetUserChatByChatterName(
                _system.ChosenChatContact.Name));
        }

        public Page? GetPageByIcon(MenuIconTextBut icon)
        {
            return icon.Name == MyProfileDrawBut.Name.ToString() ? new LoggedUserProfile(_system.LoggedUser, _system) :
                icon.Name == ContactsDrawBut.Name.ToString() ? new Contacts.MainContacts(Enums.ContactsPageAction.AddContact, _system, false) :
                icon.Name == SettingsDrawBut.Name.ToString() ? new Settings.SettingsPage(_system) : null;
        }

        private void ChatsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetChatsWidth();
        }

        public void SetChatsWidth()
        {
            for (int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is System.Windows.Controls.ListBoxItem item &&
                    item.Content is UserTalkMessage message)
                {
                    message.Width = ChatsGrid.ActualWidth;
                }
            }
        }

        private void UserChat_PreviewRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Controls.ListBoxItem item ||
                item.Content is not UserTalkMessage talkControl) return;

            //Set UserControl

        }

        private void UserChat_PreviewLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Controls.ListBoxItem item ||
                item.Content is not UserTalkMessage talkControl) return;

            ShowChatControl();

            //Set temp background
            //(if chats is unset but general been changed)

            //_system.SetGeneralBgToChatsBg();

            UserChat.SetUserChat(
                _system.GetUserChatByChatterName(talkControl.FriendLogin.Text));

            SetSizerActionWithUserChatMouseDown();
        }

        private void UserChat_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }
        private void UserChat_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void ShowChatControl()
        {
            ChosoeChatBorder.Visibility = Visibility.Hidden;
            UserChat.Visibility = Visibility.Visible;
        }

        public void SetChat(UserTalkMessage chat)
        {
            //Get chat obj from db(nah, another shit should be here)
            //Set chat params
        }

        public void SetUserChatBg()
        {
            UserChat.SetBackground();
            SetNoChatBg();
        }

        public void SetNoChatBg()
        {
            NotFoundBg.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(
                    new Uri(FilesAction.GetWallpaperPathByName
                    (_system.Settings.GetChatSettings().Wallpaper.WallpaperName),
                    UriKind.Absolute)), // или Relative
                Stretch = Stretch.UniformToFill
            };

            if (_system.Settings.GetChatSettings().Wallpaper.IsBlurred)
            {
                NotFoundBg.Effect = new BlurEffect()
                {
                    Radius = 20
                };
                return;
            }
            NotFoundBg.Effect = null;
        }

        private void SavedMessagesDrawBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set Saved Messages chat control
        }

        private void GridSplitter_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Point mousePos = Mouse.GetPosition(Application.Current.MainWindow);
            double desired = ChatsColumn.ActualWidth + e.HorizontalChange;

            if (mousePos.X < 300)
            {
                //hide all chat textBlocks except UserImage
                HideAllChatBlocks();

                ChatsColumn.MinWidth = 75;
                ChatsColumn.Width = new GridLength(75);
                SetVisibilityForChatObjects(true);

                ChatsBox.Visibility = Visibility.Visible;

                FolderSliderRow.Height = new GridLength(0);
            }
            else
            {
                double clamped = Math.Max(100, desired);
                ChatsColumn.Width = new GridLength(clamped);
                SetVisibilityForChatObjects(false);
                SearchChatBut.Visibility = Visibility.Visible;

                FolderSliderRow.Height = _system.Settings.IsTabsOnTheLeft ? new GridLength(0) :
                     new GridLength(_folderSliderHeight);
            }

            SetSearchPanelWidth();
        }

        public void SetSearchPanelWidth()
        {
            if (SearchBoxGrid.Visibility == Visibility.Hidden) return;

            SearchControl.SetControlSize();
        }

        private void SetVisibilityForChatObjects(bool isShort)
        {
            SetChatInfoVisibility(isShort);
            SetSearchLineVisibility(isShort);
        }

        public void SetSearchLineVisibility(bool isShort)
        {
            Visibility magniVis = isShort ? Visibility.Visible : Visibility.Hidden;

            MagnifierGrid.Visibility = magniVis;
            SearchBorder.Visibility = magniVis == Visibility.Visible ?
                Visibility.Hidden : Visibility.Visible;
        }

        public void SetChatInfoVisibility(bool isShort)
        {
            Visibility vis = isShort ? Visibility.Hidden : Visibility.Visible;

            foreach (var mes in ChatsBox.Items)
            {
                if (mes is not UserTalkMessage message) return;
                message.InfoGrid.Visibility = vis;
            }
        }

        private void MagnifierGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            SearchChatBut.Foreground = Brushes.White;
        }

        private void MagnifierGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            SearchChatBut.Foreground = Brushes.Gray;
        }

        private void SarchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderSliderRow.Height = new GridLength(0);
            if (SearchMessageGrid.Visibility != Visibility.Visible) SetSearchBoxVisible();
            CrossSearchColumn.Width = new GridLength(40);
        }

        public void SetSearchBoxVisible()
        {
            HideAllChatBlocks();
            SearchBoxGrid.Visibility = Visibility.Visible;

            SearchControl.SetContacts(_system);
            ChatsColumn.MinWidth = 300;
            SearchControl.UpdateColors();
        }

        private void SarchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchMessageGrid.Visibility != Visibility.Visible) HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;
            ChatsColumn.MinWidth = 50;
            CrossSearchColumn.Width = new GridLength(0);

            if (!_system.Settings.IsTabsOnTheLeft)
                FolderSliderRow.Height = new GridLength(_folderSliderHeight);
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            return;
            var focusedEl = Keyboard.FocusedElement;

            if (focusedEl is System.Windows.Controls.TextBox box
                && box.Name == SarchBox.Text)
            {

            }
            bool ifFoc = SarchBox.IsFocused;
        }

        private void ClearTextBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SarchBox.Text = string.Empty;

            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;

            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(SarchBox), null);
            Keyboard.ClearFocus();
        }

        public async Task UpdateUserChatsPanel()
        {
            ChatsBox.Items.Clear();
            for (int i = 0; i < _system.Chats.Count(); i++)
            {
                System.Windows.Controls.ListBoxItem item = new
                    System.Windows.Controls.ListBoxItem()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = _system.Chats[i].Id
                };

                item.Content = await GetTalkMessage(i);

                item.PreviewMouseLeftButtonDown += UserChat_PreviewLeftMouseDown;
                item.PreviewMouseRightButtonDown += TalkMessage_PreviewRightMouseDown;
                item.MouseEnter += UserChat_MouseEnter;
                item.MouseLeave += UserChat_MouseLeave;
                ChatsBox.Items.Add(item);
            }
        }

        public async Task<UserTalkMessage> GetTalkMessage(int chatIndex)
        {
            TelegramLib.MainClasses.UserChat chat =
                _system.GetChatByIndex(chatIndex);


            TelegramLib.MainClasses.User chatterUser = await ApiService.GetUserById(chat.Chatter.ContactUserId);
            string imageName = await SignalRHelperService.GetUserPhotoToSet(chatterUser);

            UserTalkMessage chatControl = new UserTalkMessage(imageName /*chat.GetChatter().GetFirstImageName().Name*/)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = ChatsGrid.ActualWidth
            };

            chatControl.FriendLogin.Text = chat.GetChatter().Name;

            DateTime? date = chat.GetLastMessageDateTime();

            if (date is not null) chatControl.LastMessageTime.Text =
                    $"{((DateTime)date).Day}.{((DateTime)date).Month}.{((DateTime)date).Year}";
            chatControl.LastMessage.Text = chat.GetLastMessage();

            //Set Image icon

            /*      
                   await SignalRHelperService.SetContactPhoto(chatterUser, chat, chatControl.ImageIcon, chatControl.UserEllipseImage);
      */
            return chatControl;
        }

        public void ClearChosenUserTalkValue()
        {
            UserTalkMessage message =
                GetChtControlByChatterName(_system.GetChosenChat().Chatter.Name);
            if (message is null) return;

            message.SetDefaultValues();
        }

        public void UpdateUserTalkChat()
        {
            //Check with preload messages + chats

            //Get User talk control(by system control)
            //Get by chosen chat in system 
            UserTalkMessage message =
                GetChtControlByChatterName(_system.GetChosenChat().Chatter.Name);
            if (message is null) return;

            //Get chat
            message.LastMessage.Text =
                _system.GetChosenChat().GetLastMessage();

            message.LastMessageTime.Text =
                _system.GetChosenChat().Messages.Last().GetSentTimeInString();
        }

        public UserTalkMessage GetChtControlByChatterName(string name)
        {
            for (int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is System.Windows.Controls.ListBoxItem item &&
                    item.Content is UserTalkMessage message &&
                    message.GetFriendName() == name)
                {
                    return message;
                }
            }
            return null;
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SarchBox.Text = string.Empty;
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(SarchBox), null);
                Keyboard.ClearFocus();

                if (SearchBoxGrid.Visibility == Visibility.Visible)
                {
                    SearchControl.SetContacts(_system);
                    HideAllChatBlocks();
                    ChatsBox.Visibility = Visibility.Visible;
                }
                else if (SearchMessageGrid.Visibility == Visibility.Visible)
                {
                    HideAllChatBlocks();
                    ChatsBox.Visibility = Visibility.Visible;
                }
                else if (UserChat.Visibility == Visibility.Visible)
                {
                    UserChat.Visibility = Visibility.Hidden;
                    ChosoeChatBorder.Visibility = Visibility.Visible;
                }
            }
        }

        public void SetChosenFolder(TelegramLib.MainClasses.FolderObjs.Folder chosenFolder)
        {
            ChatsBox.Items.Clear();
            foreach (UserContactcs contact in chosenFolder.Contacts)
            {
                TelegramLib.MainClasses.UserChat chat =
                    _system.GetUserChatByChatterName(contact.Name);

                UserTalkMessage message = new UserTalkMessage(chat.GetChatter().GetFirstImageName().Name)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Width = ChatsGrid.ActualWidth
                };

                message.FriendLogin.Text = chat.GetChatter().Name;

                DateTime? date = chat.GetLastMessageDateTime();

                if (date is not null) message.LastMessageTime.Text =
                        $"{((DateTime)date).Day}.{((DateTime)date).Month}.{((DateTime)date).Year}";
                message.LastMessage.Text = chat.GetLastMessage();

                System.Windows.Controls.ListBoxItem item =
                     new System.Windows.Controls.ListBoxItem()
                     {
                         Content = message
                     };

                item.PreviewMouseLeftButtonDown += UserChat_PreviewLeftMouseDown;
                item.PreviewMouseRightButtonDown += TalkMessage_PreviewRightMouseDown;
                item.MouseEnter += UserChat_MouseEnter;
                item.MouseLeave += UserChat_MouseLeave;
                ChatsBox.Items.Add(item);
            }

            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;
        }

        public void TalkMessage_PreviewRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem boxItem) return;
            if (boxItem.Content is not UserTalkMessage targetElement) return;


            //Get Position point
            Point relativePoint =  e.GetPosition(this);

            //Get point to menu
            Point point = new Point(relativePoint.X /*+ targetElement.ActualWidth + 5*/, relativePoint.Y - 15);

            AddMenuElement(new UserChatMenu(), point);
        }

        public void AddSubMenu(ToAddSubMenuType type, Point enteredItemPoint)
        {
            //To show subMenu

            //Get cord of main sub menu
            Point point = GetCordOfMainMenu();
        
            //Set subMenu pooint
            Point subMenuPoint = new Point(point.X + GetMenuWidth(), point.Y + enteredItemPoint.Y);

            //Add new SubMenu
            UserChatMenu subMenu = new UserChatMenu();
            subMenu.SetSubMenu(type);

            AddMenuElement(subMenu, subMenuPoint);
        }

        public double GetMenuWidth()
        {
            UserChatMenu? menu =
                MenusCan.Children.OfType<UserChatMenu>().FirstOrDefault();

            return menu is null ? 100 : menu.ActualWidth;
        }

        public Point GetCordOfMainMenu()
        {
            UserChatMenu? menu =  
                MenusCan.Children.OfType<UserChatMenu>().FirstOrDefault();

            return menu is null ? new Point() : 
                new Point(Canvas.GetLeft(menu), Canvas.GetTop(menu));
        }

        public void AddMenuElement(UserChatMenu menu, Point cordPoint)
        {
            MenusCan.Children.Add(menu);

            Canvas.SetLeft(menu, cordPoint.X);
            Canvas.SetTop(menu, cordPoint.Y);
        }

        public void ClearMenusCanvas()
        {
            MenusCan.Children.Clear();
        }


        public void UpdateFolders()
        {
            LeftButtons.UpdateFolders();
        }

        private void MagnifierGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set variants of action(depends from which IconKind is it)

            ChatsColumn.Width = new GridLength(300);
            SetVisibilityForChatObjects(false);
            //Change border size   
            SetSearchBoxVisible();
        }

        public void UpdateTabsPlacement()
        {
            ChangeLeftButsVisState(_system.Settings.IsTabsOnTheLeft);
        }

        private const int _leftButWidth = 100;
        private const int _hamburgIconWidth = 65;
        private const int _folderSliderHeight = 25;

        public void ChangeLeftButsVisState(bool isShow)
        {
            //Left buttins width
            LeftButtonsColumn.Width = isShow ? new GridLength(_leftButWidth) :
                new GridLength(0);

            //hamburger icon height
            AddHamburgMenuCol.Width = !isShow ? new GridLength(_hamburgIconWidth) :
                new GridLength(0);

            //Slider vis size
            FolderSliderRow.Height = !isShow ? new GridLength(_folderSliderHeight) :
                new GridLength(0);

            //Setting folders
            if (!isShow)
                FolderSliderMenu.SetSliderWithFolders(_system.Folders, _system);
        }

        private void HamburgerAddGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            HamburgIcon.Foreground = Brushes.White;
        }

        private void HamburgerAddGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            HamburgIcon.Foreground = Brushes.Gray;
        }

        public void SetWindowSizerAction(bool isToClearFromPrevLevel = false)
        {
            SizerActionType? type =
                ((MainWindow)Window.GetWindow(this)).GetWindowSizeType();

            if (isToClearFromPrevLevel) ClearPrevSizerChanges(type);
            /*            if (((MainWindow)Window.GetWindow(this)).IsWindowIsMaxSize() &&
                            ((MainWindow)Window.GetWindow(this)).GetMaxState())
                        {
                            ClearAllLevels();
                        }*/
            switch (type)
            {
                case null:
                    {
                        return;
                    }
                case SizerActionType.FirstLevel:
                    {
                        //The most window level
                        //clear every modified param
                        UserChat.SetMessagesPosition(true);

                        return;
                    }
                case SizerActionType.SecondLevel:
                    {
                        //in active user chat messages should be
                        //glued on difference siders
                        if (UserChat.Visibility == Visibility.Hidden) return;

                        UserChat.SetMessagesPosition(false);
                        return;
                    }
                case SizerActionType.ThirdLevel:
                    {
                        SetThirdLevel();
                        return;
                    }
                case SizerActionType.FourthLevel:
                    {
                        _system.Settings.IsTabsOnTheLeft = false;
                        ChangeLeftButsVisState(false);
                        SetClosingChatColumn();
                        return;
                    }
            }
        }

        public void ClearPrevSizerChanges(Enums.SizerActionType? tempSizeType)
        {
            if (tempSizeType is null ||
                tempSizeType == SizerActionType.FourthLevel) return;

            if (tempSizeType == SizerActionType.ThirdLevel)
            {
                ClearThirdLevelState();
            }
            else if (tempSizeType == SizerActionType.SecondLevel)
            {
                ClearSecondLevelState();
            }
            else if (tempSizeType == SizerActionType.FirstLevel)
            {
                //Set glued pos
                UserChat.SetMessagesPosition(true);
            }
        }

        public void ClearAllLevels()
        {
            ClearThirdLevelState();
            ClearSecondLevelState();
            UserChat.SetMessagesPosition(true);
        }

        private void ClearThirdLevelState()
        {
            _system.Settings.IsTabsOnTheLeft = true;
            ChangeLeftButsVisState(true);

            ChatsColumn.Width = new GridLength(1, GridUnitType.Star);
            ChatColumn.Width = new GridLength(0);
        }

        private void ClearSecondLevelState()
        {
            //Set min width for chats column
            SetColumnWidth(ChatsColumn, 450);

            //Set grid splitter width
            SetColumnWidth(GridSplitterColumn, 3);

            //Set chat column width
            ChatColumn.Width = new GridLength(1, GridUnitType.Star);

            //Set back button on user chat
            UserChat.SetVisibilityToBackBut(false);
        }

        public void SetThirdLevel()
        {
            //If chosen chat is exist => ChatColumn.Width = 0
            //else UserChatColumn.Width = 0 
            if (UserChat.Visibility == Visibility.Visible)
            {
                //Hide chats column 
                SetColumnWidth(ChatsColumn, 0);

                //Show Back but on user chat
                UserChat.SetVisibilityToBackBut(true);

                //Set gridSplitter column width
                SetColumnWidth(GridSplitterColumn, 0);

                //Set User Chat Column Width
                /*                double newUserChatColumn = this.ActualWidth - LeftButtons.ActualWidth;
                                SetColumnWidth(ChatColumn, newUserChatColumn);*/
            }
            else
            {
                //Hide Back Button
                UserChat.SetVisibilityToBackBut(false);

                //Set gridSplitter column width
                SetColumnWidth(GridSplitterColumn, 0);

                // Set UserChat column as Zero
                SetColumnMinWidth(ChatColumn, 0);
                SetColumnWidth(ChatColumn, 0);

                ChatsColumn.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        public void BackButton_MouseDown()
        {
            // Clear chosen chat
            _chosenChatControl = null;
            _chosenChat = null;
            ChosoeChatBorder.Visibility = Visibility.Visible;
            UserChat.Visibility = Visibility.Hidden;

            // Set UserChat column as Zero
            SetColumnMinWidth(ChatColumn, 0);
            SetColumnWidth(ChatColumn, 0);

            //Set Splitter grid as zero
            SetColumnWidth(GridSplitterColumn, 0);

            // Set Chats column Width as a star
            ChatsColumn.Width = new GridLength(1, GridUnitType.Star);
        }

        public void SetSizerActionWithUserChatMouseDown()
        {
            SizerActionType? type =
                ((MainWindow)Window.GetWindow(this)).GetWindowSizeType();

            if (type is null) return;


            if (type == SizerActionType.ThirdLevel)
            {
                UserChat.SetVisibilityToBackBut(true);

                //Set chatS column.Width = 0
                SetColumnMinWidth(ChatsColumn, 0);
                SetColumnWidth(ChatsColumn, 0);

                //Set TempChat column.Width = Star
                ChatColumn.Width = new GridLength(1, GridUnitType.Star);
            }
            else if (type == SizerActionType.FourthLevel)
            {
                UserChat.SetVisibilityToBackBut(true);

                //Set chatS column.Width = 0
                SetColumnMinWidth(ChatsColumn, 0);
                SetColumnWidth(ChatsColumn, 0);

                //Set leftButtons column.Width = 0
                SetColumnMinWidth(LeftButtonsColumn, 0);
                SetColumnWidth(LeftButtonsColumn, 0);

                //Set Grid Splitter column.Width = 0
                SetColumnMinWidth(GridSplitterColumn, 0);

                //Set TempChat column.Width = Star
                ChatColumn.Width = new GridLength(1, GridUnitType.Star);

                SetColumnWidth(ChatsColumn, 0);

                Console.WriteLine(ChatColumn.ActualWidth);
                Console.WriteLine(ChatsColumn.ActualWidth);
                Console.WriteLine(LeftButtonsColumn.ActualWidth);
            }
        }

        public void SetColumnMinWidth(ColumnDefinition column, double minWidth)
        {
            column.MinWidth = minWidth;
        }

        public void SetColumnWidth(ColumnDefinition column, double width)
        {
            column.Width = new GridLength(width);
        }

        public void SetClosingChatColumn()
        {
            UserChat.Visibility = Visibility.Hidden;
            ChatColumn.MinWidth = 0;
            ChatColumn.Width = new GridLength(0);

            ChatsColumn.Width = new GridLength(1, GridUnitType.Star);
            //this.ActualWidth - LeftButtonsColumn.Width.Value - GridSplitterColumn.Width.Value);

            // ChatsColumn.MaxWidth = ChatsColumn.Width.Value;
        }

       

    }
}
