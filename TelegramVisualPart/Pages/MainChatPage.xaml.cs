using FFMpegCore;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Xaml.Behaviors.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Xml.Linq;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramVisualPart.CustWindows;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Enums.Menus;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.ChatActions;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls;
using TelegramVisualPart.UserControls.ChatsControls;
using TelegramVisualPart.UserControls.ChatsSearch;
using TelegramVisualPart.UserControls.ContactsControls;
using TelegramVisualPart.UserControls.DifferButs;
using TelegramVisualPart.UserControls.MainPage.LittleMainControls;
using static MaterialDesignThemes.Wpf.Theme;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ListBoxItem = System.Windows.Controls.ListBoxItem;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainChatPage.xaml
    /// </summary>
    public partial class MainChatPage : Page
    {
        private TelSystem _system;
        public TelegramLib.MainClasses.UserChat _chosenChat;

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

            UpdateTabsPlacement();
            SetLangText();

            SetEvents();
        }

        public void SetEvents()
        {
            SignalRService.UpdateReadStatus += UserChat.UpdateReadStatus;
        }

        public void SetLangText()
        {
            //VisConstParamsJsonService.SetFileName("EnglishLang.json");
            SetLanguageText.SetMainChatPageParams(this);
        }

        public async Task SetBasicParams()
        {
            /*            LeftButtons.OnMenuClick += LeftButtons_OnMenuClick;
                        LeftButtons.OnMenuClick += LeftButtons_OnMenuClick;*/

            SetDrawButsStyles();

            SetChatClick();

            LeftButtons.SetSystemParam(_system);

            UserChat.SetSystemAndMainWindowParam(_system, (MainWindow)Window.GetWindow(this));
            UserChat.BackButton_MouseDown += BackButton_MouseDown;

            SetUserImage();

            //await UpdateUserChatsPanel();

            SetNoChatBg();

            /*await */
            SetActiveChats();

            SetColorToSettingsButs();

            SliderLogin.Text = _system.LoggedUser.Login;

            SearchControl.SetSearchType += SetSearchedParams;

            SignalRService.UpdateUserImage += AddedUserImage;
            SignalRService.UpdateContactPhotoDel += AddedUserImage;

            //Check with last message
            SignalRService.ClearChatDel += ClearChatAction;

            SignalRService.TextMessageReceived += SentTextMessage;
            SignalRService.MediaMessageReceived += SetMediaMessage;
            SignalRService.DeleteChat += DeleteChat;
            SignalRService.SetShareContactMessage += AddShareContactMesInDb;
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

                TelegramLib.MainClasses.UserChat chat =
                await ApiService.GetChatByUserAndSenderId(_system.LoggedUser.Id,
                _system.Chats.First(x => x.Id == chatId).Chatter.Id);

                await SignalRHelperService.SetContactPhoto(user, chat,
                    talkControl.ImageIcon, talkControl.UserEllipseImage);

                //Change 
                await UserChat.SetStopMessageForChatter();
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
            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;

            /*            for (int i = 0; i < _system.Contacts.Count; i++)
                        {
                            SetUserChat(_system.Contacts[i].Login);
                        }*/
            await RepaintUserChatsPanel();
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

            UserChat.TurnOnLoopState();

            SearchMessage.SetUserImage(_system.LoggedUser.GetFirstImageNameInString());

            _chosenChat = _system.GetChosenChat();
        }

        private void SarchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*if (SearchMessageGrid.Visibility == Visibility.Visible)
            {*/
            SetChatPanelsVisibility();

            SetMessagesForSearch();
            //}
        }

        public void SetChatPanelsVisibility()
        {
            if (SarchBox.Text != string.Empty)
            {
                SearchBoxGrid.Visibility = Visibility.Hidden;

                //is User chat is Visible + is loop is pressed

                if (UserChat.Visibility == Visibility.Visible &&
                   UserChat.GetLoopState())
                {
                    if (SarchBox.Text == string.Empty) UserChat.TurnOfLoopState();
                    SearchMessageGrid.Visibility = Visibility.Visible;
                    GlobalMessageSearch.Visibility = Visibility.Hidden;
                }
                else
                {
                    GlobalMessageSearch.Visibility = Visibility.Visible;
                    SearchMessageGrid.Visibility = Visibility.Hidden;
                    UserChat.TurnOfLoopState();
                }
            }
        }

        public void SetMessagesForSearch()
        {
            SearchedMessageslistBox.Items.Clear();

            NothingFoundSearch.Visibility = Visibility.Hidden;

            //If Chosen chat is null -- all chats
            //else find from chosen chat

            if (GlobalMessageSearch.Visibility == Visibility.Visible)
            {
                //all chats
                SearchMessagesInAllChat();
            }
            else if (SearchMessageGrid.Visibility == Visibility.Visible)
            {
                SetSearchMessagesInChosenChat();

                if (SearchedMessageslistBox.Items.Count == 0)
                {
                    NothingFoundSearch.Visibility = Visibility.Visible;
                }
            }
        }

        public void SearchMessagesInAllChat()
        {
            GlobalMessageSearch.Items.Clear();

            //Set height for global search panel
            SetGlobalSearchListHeight();

            //Contacts 
            SetFoundGlobalChats();

            //Set found messages
            SetFoundGlobalMessages();
        }

        public void SetGlobalSearchListHeight()
        {
            GlobalMessageSearch.Height = Height -
                SearchBoxRow.Height.Value -
                FolderSliderRow.Height.Value;
        }

        public Dictionary<int, UserTalkMessage> _allContactDict = new Dictionary<int, UserTalkMessage>();
        public void SetFoundGlobalChats()
        {
            ChatsBox.Items.Clear();
            //_allContactDict - all contacts

            /* for (int i = 0; i < _system.Contacts.Count; i++)
             {
                 if (_allContactDict.ContainsKey(_system.Contacts[i].Id))
                 {
                     GlobalMessageSearch.Items.Add(_allContactDict[_system.Contacts[i].Id]);
                 }

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
                 //ChatsBox.Items.Add(item);
                 items.Add(item);

                 _chatsDict.TryAdd(_system.Chats[i].Id, item);

             }*/

            foreach (var pair in _chatsDict)
            {
                ListBoxItem item = pair.Value;
                if (item.Content is UserTalkMessage mes &&
                   GetContainedChat(mes))
                {
                    GlobalMessageSearch.Items.Add(item);
                }
            }
        }

        public bool GetContainedChat(UserTalkMessage mes)
        {
            return mes.FriendLogin.Text.Contains(SarchBox.Text);
        }

        public void SetFoundGlobalMessages()
        {
            List<(TextMessage, int)> messages =
                _system.GetMessagesChatIdFromChatsWithGivenSubChat(SarchBox.Text);

            //Sent amount of messages block 
            if (messages.Count > 0)
            {
                TotalFoundMessasges total = new TotalFoundMessasges(messages.Count);

                ListBoxItem totalItem = new ListBoxItem()
                {
                    Content = total,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };

                GlobalMessageSearch.Items.Add(totalItem);
            }

            //Messages
            for (int i = 0; i < messages.Count; i++)
            {
                TelegramLib.MainClasses.User sender =
                  _system.LoggedUser.Id == messages[i].Item1.SenderUserId ?
                  _system.LoggedUser :
                  _system.GetChatterById(messages[i].Item1.SenderUserId);

                UserTalkMessage message = new UserTalkMessage(
                    sender.GetFirstImageName().Name)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = messages[i].Item2
                };
                message.SetUnreadAmountVisibility(false);

                /*           if (sender is not null) message.FriendLogin.Text = sender.Name;
                           else if (_system.IsUserIsSameId(i) is not null) message.FriendLogin.Text = _system.LoggedUser.Name;
           */

                message.FriendLogin.Text = sender.Name;

                message.LastMessage.Text = messages[i].Item1.Text;
                message.LastMessageTime.Text = messages[i].Item1.GetSentTimeInString();

                //SET ICON HERE 
                System.Windows.Controls.ListBoxItem item =
                    new System.Windows.Controls.ListBoxItem()
                    {
                        Content = message,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                item.PreviewMouseDown += OpenChatInChosenMessage_PreviewMouseDown;
                item.MouseEnter += SearchMessage_MouseEnter;
                item.MouseLeave += SearchMessage_MouseLeave;


                message.SetUnreadAmountVisibility(false);
                GlobalMessageSearch.Items.Add(item);
            }
        }

        public void OpenChatInChosenMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Controls.ListBoxItem item ||
                item.Content is not UserTalkMessage message) return;

            //open chat
            TelegramLib.MainClasses.UserChat chat =
                _system.GetChatById(int.Parse(message.Tag.ToString()));


            //Set chat in UserChat
            chat.IsMarked = false;

            if (((MainWindow)Window.GetWindow(this)).ChatIsOnOtherWindow(chat))
            {
                //Set window on the front
                ((MainWindow)Window.GetWindow(this)).SetOtherChatWindowOnFront(chat);
                return;
            }
            UserChat.SetUserChat(chat);

            SetChosenChatValues(chat);

            ShowChatControl();
            SetSizerActionWithUserChatMouseDown();

            Action tempHandler = null;
            tempHandler = () =>
            {
                int? messIndex = chat.GetMessageIndexByText(message.GetLastMessageText());
                if (messIndex is null) return;

                UserChat.ScrollToChosenItem((int)messIndex);
                UserChat.SettingEnded -= tempHandler;
            };
            UserChat.SettingEnded += tempHandler;


            /*            UserChat.SettingEnded += () =>
                        {
                            //Scroll to chosen message
                            int? messIndex =
                                chat.GetMessageIndexByText(message.GetLastMessageText());
                            if (messIndex is null) return;

                            UserChat.ScrollToChosenItem((int)messIndex);
                        };*/
            //scroll to the message
            //UserChat.ScrollToChosenItem((int)messIndex);
        }

        public void SearchMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        public void SearchMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void SetSearchMessagesInChosenChat()
        {
            List<TextMessage> messages = _chosenChat.GetMessagesWithGivenText(SarchBox.Text);

            for (int i = 0; i < messages.Count; i++)
            {
                //If sender is null, check logged user
                TelegramLib.MainClasses.User sender =
                    _system.LoggedUser.Id == messages[i].SenderUserId ?
                    _system.LoggedUser :
                    _system.GetChatterById(messages[i].SenderUserId);

                UserTalkMessage message = new UserTalkMessage(
                    _chosenChat.Chatter.GetFirstImageName().Name)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                message.SetUnreadAmountVisibility(false);

                /*           if (sender is not null) message.FriendLogin.Text = sender.Name;
                           else if (_system.IsUserIsSameId(i) is not null) message.FriendLogin.Text = _system.LoggedUser.Name;
           */

                message.FriendLogin.Text = sender.Name;

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

        public void ShowChosenMessageByMessageId(int mesId)
        {
            UserChat.ScrollToMessageByMessageId(mesId);
        }

        public void HideAllChatBlocks()
        {
            ChatsBox.Visibility = Visibility.Hidden;
            SearchBoxGrid.Visibility = Visibility.Hidden;
            SearchMessageGrid.Visibility = Visibility.Hidden;
            NothingFoundSearch.Visibility = Visibility.Hidden;
            GlobalMessageSearch.Visibility = Visibility.Hidden;

            UserChat.TurnOfLoopState();
            SarchBox.Text = string.Empty;
        }

        public void SetMessageGridMagnifier()
        {
            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;
        }

        private void LeftButtons_OnMenuClick(object sender, EventArgs e)
        {
            //UserChat.Visibility = Visibility.Hidden;
            //ChosoeChatBorder.Visibility = Visibility.Visible;
            DrawerHost.OpenDrawerCommand.Execute(Dock.Left, MainDrawerHost);
        }

        private void SetDrawButsStyles()
        {
            ClearTextBut.IconType.Kind = PackIconKind.Close;

            MyProfileDrawBut.IconType.Kind = PackIconKind.AccountCircleOutline;
            //MyProfileDrawBut.ButName.Text = "My Profile";

            WalletDrawBut.IconType.Kind = PackIconKind.AccountBalanceWalletOutline;
            //WalletDrawBut.ButName.Text = "Wallet";

            NewGroupDrawBut.IconType.Kind = PackIconKind.Users;
            //NewGroupDrawBut.ButName.Text = "New Group";

            NewChannelDrawBut.IconType.Kind = PackIconKind.Megaphone;
            //NewChannelDrawBut.ButName.Text = "New Channel";

            ContactsDrawBut.IconType.Kind = PackIconKind.Account;
            //ContactsDrawBut.ButName.Text = "Contacts";

            CallsDrawBut.IconType.Kind = PackIconKind.TelephoneInTalk;
            //CallsDrawBut.ButName.Text = "Calls";

            //SavedMessagesDrawBut.IconType.Kind = PackIconKind.ContentSaveOutline;
            //SavedMessagesDrawBut.ButName.Text = "Saved Messages";

            SettingsDrawBut.IconType.Kind = PackIconKind.SettingsOutline;
            //SettingsDrawBut.ButName.Text = "Settings";
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
            if (ChatsBox.Visibility != Visibility.Visible)
            {
                ChatsBox.Visibility = Visibility.Visible;
            }
            if (sender is not UserContact userControl) return;

            SetUserChat(userControl.UserLogin.Text);
            await RepaintUserChatsPanel();
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
            UserChat.SetUserChat(_system.GetUserChatByChatterId(
                _system.ChosenChatContact.Id));
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
            SetChatParams(item, talkControl);
        }


        public void SetChatByChatterId(int userId)
        {
           //Get user chat
           TelegramLib.MainClasses.UserChat chat = 
                _system.GetChatByChatterId(userId);
            if (chat is null) return;

            //Get ListBoxItem (chat control)
            ListBoxItem chatItem = GetChatItemByChatId(chat.Id);
            if (chatItem is null ||
                chatItem.Content is not UserTalkMessage talkControl) return;

            SetChatParams(chatItem, talkControl);
        }

        public ListBoxItem? GetChatItemByChatId(int chatId)
        {
            return ChatsBox.Items
                .OfType<ListBoxItem>()
                .FirstOrDefault(x => x.Tag.ToString() == chatId.ToString());
        }

        public void SetChatParams(ListBoxItem item, UserTalkMessage talkControl)
        {
            SetChosenChatBg(item);
            int.TryParse(item.Tag.ToString(), out int id);

            talkControl.SetVisibilityToUnreadEllipse(false);

            TelegramLib.MainClasses.UserChat chat = _system.GetChatById(id);

            if (chat is null)
            {
                int.TryParse(talkControl.Tag.ToString(), out int userId);

                //Get contact
                UserContactcs contact = _system.GetContactByUserId(userId);
                if (contact is null) return;

                //Add chat if it was deleted 
                AddChat(contact);

                //change control Tag in new chat id
                chat = _system.Chats.Last();

                RepaintUserChatsPanel();
            }

            //_system.GetUserChatByChatterLogin(talkControl.FriendLogin.Text);
            chat.IsMarked = false;

            if (((MainWindow)Window.GetWindow(this)).ChatIsOnOtherWindow(chat))
            {
                //Set window on the front
                ((MainWindow)Window.GetWindow(this)).SetOtherChatWindowOnFront(chat);
                return;
            }
            UserChat.SetUserChat(chat);

            SetChosenChatValues(chat);

            ShowChatControl();
            SetSizerActionWithUserChatMouseDown();
        }

        public void SetChosenChatValues(TelegramLib.MainClasses.UserChat chat)
        {
            _chosenChat = chat;
            _system.ChosenChatContact = chat.Chatter;
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
            if (SearchMessageGrid.Visibility == Visibility.Hidden &&
                GlobalMessageSearch.Visibility == Visibility.Hidden) HideAllChatBlocks();

            if (SearchMessageGrid.Visibility == Visibility.Hidden &&
                GlobalMessageSearch.Visibility == Visibility.Hidden) ChatsBox.Visibility = Visibility.Visible;

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

        private Dictionary<int, ListBoxItem> _chatsDict = new Dictionary<int, ListBoxItem>();

        public async Task RepaintUserChatsPanel()
        {
            ChatsBox.Items.Clear();
            GlobalMessageSearch.Items.Clear();

            List<ListBoxItem> items = new List<ListBoxItem>();

            for (int i = 0; i < _system.Chats.Count(); i++)
            {
                if (_chatsDict.ContainsKey(_system.Chats[i].Id))
                {
                    _chatsDict.TryGetValue(_system.Chats[i].Id,
                        out ListBoxItem tempItem);

                    if (tempItem.Content is UserTalkMessage usTalk)
                        SetUnreadForUserTalk(usTalk, _system.Chats[i]);

                    items.Add(tempItem);
                    continue;
                }

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

                if (item.Content is UserTalkMessage mes)
                    SetUnreadForUserTalk(mes, _system.Chats[i]);

                items.Add(item);

                _chatsDict.TryAdd(_system.Chats[i].Id, item);
            }

            foreach (var item in items)
            {
                ChatsBox.Items.Add(item);
            }
            MarkStartFolderChat();
        }

        public void ClearAllChatsBgs()
        {
            for(int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is ListBoxItem item && 
                    item.Content is UserTalkMessage mes)
                {
                    item.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        public void SetChosenChatBg(ListBoxItem item)
        {
            //Clear bg for every chat
            ClearAllChatsBgs();

            //set bg for new chat
            var brush = (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
            item.Background = brush;

        }

        public void SetUnreadForUserTalk(UserTalkMessage mes,
            TelegramLib.MainClasses.UserChat chat)
        {
            if (mes is null || chat is null) return;
            mes.SetUnreadMessageValue(chat.GetAmountOfUnreadMessages(_system.LoggedUser.Id));
        }

        public void SetMessageInUserTalkControl(int chatId, string message)
        {
            if (!_chatsDict.ContainsKey(chatId)) return;
            _chatsDict.TryGetValue(chatId, out ListBoxItem? item);
            if (item is null ||
            item.Content is not UserTalkMessage talkMes) return;

            talkMes.LastMessageTime.Text =
               $"{DateTime.Now.Day.ToString()}.{DateTime.Now.Month.ToString()}.{DateTime.Now.Year.ToString()}";
            talkMes.LastMessage.Text = message;
        }

        public async Task<UserTalkMessage> GetTalkMessage(int chatIndex)
        {
            TelegramLib.MainClasses.UserChat chat =
                _system.GetChatByIndex(chatIndex);

            TelegramLib.MainClasses.User chatterUser = chat.Chatter;//  await ApiService.GetUserById(chat.Chatter.Id);
            string imageName = await SignalRHelperService.GetUserPhotoToSet(chatterUser);

            UserTalkMessage chatControl = new UserTalkMessage(imageName /*chat.GetChatter().GetFirstImageName().Name*/)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Width = ChatsGrid.ActualWidth,
                //Tag = chat.Id
                Tag = chatterUser.Id
            };

            chatControl.SetVisibilityToPinBlock(chat.IsPinned);
            chatControl.SetVisibilityToUnreadEllipse(chat.IsMarked);

            UserContactcs cont = _system.GetContactByUserId(chat.GetChatter().Id);
            chatControl.FriendLogin.Text = cont is null ? chat.GetChatter().Name : cont.Name;

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
            TelegramLib.MainClasses.UserChat chat = _system.GetChosenChat();
            if (chat is null) chat = ((MainWindow)Window.GetWindow(this)).GetOnlyChat();

            UserTalkMessage message =
                GetChtControlByChatterName(chat.Chatter.Name, chat.Id);
            if (message is null) return;

            message.SetDefaultValues();
        }

        public void UpdateUserTalkChat()
        {
            //Check with preload messages + chats

            //Get User talk control(by system control)
            //Get by chosen chat in system 

            if (IsOnlyChat()) return;

            TelegramLib.MainClasses.UserChat chat = _system.GetChosenChat();
            if (chat is null) chat = ((MainWindow)Window.GetWindow(this)).GetOnlyChat();
            if (chat is null) return;


            UserTalkMessage message =
                GetChtControlByChatterName(chat.Chatter.Name, chat.Id);

            if (message is null) return;

            //Get chat
            message.LastMessage.Text = chat.GetLastMessage();
            message.LastMessageTime.Text = chat.Messages.Last().GetSentTimeInString();

            SetUnreadForUserTalk(message, chat);
        }


        public bool IsOnlyChat()
        {
            if (((MainWindow)Window.GetWindow(this)).GetIsOnlyChat())
            {
                ((MainWindow)Window.GetWindow(this)).UpdateUserTalkChat();
                return true;
            }
            return false;
        }

        public UserTalkMessage GetChtControlByChatterName(string name, int chatId)
        {
            for (int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is System.Windows.Controls.ListBoxItem item &&
                    item.Content is UserTalkMessage message)
                {
                    int.TryParse(item.Tag.ToString(), out int id);
                    if (id == chatId) return message;
                }

                /*                if (ChatsBox.Items[i] is System.Windows.Controls.ListBoxItem item &&
                                    item.Content is UserTalkMessage message &&
                                    message.GetFriendName() == name)
                                {
                                    return message;
                                }*/
            }
            return null;
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (((MainWindow)Window.GetWindow(this)).GetIsOnlyChat()) return;

            if (e.Key == Key.Escape)
            {

                if (!((MainWindow)Window.GetWindow(this)).EscapePressed()) return;


                SarchBox.Text = string.Empty;
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(SarchBox), null);
                Keyboard.ClearFocus();

                //1 - clear search stuff (on chats box)
                //2 - clear chat
                //3 - Choose chat Box
                //4 - sett all chats

                EscLevels level = GetEscapeLevel();

                switch (level)
                {
                    case EscLevels.First:
                        {
                            SetFirstLEvelEscVisibility();
                            break;
                        }
                    case EscLevels.Second:
                        {
                            ChosoeChatBorder.Visibility = Visibility.Visible;
                            UserChat.Visibility = Visibility.Hidden;
                            break;
                        }
                    case EscLevels.Third:
                        {
                            break;
                        }
                    case EscLevels.Forth:
                        {
                            break;
                        }
                }

                if (UserChat.Visibility == Visibility.Hidden) ClearAllChatsBgs();

                /*                if (SearchBoxGrid.Visibility == Visibility.Visible)
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
                                }*/
            }
        }

        public EscLevels GetEscapeLevel()
        {
            return SearchBoxGrid.Visibility == Visibility.Visible ||
                GlobalMessageSearch.Visibility == Visibility.Visible ||
                SearchMessageGrid.Visibility == Visibility.Visible ||
                /*ChatsBox.Visibility == Visibility.Visible ||*/
                NothingFoundSearch.Visibility == Visibility.Visible ?

                EscLevels.First :

                UserChat.Visibility == Visibility.Visible ?

                EscLevels.Second :
                EscLevels.Third;
        }

        public void SetFirstLEvelEscVisibility()
        {
            SearchBoxGrid.Visibility = Visibility.Hidden;
            GlobalMessageSearch.Visibility = Visibility.Hidden;
            SearchMessageGrid.Visibility = Visibility.Hidden;
            NothingFoundSearch.Visibility = Visibility.Hidden;

            UserChat.TurnOfLoopState();
            ChatsBox.Visibility = Visibility.Visible;
        }

        public void SetChosenFolder(TelegramLib.MainClasses.FolderObjs.Folder chosenFolder)
        {
            ChatsBox.Items.Clear();
            foreach (TelegramLib.MainClasses.User contact in chosenFolder.Contacts)
            {
                TelegramLib.MainClasses.UserChat chat =
                    _system.GetUserChatByChatterId(contact.Id);

                UserTalkMessage message = new UserTalkMessage(chat.GetChatter().GetFirstImageName().Name)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Width = ChatsGrid.ActualWidth,
                    //Tag = chat.Id
                    Tag = chat.Chatter.Id
                };
               
                UserContactcs cont = _system.GetContactByUserId(chat.GetChatter().Id);
                message.FriendLogin.Text = cont is null ? chat.GetChatter().Name : cont.Name;

                DateTime? date = chat.GetLastMessageDateTime();

                if (date is not null) message.LastMessageTime.Text =
                        $"{((DateTime)date).Day}.{((DateTime)date).Month}.{((DateTime)date).Year}";
                message.LastMessage.Text = chat.GetLastMessage();

                System.Windows.Controls.ListBoxItem item =
                     new System.Windows.Controls.ListBoxItem()
                     {
                         Content = message,
                         Tag = chat.Id
                     };

                item.PreviewMouseLeftButtonDown += UserChat_PreviewLeftMouseDown;
                item.PreviewMouseRightButtonDown += TalkMessage_PreviewRightMouseDown;
                item.MouseEnter += UserChat_MouseEnter;
                item.MouseLeave += UserChat_MouseLeave;
                ChatsBox.Items.Add(item);
            }

            HideAllChatBlocks();
            ChatsBox.Visibility = Visibility.Visible;

            //Mark if chat Is open
            MarkStartFolderChat();
        }

        public void MarkStartFolderChat()
        {
            if (UserChat.Visibility == Visibility.Hidden)
            {
                ClearAllChatsBgs();
                return;
            }
            ListBoxItem? item = ChatsBox.Items
                .OfType<ListBoxItem>().FirstOrDefault(x => x.Tag.ToString() == UserChat._chat.Id.ToString());
            if (item is null) return;
            SetChosenChatBg(item);
        }

        private UserTalkMessage _menuChatterTalk = null;
        public void TalkMessage_PreviewRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem boxItem) return;
            if (boxItem.Content is not UserTalkMessage targetElement) return;

            _menuChatterTalk = targetElement;

            //Get Position point
            Point relativePoint = e.GetPosition(this);

            //Get point to menu
            Point point = new Point(relativePoint.X /*+ targetElement.ActualWidth + 5*/, relativePoint.Y - 15);


            int.TryParse(boxItem.Tag.ToString(), out int id);
            TelegramLib.MainClasses.UserChat chat = _system.GetChatById(id);

            /*TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();*/

            AddMenuElement(new UserChatMenu(chat, _system), point);
        }

        public void ClearMessageUSerTalk()
        {
            _menuChatterTalk = null;
        }

        public void AddSubMenu(ToAddSubMenuType type, Point enteredItemPoint)
        {
            //Get cord of main sub menu
            Point point = GetCordOfMainMenu();

            //Set subMenu pooint
            Point subMenuPoint = new Point(point.X + GetMenuWidth(), point.Y + enteredItemPoint.Y);

            TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();

            //Add new SubMenu
            UserChatMenu subMenu = new UserChatMenu(chat, _system);

            subMenu.SetWindow((MainWindow)Window.GetWindow(this));
            subMenu.SetSubMenu(type);

            AddMenuElement(subMenu, subMenuPoint);
        }

        public void ClearSubMenus()
        {
            List<UserChatMenu> toRemove = MenusCan.Children.OfType<UserChatMenu>().ToList();
            if (toRemove.Count() == 0) return;
            toRemove.RemoveAt(0);

            foreach (var remove in toRemove)
            {
                MenusCan.Children.Remove(remove);
            }
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

            Window window = Window.GetWindow(menu);
            if (window is null ||
                window is not MainWindow) throw new Exception("Its should be Main Window");

            menu.SetWindow(window as MainWindow);


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
        private const int _folderSliderHeight = 40;

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


        private SizerActionType? _sizeType;
        public void SetWindowSizerAction(bool isToClearFromPrevLevel = false)
        {
            _sizeType =
                ((MainWindow)Window.GetWindow(this)).GetWindowSizeType();

            if (isToClearFromPrevLevel) ClearPrevSizerChanges(_sizeType);
            /*            if (((MainWindow)Window.GetWindow(this)).IsWindowIsMaxSize() &&
                            ((MainWindow)Window.GetWindow(this)).GetMaxState())
                        {
                            ClearAllLevels();
                        }*/
            switch (_sizeType)
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

        public void SetUserTalkMenuAction(UserTalkControlButTypes type)
        {
            if (_menuChatterTalk is null) return;

            switch (type)
            {
                case UserTalkControlButTypes.OpenInNewWindow:
                    {
                        SetChatInOtherWindowAction();
                        return;
                    };
                case UserTalkControlButTypes.Archive:
                    break;
                case UserTalkControlButTypes.Unpin:
                    {
                        SetPinAction();
                        return;
                    }
                case UserTalkControlButTypes.MuteNotifs:
                    {
                        //Nothing there
                        return;
                    }
                case UserTalkControlButTypes.MarkRead:
                    {
                        SetUnreadMark();
                        return;
                    }
                case UserTalkControlButTypes.AddToFolder:
                    break;
                case UserTalkControlButTypes.ClearChat:
                    {
                        SetClearChat();
                        return;
                    }
                case UserTalkControlButTypes.DeleteChat:
                    {
                        SetDeleteChat();
                        return;
                    }
            }
        }

        public async Task SetDeleteChat()
        {
            TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();

            if (chat is null)
            {
                return;
            }
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                    new DeleteChat(await ApiService.GetUserById(chat.GetChatter().Id)));

        }

        public void SetClearChat()
        {
            TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new ClearChatHistory(chat, _system));
        }

        public void SetPinAction()
        {
            Window tempWindow = Window.GetWindow(this);
            if (tempWindow is not MainWindow main ||
                _menuChatterTalk is null) return;

            //Set new Window
            TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();

            chat.IsPinned = !chat.IsPinned;

            // Set in upper ChatsBox part
            //Change queue in system chats
            _system.Chats.Remove(chat);
            _system.Chats.Insert(0, chat);

            _menuChatterTalk.SetVisibilityToPinBlock(chat.IsPinned);

            //Update chats talk items
            RepaintUserChatsPanel();
        }

        public void SetUnreadMark()
        {
            Window tempWindow = Window.GetWindow(this);
            if (tempWindow is not MainWindow main ||
                _menuChatterTalk is null) return;

            TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();

            _menuChatterTalk.ChangeUnreadEllipseVisOnOtherDirection();
            chat.IsMarked = !chat.IsMarked;

            //close windows with this chat
            ((MainWindow)Window.GetWindow(this)).CloseWindowWithGivenChat(chat);

            if (UserChat.IsChatsAreEqual(chat))
            {
                UserChat.Visibility = Visibility.Hidden;
                ChosoeChatBorder.Visibility = Visibility.Visible;
            }
        }

        public void SetChatInOtherWindowAction()
        {
            Window tempWindow = Window.GetWindow(this);
            if (tempWindow is not MainWindow main) return;

            //Set new Window
            TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();

            _menuChatterTalk.SetVisibilityToUnreadEllipse(false);

            //If temp chat is on
            if (_system.GetChosenChat() is not null &&
                _system.GetChosenChat().Id == chat.Id)
            {
                //Clear chat
                UserChat.Visibility = Visibility.Hidden;
                ChosoeChatBorder.Visibility = Visibility.Visible;
            }
            //If chat is already y on main page
            if (((MainWindow)Window.GetWindow(this)).ChatIsOnOtherWindow(chat))
            {
                //Set window on the front
                ((MainWindow)Window.GetWindow(this)).SetOtherChatWindowOnFront(chat);
                return;
            }


            MainWindow window = new MainWindow(_system, chat, main);

            window.Show();
            //SetWindowChat(windChat);
        }
        /*        public void SetWindowChat(MainWn windChat)
                {
                    //Get chat to Add in new Window
                    TelegramLib.MainClasses.UserChat chat = GetChosenUserChat();

                    //Is already on other window
                    if (IsChatIsOnOtherWindow(chat)) return;

                    //Add chosen chat in system
                    _system.AddChatInOtherWindow(chat);

                    //Set chat in new Window
                    windChat.SetUserChat(chat, _system);
                }*/

        public bool IsChatIsOnOtherWindow(TelegramLib.MainClasses.UserChat chat)
        {
            return _system.IsChatContainsInOtherWidowList(chat);
        }

        public TelegramLib.MainClasses.UserChat? GetChosenUserChat()
        {
            int.TryParse(_menuChatterTalk.Tag.ToString(), out int id);
            return _system.Chats.FirstOrDefault(x => x.Chatter.Id == id);//.GetChatById(id); //_system.GetUserChatByChatterLogin(_menuChatterTalk.FriendLogin.Text);

            /*UserChat.SetUserChat(
             _system.GetUserChatByChatterName(_menuChatterTalk.FriendLogin.Text));*/
        }


        public void SetOnlyChatPage(TelegramLib.MainClasses.UserChat chat)
        {
            SetColumnMinWidth(LeftButtonsColumn, 0);
            SetColumnWidth(LeftButtonsColumn, 0);

            SetColumnMinWidth(ChatsColumn, 0);
            SetColumnWidth(ChatsColumn, 0);

            SetColumnMinWidth(GridSplitterColumn, 0);
            SetColumnWidth(GridSplitterColumn, 0);

            ShowChatControl();
            UserChat.SetUserChat(chat);
        }


        public event Action PageLoadedAction;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageLoadedAction?.Invoke();
        }

        public void UpdateContact(UserContactcs contact)
        {
            //Update chatTalk(ChatsBox) (If contains)
            UpdateUserTalk(contact);

            //Update Update UserChat (If chosen)
            UserChat.UpdateChatterName(contact);

            //Check in userChat
            if (UserChat.Visibility == Visibility.Visible)
            {
                UserChat.UpdateContactInfoBlock();
            }
        }

        public void UpdateUserTalk(UserContactcs contact)
        {
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByUserId(contact);
            if (chat is null) return;

            for (int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is ListBoxItem item &&
                    item.Tag.ToString() == chat.Id.ToString())
                {
                    if (item.Content is not UserTalkMessage mes) return;
                    mes.FriendLogin.Text = contact.Name;
                }
            }
        }

        public void UpdateVisAfterContactDeletion(UserContactcs contact)
        {
            //Remove contact from system
            _system.RemoveContact(contact);

            //UpdateChatsVis
            RepaintUserChatsPanel();

            //Update in temp userChat
            UpdateChatVis();
            UserChat.SetNameSurnameInUserParams();
        }

        public void UpdateChatVis()
        {
            if (UserChat.Visibility == Visibility.Visible)
            {
                UpdateChatContactInfo();
                //UserChat.RemoveContactAction();
            }
        }

        public void RemoveFromChatsBoxByContact(UserContactcs contact)
        {
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByUserId(contact);
            if (chat is null) return;

            ChatsBox.Items
                .Remove(ChatsBox.Items
                    .OfType<ListBoxItem>()
                    .Where(x => x.Tag.ToString() == chat.Id.ToString()).First());
        }

        public void UpdateTalkMessage(TelegramLib.MainClasses.UserContactcs contact)
        {
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(contact.ContactUserId);

            for (int i = 0; i < ChatsBox.Items.Count; i++)
            {
                if (ChatsBox.Items[i] is ListBoxItem item &&
                    item.Tag.ToString() == chat.Id.ToString() &&
                    item.Content is UserTalkMessage mes)
                {
                    mes.FriendLogin.Text = contact.Name;
                    //mes.LastMessage.Text = chat.GetLastMesDateInString();
                }
            }
        }

        public void UpdateBlockVis()
        {
            UserChat.SetUnblockGridVis();

            UpdateChatContactInfo();
        }

        public void UpdateChatContactInfo()
        {
            if (UserChat.Visibility == Visibility.Visible)
            {
                UserChat.UpdateContactInfoBlock();
            }
        }

        public void UpdateFoldersTalkMessages()
        {
            TelegramLib.MainClasses.FolderObjs.Folder fold = _system.GetFolderById(_system.Settings.ChosenFolderId);
            if (fold is null) return;

            ((MainWindow)Window.GetWindow(this)).SetChosenFolderByName(fold.Name);
        }

        public async Task DeleteChat(TelegramLib.MainClasses.User chatter,
            bool isDeleteForOtherUser)
        {
            //Delet for other user
            if (isDeleteForOtherUser) await DeleteForOtherUser(chatter);
            DeleteChat(chatter);
        }

        public async Task DeleteForOtherUser(TelegramLib.MainClasses.User chatter)
        {
            bool isOtherOnline = await ApiService.IsUserOnline(chatter.Id);
            if (!isOtherOnline)
            {
                //Delete from db (bgs + messages + chat + from folders etc...);
                //DeleteChatFromDb(chatter);

                await DeleteOtherChatFromDB(chatter);
                return;
            }
            await SignalRService.DeleteChatMethod(_system.LoggedUser, chatter);
        }

        public async Task DeleteOtherChatFromDB(TelegramLib.MainClasses.User chatter)
        {
            TelegramLib.MainClasses.UserChat chat =
                await ApiService.GetChatByUserAndSenderId(chatter.Id, _system.LoggedUser.Id);
            if (chat is null) return;

            await ApiService.DeleteChatById(chat.Id);
        }

        public void DeleteChat(TelegramLib.MainClasses.User chatter)
        {
            //Clear From db (messages + chat)
            DeleteChatFromDb(chatter);

            //Clear chat + in Folders in system 
            _system.DeleteChatByChatter(chatter);

            //Delete For Chatter by SignalR id need

            Application.Current.Dispatcher.Invoke(() =>
            {
                //Clear from vis(just updated)  
                RepaintUserChatsPanel();

                //Clear user chat (temp)
                UserChat.Visibility = Visibility.Hidden;
                ChosoeChatBorder.Visibility = Visibility.Visible;
            });
        }

        //If chat is absent(Was deleted)
        public async Task AddChat(UserContactcs contact)
        {
            //Add in db
            await ApiService.AddNewChat(_system.LoggedUser.Id, contact.ContactUserId);

            TelegramLib.MainClasses.UserChat chat = await ApiService.GetChatByUserAndSenderId(_system.LoggedUser.Id, contact.ContactUserId);

            //Add In system
            _system.AddChat(chat);

            //In visual
            await RepaintUserChatsPanel();
        }

        public void DeleteChatFromDb(TelegramLib.MainClasses.User chatter)
        {
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(chatter.Id);

            //Check this something wierd
            //await ApiService.DeleteChatByChatterId(chatter.Id);
            //ApiService.ClearChat(chat);

            //Delete Chats
            ApiService.DeleteChatById(chat.Id);

            //Get Folders which contains User
            List<int> foldersIds = _system.GetFoldersIdWithGivenUserId(chatter.Id);

            for (int i = 0; i < foldersIds.Count; i++)
            {
                ApiService.DeleteContactFromFolder(foldersIds[i], chatter.Id);
            }
        }

        public void SetImageMessages(string capture, List<Image> imgs)
        {
            UserChat.AddBigMediaImagesMessage(capture, imgs);
        }

        public async Task SetShareContactControl(int chatId, UserContactcs contactToSend,
            bool isAddInSignalR = true)
        {
            //Get contact params
            /*            (string name, string phoneNumber, string imgName) contactParams =
                            _system.GetChatterNameByChatId(chatId);*/

            string senderImgName = _system.LoggedUser.GetFirstImageNameInString();

            TelegramLib.MainClasses.UserChat chat = _system.GetChatById(chatId);
            if (chat is null) return;
            chat.IsMarked = false;

            //Add shared message in db
            await ApiService.AddShareContactMessage(contactToSend.ContactUserId,
                contactToSend.Name, chatId, _system.LoggedUser.Id, null);

            //Get sharedId
            int sharedId = await ApiService.GetLastSharedMessageIdByChatId(chatId);

            TelegramLib.MainClasses.User sharedUser =
                await ApiService.GetUserById(contactToSend.ContactUserId);

            //Add Shared message in system
            chat.AddSharedMessage(_system.LoggedUser.Id,
                sharedId, sharedUser, contactToSend.Name);

            //Add backwards
            if (isAddInSignalR)
            {
                AddSharedMessageInSignalR(chat.Chatter, contactToSend);
            }
            //UserChat.SetUserChat(chat);

            //UserChat.ShareContact(contactToSend, contactToSend.Name);
        }

        public async Task AddSharedMessageInSignalR(TelegramLib.MainClasses.User chatter,
            UserContactcs contactToSend)
        {
            if (await ApiService.IsUserOnline(chatter.Id))
            {
                //Go with signalR

                await SignalRService.AddShareContactMessage(_system.LoggedUser, chatter, contactToSend);
                return;
            }
            //just add in db (from temp user view)
            await AddShareMessageInDbIfOffline(chatter, contactToSend);
        }

        public async Task AddShareMessageInDbIfOffline(TelegramLib.MainClasses.User chatter,
            UserContactcs contactToSend)
        {
            TelegramLib.MainClasses.UserChat chat =
                await ApiService.GetChatByUserAndSenderId(chatter.Id, _system.LoggedUser.Id);

            if (chat is null) return;

            await ApiService.AddShareContactMessage(contactToSend.ContactUserId,
                contactToSend.Name, chat.Id, _system.LoggedUser.Id, null);
        }

        public async Task AddShareContactMesInDb
            (TelegramLib.MainClasses.User chatter,
            UserContactcs contactToSend)
        {
            TelegramLib.MainClasses.UserChat chat =
                _system.GetChatByChatterId(chatter.Id);
            //UserContactcs contact = _system.GetContactByUserId(chatter.Id);
            if (chat is null || contactToSend is null) return;
            chat.IsMarked = false;

            //Add shared message in db
            await ApiService.AddShareContactMessage(contactToSend.ContactUserId,
                 contactToSend.Name, chat.Id, chatter.Id, null);

            //Get sharedId
            int sharedId = await ApiService.GetLastSharedMessageIdByChatId(chat.Id);

            TelegramLib.MainClasses.User sharedUser =
                await ApiService.GetUserById(contactToSend.ContactUserId);

            //Add Shared message in system
            chat.AddSharedMessage(chatter.Id,
                sharedId, sharedUser, contactToSend.Name);

            //Update in visual things
            //Is to update is chosen chat
            await VisualUpdateAfterAddingShareControl(chat, contactToSend);
        }

        private async Task VisualUpdateAfterAddingShareControl(
            TelegramLib.MainClasses.UserChat chat, UserContactcs contactToSend)
        {
            if (UserChat.IsChoseChatIdIsEqual(chat.Id))
            {
                TelegramLib.MainClasses.User shared =
                    await ApiService.GetUserById(contactToSend.ContactUserId);
                TelegramLib.MainClasses.Messages.Message last =
                    chat.GetLastMessageObj();

                //Add share message
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UserChat.ShareContact(shared, shared.Name, last);
                });
            }
            SetMessageInUserTalkControl(chat.Id, "Contact");
        }

        public void UpdateAmountOfReadMessages(int chatId)
        {
            _chatsDict.TryGetValue(chatId, out ListBoxItem? item);
            if (item is null || item.Content is not UserTalkMessage talkMes) return;
            SetUnreadForUserTalk(talkMes, _system.GetChatById(chatId));
        }

        public async Task SetForwardMessage(int userIdToSend, 
            TelegramLib.MainClasses.Messages.Message mes)
        {
            UserChat.SetForwardedMessage(new List<Message>() { mes }, userIdToSend);
        }


        public void UpdateAmountOfSelectedMessages()
        {
            if (UserChat.Visibility == Visibility.Hidden) return;
            UserChat.UpdateSelectedAmount();
        }
    }
}
