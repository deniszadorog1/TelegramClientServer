using Accessibility;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramVisualPart.EnterInAccount;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Enums.Menus;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Pages.ChatActions;
using TelegramVisualPart.Pages.ChatActions.MessageMenuPages;
using TelegramVisualPart.Pages.ChatActions.SendMedia;
using TelegramVisualPart.Pages.MyProfile;
using TelegramVisualPart.Pages.Settings;
using TelegramVisualPart.Pages.Settings.ChatSettings;
using TelegramVisualPart.Pages.Settings.PrivAndSecurity;
using TelegramVisualPart.Pages.UserInfoContact.ActionsFolder;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls;
using TelegramVisualPart.UserControls.ChatControls.ChatButsControls;
using TelegramVisualPart.UserControls.FolderControls;
using TelegramVisualPart.Windows;
using Brushes = System.Windows.Media.Brushes;

namespace TelegramVisualPart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TelSystem _system;// = new TelSystem();

        private bool _isOnlyChat = false;
        public UserChat _onlyChatUserChat;

        List<MainWindow> _chatWindows = new List<MainWindow>();
        private MainWindow _bossWindow;

        DispatcherTimer _blockTimer;

        List<MediaWindow> _mediaWidows = new List<MediaWindow>();

        public bool IsOnlyTempOnlyChatIsExist(
            TelegramLib.MainClasses.UserChat chat)
        {
            return _chatWindows.Any(x => IsOnlyChatWindowHasChat(x, chat));
        }

        public bool IsOnlyChatWindowHasChat(MainWindow onlyChatWindow,
            TelegramLib.MainClasses.UserChat chat)
        {
            if (onlyChatWindow.MainFrame.Content is not MainChatPage chatPage) return false;
            return chatPage.IsChatIsOpened(chat);
        }

        //Basic start
        public MainWindow()
        {
            VisConstParamsJsonService.SetFileName("EnglishLang.json");
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            OutWindowBorder.BorderThickness = new Thickness(0);

            SetLoginPage();
            SetWindowSizeState();

            SignalRService.UpdateContactDel += UpdateUserSignalR;
            SignalRService.UpdateUserImagesDel += UpdateUserImages;
            SignalRService.UpdatePagePhotoDel += UpdatePagePhoto;
        }

        //Chat in other Window
        public MainWindow(TelSystem system,
            TelegramLib.MainClasses.UserChat chat,
            MainWindow boss)
        {
            InitializeComponent();

            _isOnlyChat = true;
            _system = system;
            _bossWindow = boss;
            _onlyChatUserChat = chat;

            //AddChatMainWindow();

            SetMainPage(system, isOnlyChat: true);
        }

        public bool IsSameOnlyChatById(int chatId)
        {
            for (int i = 0; i < _chatWindows.Count; i++)
            {
                if (_chatWindows[i]._onlyChatUserChat.Id == chatId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSavedMessesIsOnlyChat()
        {
            MainWindow? window = _chatWindows.FirstOrDefault(x => x._onlyChatUserChat is TelegramLib.MainClasses.SavedMessagesChat);
            return window is not null;
        }

        public bool GetIsOnlyChat()
        {
            return _isOnlyChat;
        }

        public UserChat GetOnlyChat()
        {
            return _onlyChatUserChat;
        }

        public UserChat UpdateUserTalkChat()
        {
            if (_bossWindow is null ||
                _bossWindow.MainFrame.Content is not MainChatPage bossChatPage) throw new Exception("Cant be");

            _bossWindow._onlyChatUserChat = _onlyChatUserChat;

            bossChatPage.UpdateUserTalkChat();

            return _onlyChatUserChat;
        }

        public void SetOnlyChatPage()
        {
            if (!_isOnlyChat ||
                MainFrame.Content is not MainChatPage page) return;
            page.SetOnlyChatPage(_onlyChatUserChat, _system);
        }

        private void UpdateUserImages(TelegramLib.MainClasses.User user)
        {
            if (_system.LoggedUser.Id == user.Id) return;

            //Set User image to system
            _system.UpdateChatterImage(user);

            Dispatcher.InvokeAsync(() =>
            {
                //Update in vis (chat for exp)
                UpdateChat();

                //update user TalkMessage
                if (MainFrame.Content is not MainChatPage chatPage) return;
                chatPage.UpdateChatTalkMessage(user.Id);

                ClearMediaWindows();
            });

        }

        public void UpdatePagePhoto(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (ThirdFrame.Content is EditUserContact edit)
                {
                    edit.UpdateImage();
                }
            });
        }

        public void ClearMediaWindows()
        {
            for(int i = 0; i < _mediaWidows.Count; i++)
            {
                _mediaWidows[i].Hide();
            }
            _mediaWidows.Clear();
        }

        private void UpdateUserSignalR(TelegramLib.MainClasses.User updated)
        {
            Dispatcher.Invoke(() =>
            {
                if (_system is null) return;

                TelegramLib.MainClasses.User? user = _system.Chats
                .Select(x => x.Chatter)
                .FirstOrDefault(x => x.Id == updated.Id);

                if (user is not null)
                {
                    user.UpdateParamsByUser(updated);
                }

                UserContactcs? contactToUpdate =
                    _system.Contacts.FirstOrDefault(x => x.ContactUserId == updated.Id);
                if (contactToUpdate is null) return;

                contactToUpdate.UpdateByUser(updated);

            });
        }

        private void SetLoginPage()
        {
            MainFrame.Content = null;
            EnterPage page = new EnterInAccount.EnterPage();
            MainFrame.Content = page;
        }

        public async void SetMainPage(TelSystem system, bool isOnlyChat = false)
        {
            _system = system;
            SetLanguageFile();

            if (!isOnlyChat)
            {
                SignalRService.SetSystem(_system);
                await SignalRService.SetBasicSignalRConnection();
                await SignalRService.UpdateOnlineStatus(_system.LoggedUser);
            }

            MainChatPage page = null; 
            try
            {
                page = new MainChatPage(_system);
            }
            catch (Exception ex)
            {
                Exception realEx = ex;
                while (realEx.InnerException != null) realEx = realEx.InnerException;
                MessageBox.Show($"BINGO: {realEx.Message}");
            }


            page.PageLoadedAction += SetOnlyChatPage;

            ((MainWindow)Window.GetWindow(this)).
                SetMainFrameContent(page);

            SetTimer();
        }

        public void SetLanguageFile()
        {
            string fileName = _system.Settings.LanguageSettings.Type ==
                TelegramLib.Enums.Settings.Language.LanguageType.English ?
                "EnglishLang.json" : "RussianLang.json";


            VisConstParamsJsonService.SetFileName(fileName);
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            return;
        }

        public TelSystem GetSystem()
        {
            return _system;
        }

        public void SetSecondaryFrame(Page page)
        {
            //SecondaryFrame.Visibility = Visibility.Visible;
            if (MainFrame.Content is MainChatPage chat)
            {
                DrawerHost.CloseDrawerCommand.Execute(null, chat.MainDrawerHost);
            }
            SecondaryFrame.Content = page;
            SetBlurEffectToFrame(MainFrame);

            SetFramePageHeight(page);
        }

        public void SetBlurEffectToFrame(Frame frame)
        {
            frame.Effect = null;

            frame.Effect = new BlurEffect()
            {
                Radius = 15,
            };
            frame.Background = Brushes.Transparent;
            SetBgShadowEffect(frame);
        }

        public void SetBgShadowEffect(Frame frame)
        {
            SolidColorBrush shadowBrush =
                new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));

            if (frame == MainFrame)
            {
                FirstFrameShadow.Background = shadowBrush;
            }
            else if (frame == SecondaryFrame)
            {
                SecondFrameShadow.Background = shadowBrush;
            }
            else if (frame == ThirdFrame)
            {
                ThirdFrameShadow.Background = shadowBrush;
            }
        }

        public void ClearShadowGridsAndEffects()
        {
            SolidColorBrush trancBrush = new SolidColorBrush(Colors.Transparent);
            FirstFrameShadow.Background = trancBrush;
            MainFrame.Effect = null;

            SecondFrameShadow.Background = trancBrush;
            SecondaryFrame.Effect = null;

            ThirdFrameShadow.Background = trancBrush;
            ThirdFrame.Effect = null;
        }

        private void ClearShadowGridEffect(Grid shadowGrid, Frame frame)
        {
            SolidColorBrush transparentColor =
                new SolidColorBrush(Colors.Transparent);

            shadowGrid.Background = transparentColor;
            frame.Effect = null;
            /*
                        FirstFrameShadow.Background = transparentColor;
                        MainFrame.Effect = null;*/
        }

        public void ClearShadowOfFrame(Frame frame)
        {
            SolidColorBrush trancFrame = new SolidColorBrush(Colors.Transparent);
            if (frame == MainFrame)
            {
                FirstFrameShadow.Background = trancFrame;
            }
            else if (frame == SecondaryFrame)
            {
                SecondFrameShadow.Background = trancFrame;
            }
            else if (frame == ThirdFrame)
            {
                ThirdFrameShadow.Background = trancFrame;
            }
        }

        public void ClearSecFrame()
        {
            //SecondaryFrame.Visibility = Visibility.Hidden;
            SecondaryFrame.Content = null;
            MainFrame.Effect = null;
            //ClearShadowGridsAndEffects();

            /*            MainFrame.Effect = null;
                        MainFrame.Background = Brushes.Transparent;
                        FirstFrameShadow.Background = Brushes.Transparent;
            */
            ClearShadowGridEffect(FirstFrameShadow, MainFrame);
        }

        public void SetMainFrame(Page page)
        {
            MainFrame.Content = page;
            ClearShadowGridsAndEffects();
        }

        public void UpdateUpperBorder()
        {
            UpperBorder.Background = (SolidColorBrush)Application.Current.Resources["UpperBangColor"];
        }

        public void SetThirdFrame(Page page)
        {
            //ThirdFrame.Visibility = Visibility.Visible;
            ThirdFrame.Content = page;
            SetBlurEffectToFrame(SecondaryFrame);
            //SetBlurEffectToMainFrame(MainFrame);
        }

        public void ClearThirdFrame()
        {
            if (ThirdFrame.Content is null) return;

            UpdatePrivacyAndSecurity();

            //ThirdFrame.Visibility = Visibility.Hidden;
            ThirdFrame.Content = null;

            SecondaryFrame.Effect = null;
            SecondaryFrame.Background = null;
            MainFrame.Background = Brushes.Transparent;
            SecondFrameShadow.Background = Brushes.Transparent;
            SecondaryFrame.Effect = null;

            //ClearShadowGridsAndEffects();
            ClearShadowGridEffect(ThirdFrameShadow, ThirdFrame);
        }

        public void UpdatePrivacyAndSecurity()
        {
            if (SecondaryFrame.Content is PrivacyAndSecurity privSettings)
            {
                privSettings.SetBasicParams();
            }
        }

        private void MainFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SecondaryFrame.Content is not null)
            {
                e.Handled = true;
            }
            ClearSecFrame();
        }

        private void ThirdFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SecondaryFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearThirdFrame();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private bool _isMax = false;
        private const int _normalSizeParam = 600;

        public bool GetMaxState()
        {
            return _isMax;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            SetWindowSizeState();

            //SizeC
        }

        public bool IsWindowIsMaxSize()
        {
            return this.Height == SystemParameters.WorkArea.Height &&
                this.Width == SystemParameters.WorkArea.Width;
        }

        public void SetWindowSizeState()
        {
            if (!_isMax)
            {
                this.Height = SystemParameters.WorkArea.Height;
                this.Width = SystemParameters.WorkArea.Width;

                this.Left = SystemParameters.WorkArea.Left;
                this.Top = SystemParameters.WorkArea.Top;

                _isMax = true;
            }
            else
            {
                this.Height = _normalSizeParam;
                this.Width = _normalSizeParam;

                this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2 + SystemParameters.WorkArea.Left;
                this.Top = (SystemParameters.WorkArea.Height - this.Height) / 2 + SystemParameters.WorkArea.Top;

                this.WindowState = WindowState.Normal;
                _isMax = false;
            }
            SetMainPageOnWindowSizeChange();
        }

        public void SetMainPageOnWindowSizeChange()
        {
            if (MainFrame.Content is MainChatPage page &&
               !_isOnlyChat)
            {
                page.ChatsColumn.Width = new GridLength(300);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ClearBasicChatSignalRMethods();

            if (_isOnlyChat)
            {
                this.Close();
                RemoveChatMainWindow();
                return;
            }

            ClearAllChatWindows();
            LogOut();
        }

        public void ClearBasicChatSignalRMethods()
        {
            if (MainFrame.Content is not MainChatPage mainPage) return;
            mainPage.ClearBasicChatSignalRMethods();
        }

        public async void LogOut()
        {
            CloseAllMediaWindows();
            if (MainFrame.Content is EnterPage)
            {
                this.Close();
                return;
            }

            if (_system is not null && _system.LoggedUser is not null)
            {
                await ApiService.SetUserOnlineStatus(_system.LoggedUser.Id, false);

                _system.LoggedUser.IsOnline =
                    (await ApiService.GetUserById(_system.LoggedUser.Id)).IsOnline;

                SignalRService.UpdateOnlineStatus(_system.LoggedUser);
            };

            ClearThirdFrame();
            ClearSecFrame();

            SetLoginPage();
            await SignalRService.DisconnectAsync();
        }

        public void CloseAllMediaWindows()
        {
            if (_bossWindow is not null) _bossWindow.CloseAllMediaWindows();

            for (int i = 0; i < _mediaWidows.Count; i++)
            {
                _mediaWidows[i].Close();
            }
            _mediaWidows.Clear();
        }

        public void AddMediaWindow(MediaWindow window)
        {
            _mediaWidows.Add(window);
        }

        private void UpperBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            //Check pro
            if (sender is Button button)
            {
                button.Background =
                    (SolidColorBrush)System.Windows.Application.Current.Resources["OtherUpperButColor"];
            }
            else if (sender is Grid grid)
            {
                grid.Background =
                    (SolidColorBrush)System.Windows.Application.Current.Resources["OtherUpperButColor"];
            }
        }

        private void UpperBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;

            if (sender is Button button) button.Background = Brushes.Transparent;
            else if (sender is Grid grid)
            {
                grid.Background = Brushes.Transparent;
            }
        }

        private void CloseWindowBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                    (SolidColorBrush)System.Windows.Application.Current.Resources["CloseWindowColor"];
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainFrame.Content is MainChatPage page)
            {
                page.UserChat.UserChatMenu.Visibility = Visibility.Hidden;
                page.UserChat.SendMesMenu.Visibility = Visibility.Hidden;

                page.ClearMenusCanvas();
            }
            if (SecondaryFrame.Content is UserInfo info)
            {
                info.ContactInfo.ContactMenu.Visibility = Visibility.Hidden;
            }

            Menus.Children.Clear();
        }

        public void ClearMainChatPageMenus()
        {

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WindowSizeChanged();
        }

        public void WindowSizeChanged()
        {
            double width = this.ActualWidth;
            double height = this.ActualHeight;

            if (height < SystemParameters.WorkArea.Height ||
                width < SystemParameters.WorkArea.Width)
            {
                WindowSizerIcon.Kind = PackIconKind.WindowMaximize;
                _isMax = false;
            }
            else
            {
                WindowSizerIcon.Kind = PackIconKind.WindowRestore;
                _isMax = true;
            }
            //Set page size for second and third frame pages
            if (SecondaryFrame.Content is Page page) SetFramePageHeight(page);


            //Set Window Parts Visibility
            //if (MainFrame.Content is not MainChatPage chatPage) return;

            if (_isOnlyChat) return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SetMainChatPagePartsSize();

                if (MainFrame.Content is MainChatPage mainChatPage &&
                /*IsWindowIsMaxSize()*/ _isMax && GetMaxState())
                {
                    mainChatPage.ClearAllLevels();
                }
            }), System.Windows.Threading.DispatcherPriority.Render);
        }


        private Enums.SizerActionType? _chosenWindowSizeType;

        public Enums.SizerActionType? GetWindowSizeType()
        {
            if (_isMax) return null;
            return _chosenWindowSizeType;
        }

        public void SetWindowSizeType(Enums.SizerActionType? type)
        {
            _chosenWindowSizeType = type;
        }

        private (double width, double height) _tempSize = (0, 0);

        public void SetMainChatPagePartsSize()
        {
            /*            if (this.ActualWidth != _tempSize.width ||
                            this.ActualHeight != _tempSize.height)
                        {
                            _tempSize = (this.ActualWidth, this.ActualHeight);
                        }
                        else return;*/

            //From The most
            if (MainFrame.Content is MainChatPage mainChatPage)
            {
                Enums.SizerActionType? tempSizer = GetWindowSizeType();

                double width = GetMainWindowWidthWithoutRightContactInfo();


                //Temp chat messages glues to one part(left)
                if (/*this.ActualWidth*/ width < 1500)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.FirstLevel);

                    SetWindowSizeType(Enums.SizerActionType.FirstLevel);
                    mainChatPage.SetWindowSizerAction();
                }
                //Temp chat messages in glued to differ borders
                if (/*this.Width*/ width < 1200)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.SecondLevel);

                    SetWindowSizeType(Enums.SizerActionType.SecondLevel);
                    mainChatPage.SetWindowSizerAction(isClearPrev);
                }
                //AllChats Closing
                if (/*this.Width*/ width < 1000)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.ThirdLevel);

                    SetWindowSizeType(Enums.SizerActionType.ThirdLevel);
                    mainChatPage.SetWindowSizerAction(isClearPrev);
                }
                //Temp chat is closing + Tabs is going to top
                if (/*this.ActualWidth*/ width < 800)
                {
                    SetWindowSizeType(Enums.SizerActionType.FourthLevel);
                    mainChatPage.SetWindowSizerAction();

                    if (MainFrame.Content is MainChatPage page) page.ClearChatBgs(true);
                }
            }
            return;
        }

        private double GetMainWindowWidthWithoutRightContactInfo()
        {
            double baseSize = this.ActualWidth;

            if (MainFrame.Content is MainChatPage page)
            {
                return baseSize - page.GetAdditionalContactInfoWidth();
            }

            return baseSize;
        }

        private void SetFramePageHeight(Page page)
        {
            if (this.Height <= page.ActualHeight ||

                (this.Height > page.ActualHeight &&
                this.Height <= page.MaxHeight &&
                page.MaxHeight != double.PositiveInfinity))
            {
                page.Height = this.Height - 100;

                WindowSizerIcon.Kind = PackIconKind.WindowMaximize;
            }
            else if (this.Height >= page.MaxHeight &&
                page.MaxHeight != double.PositiveInfinity)
            {
                page.Height = page.MaxHeight;
                WindowSizerIcon.Kind = PackIconKind.WindowRestore;
            }
        }

        private System.Windows.Point _mouseDownPosition;
        private bool _isMouseDown = false;

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            //SetMainPageOnWindowSizeChange();
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _mouseDownPosition = e.GetPosition(this);
                _isMouseDown = true;

                if (this.WindowState != WindowState.Maximized)
                {
                    this.DragMove();
                }
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && this.WindowState == WindowState.Maximized && e.LeftButton == MouseButtonState.Pressed)
            {
                const int _windWidth = 1000;
                _isMouseDown = false;

                var mousePosition = e.GetPosition(this);
                double percentHorizontal = mousePosition.X / this.ActualWidth;
                double targetWidth = _windWidth;

                this.WindowState = WindowState.Normal;

                var screenPoint = PointToScreen(mousePosition);
                this.Left = screenPoint.X - targetWidth * percentHorizontal;
                this.Top = 0;
                this.Width = targetWidth;

                this.DragMove();
            }
        }

        public void ShowChosenMessageByMessageId(int mesId)
        {
            if (MainFrame.Content is MainChatPage page)
                page.ShowChosenMessageByMessageId(mesId);
        }

        public void SetChatsMessages()
        {
            if (MainFrame.Content is not MainChatPage) return;
            ((MainChatPage)MainFrame.Content).SetMessageGridMagnifier();
        }

        public void AddEmojiOnPage(string text)
        {
            //Is Media send page
            if (SecondaryFrame.Content is SendMediaPage media)
            {
                media.AddSmileInTextBox(text);
                return;
            }


            //Is Chat page
            if (MainFrame.Content is MainChatPage chatPage)
            {
                chatPage.UserChat.AddEmoji(text);
            }
        }

        public void SendGif(string gifPath, string senderImageName)
        {
            if (MainFrame.Content is MainChatPage chatPage)
            {
                chatPage.UserChat.SendGif(gifPath, senderImageName);
            }
        }

        public void SendStickerInChat(System.Windows.Controls.Image img, string senderImageName)
        {
            if (MainFrame.Content is MainChatPage chatPage)
            {
                chatPage.UserChat.AddStickerMessage(img, senderImageName);
            }
        }

        public void SetMainFrameContent(Page page)
        {
            MainFrame.Content = page;
        }

        public void ClearPageFromParentFrame(Page page)
        {
            var frame = FindParentFrame(page);
            if (frame is null) return;

            ClearShadowGridsAndEffects();
            //ClearShadowOfFrame(frame);
            frame.Content = null;
        }

        private Frame FindParentFrame(DependencyObject child)
        {
            while (child != null)
            {
                if (child is Frame frame)
                    return frame;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        public void UpdateLoggedUserPage()
        {
            SetSecondaryFrame(new MyProfileSettings(_system.LoggedUser, _system, new SettingsPage(_system)));
        }

        public void UpdateChatSettingsPage()
        {
            SetSecondaryFrame(new MainChatSetPage(_system));
        }

        public void SetChatBg()
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.SetUserChatBg();
        }

        public void ClearChat()
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.UserChat.ClearChat();
            chatPage.ClearChosenUserTalkValue();

            if (_isOnlyChat && _bossWindow is not null)
            {
                _bossWindow.ClearChatFromOnlyChatWindow(_onlyChatUserChat);
            }
        }

        public void ClearChatFromOnlyChatWindow(TelegramLib.MainClasses.UserChat chat)
        {
            if (MainFrame.Content is MainChatPage chatPage)
            {
                chatPage.ClearTalkMessageFromOnlyChat(chat);
            }
        }

        public void ClearVisChat()
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.UserChat.ClearChat();
        }

        public void UpdateUserChatTalkControl()
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.UpdateUserTalkChat();
        }

        public void SetChosenFolderByName(string folderName)
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;

            TelegramLib.MainClasses.FolderObjs.Folder folder = _system.GetFolderByName(folderName);
            if (folder is null) return;

            chatPage.SetChosenFolder(folder);
        }

        public void UpdateFolders()
        {
            if (_isOnlyChat && _bossWindow is not null)
            {
                _bossWindow.UpdateFolders();
                return;
            }

            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.UpdateFolders();
        }

        public void RemoveElementFromChat(int elIndex,
            TelegramLib.Enums.Messages.MediaType type)
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.UserChat.RemoveElementFromChatBox(elIndex, type);
        }

        public void ClearVisualActionPage()
        {
            if (SecondaryFrame.Content is VisualActionPage) ClearSecFrame();
            else ClearThirdFrame();
        }

        public async Task SetAllChatsInMainPage()
        {
            if (MainFrame.Content is not MainChatPage page) return;
            await page.SetActiveChats();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        public void UpdateTabsStandings()
        {
            if (MainFrame.Content is MainChatPage chatPage)
                chatPage.UpdateTabsPlacement();
        }

        public void AddSubMenu(ToAddSubMenuType type, Point enteredItemPoint)
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.AddSubMenu(type, enteredItemPoint);
        }

        public void ClearSubMenus()
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.ClearSubMenus();
        }

        public void SetSubMenuAction(UserTalkControlButTypes type)
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.SetUserTalkMenuAction(type);
        }

        public void ClearAllChatWindows()
        {
            foreach (var window in _chatWindows)
            {
                window.Close();
            }
            _chatWindows.Clear();
        }

        public void ClearAllChatWindowsFromBosWindow()
        {
            if (_bossWindow is null) return;

            _bossWindow.ClearAllChatWindows();
        }

        public void DeleteMediaWindow(MediaWindow mediaWindow)
        {
            _mediaWidows.Remove(mediaWindow);
            mediaWindow.Close();
        }

        public bool ChatIsOnOtherWindow(TelegramLib.MainClasses.UserChat chat)
        {
            return _chatWindows.FirstOrDefault(x => x._onlyChatUserChat.Id == chat.Id) is not null;
        }

        public void SetOtherChatWindowOnFront(TelegramLib.MainClasses.UserChat chat)
        {
            MainWindow? window =
                _chatWindows.FirstOrDefault(x => x._onlyChatUserChat.Id == chat.Id);
            if (window is null) return;

            //window.Activate();          
            window.Topmost = true;
            window.Topmost = false;
        }

        //from chat window
        public void AddChatMainWindow()
        {
            if (_bossWindow is null) return;
            _bossWindow._chatWindows.Add(this);
        }

        //from sub window
        public void RemoveChatMainWindow()
        {
            _bossWindow._chatWindows.Remove(this);
        }

        public void CloseWindowWithGivenChat(TelegramLib.MainClasses.UserChat chat)
        {
            MainWindow? toRemove =
                _chatWindows.FirstOrDefault(x => x._onlyChatUserChat.Id == chat.Id);
            if (toRemove is null) return;

            toRemove.Close();
            _chatWindows.Remove(toRemove);
        }

        public void UpdateContactParams(UserContactcs contact)
        {
            if (_bossWindow is not null)
            {
                _bossWindow.UpdateContactParams(contact);
            }
            if (MainFrame.Content is not MainChatPage page) return;

            page.UpdateContact(contact);

            //Checked in open page
            if (SecondaryFrame.Content is UserInfo info)
            {
                info.UpdateContact(contact);
            }
            //check in UserChat thing
        }

        public void UpdateDeletedUser(UserContactcs contact)
        {
            //correct in mainChatPage
            if (MainFrame.Content is MainChatPage page)
            {
                page.UpdateVisAfterContactDeletion(contact);
            }

            //Correct in SecFrame(user info)
            if (SecondaryFrame.Content is UserInfo info)
            {
                ClearSecFrame();
                //info.ContactRemoveAction();           
            }
        }

        public void UpdateChatParamsVis()
        {
            if (MainFrame.Content is MainChatPage page)
            {
                page.UpdateChatVis();
            }
        }

        public void UpdateUserTalkMessage(UserContactcs contact)
        {
            if (MainFrame.Content is not MainChatPage page) return;

            page.UpdateTalkMessage(contact);
        }

        public void SetFramesAfterBlockingContact()
        {
            //Update user info (SecFrame)
            if (SecondaryFrame.Content is UserInfo info)
            {
                info.UpdateBlockAction();
            }

            //Update mainframe (Chat stuff)
            if (MainFrame.Content is MainChatPage page)
            {
                page.UpdateBlockVis();
            }
        }

        public async Task SetBlockedUserVisParams(bool isBlock, TelegramLib.MainClasses.User user)
        {
            if (MainFrame.Content is MainChatPage page) await page.SetUserBlockParams(isBlock, user);
        }

        public void ClearTempPageFrame(Page page)
        {
            if (SecondaryFrame.Content == page) ClearSecFrame();
            else if (ThirdFrame.Content == page) ClearThirdFrame();
        }

        public void FocusUserChat()
        {
            if (MainFrame.Content is MainChatPage main)
            {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Input,
                    new Action(() =>
                    {
                        main.UserChat.Focus();
                        Keyboard.Focus(main.UserChat);
                    }));
            }
        }

        public void SetPageOnSameFrame(Page toCheck, Page toSet)
        {
            if (SecondaryFrame.Content == toCheck) SetSecondaryFrame(toSet);
            else if (ThirdFrame.Content == toCheck) SetThirdFrame(toSet);
        }

        public void ClearPageFrame(Page page)
        {
            if (SecondaryFrame.Content == page) ClearSecFrame();
            else if (ThirdFrame.Content == page) ClearThirdFrame();
        }

        public void UpdateFolder()
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.UpdateFoldersTalkMessages();
        }

        public void ClearBlockFrame()
        {
            BlockFrame.Content = null;
            _blockTimer.Stop();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_system is not null && BlockFrame.Content is null &&
               _system.Settings.PrivacySettings.PassCode is not null &&
               _blockTimer is not null)
            {
                _blockTimer.Stop();
                _blockTimer.Start();
            }
        }

        public void StartBlockTimer()
        {
            if (_blockTimer is not null)
            {
                _blockTimer.Stop();
                _blockTimer.Start();
            }
        }

        public void StopTimer()
        {
            if (_blockTimer is not null) _blockTimer.Stop();
        }

        public void ClearTimer()
        {
            if (_blockTimer is not null)
            {
                _blockTimer.Stop();
                _blockTimer = null;
            }
        }

        public void SetTimer()
        {
            if (_system is null ||
                _system.Settings.PrivacySettings.PassCode is null ||
                _system.Settings.PrivacySettings.PassCode.MinutesTimer == -1) return;

            _blockTimer = new DispatcherTimer();
            int seconds = _system.Settings.PrivacySettings.PassCode.MinutesTimer * 60;

            //if (seconds == 0) seconds = 60;

            seconds = 5;

            _blockTimer.Interval = TimeSpan.FromSeconds(seconds);
            _blockTimer.Tick += (sender, e) =>
            {
                if (_system.Settings.PrivacySettings.PassCode is null) return;
                SetBlockFrame();
            };
            _blockTimer.Start();
        }

        public void SetBlockFrame()
        {
            _blockTimer.Stop();
            BlockPage page = new BlockPage(_system);
            BlockFrame.Content = page;
        }

        public async Task DeleteChat(TelegramLib.MainClasses.User chatter,
            bool isDeleteForOtherUser)
        {
            if (_isOnlyChat && _bossWindow is not null)
            {
                _bossWindow.DeleteChat(chatter, isDeleteForOtherUser);
                RemoveChatMainWindow();
            }

            if (MainFrame.Content is not MainChatPage page) return;

            //Close only chat window
            CloseOnlyChatWindowByChatter(chatter);

            await page.DeleteChat(chatter, isDeleteForOtherUser);

            page.EscapePressedAction();
        }

        private void CloseOnlyChatWindowByChatter(TelegramLib.MainClasses.User chatter)
        {
            if (_bossWindow is not null) return;

            MainWindow? window = _chatWindows.FirstOrDefault(x =>
            x._onlyChatUserChat is not null &&
            x._onlyChatUserChat.GetChatter() is not null &&
            x._onlyChatUserChat.GetChatter().Id == chatter.Id);

            if (window is null) return;

            _chatWindows.Remove(window);
            window.Close();
        }

        public void AddAddMediaPage(List<string> firstMediaPath, string text, UserChat chat, List<Message> forwardMessages)
        {
            SetSecondaryFrame(new SendMediaPage(firstMediaPath, text, _system, chat, forwardMessages));
        }

        public void SendBigImagesMessage(string capture, List<Image> imgs, List<string> paths, SendMediaType type)
        {
            if (MainFrame.Content is MainChatPage page) page.SetImageMessages(capture, imgs, paths, type);
        }

        public void SetSharedContact(int chatId, UserContactcs contact)
        {
            if (MainFrame.Content is MainChatPage page)
                page.SetShareContactControl(chatId, contact);
        }

        public async Task AddChatInMainPage(TelegramLib.MainClasses.UserContactcs contact)
        {
            if (MainFrame.Content is MainChatPage page) await page.AddChat(contact);
        }

        public void SetChosenChat(UserChat chat)
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page._chosenChat = chat;
        }

        public bool EscapePressed()
        {
            if (ThirdFrame.Content is not null)
            {
                ClearThirdFrame();
                return false;
            }
            if (SecondaryFrame.Content is not null)
            {
                ClearSecFrame();
                return false;
            }
            return true;
        }

        public void UpdateReadCountOfReadMessages(int chatId)
        {
            //Get boss window

            if (_bossWindow is not null &&
                _bossWindow.MainFrame.Content is MainChatPage mainPage)
            {
                mainPage.UpdateAmountOfReadMessages(chatId);
                return;
            }

            if (MainFrame.Content is not MainChatPage page) return;
            page.UpdateAmountOfReadMessages(chatId);
        }

        public void UpdateChatControls()
        {
            if (_bossWindow is not null)
            {
                _bossWindow.UpdateChatControls();
                return;
            }

            if (MainFrame.Content is MainChatPage page) page.RepaintUserChatsPanel();
        }

        public void SetVisibilityInTaskBar(bool isVis)
        {
            this.ShowInTaskbar = isVis;
        }

        public void SetMessageMenu(MessageMenuType type)
        {
            switch (type)
            {
                case MessageMenuType.TextMessage:
                    {
                        break;
                    }
                case MessageMenuType.MediaMessage:
                    {
                        break;
                    }
            }
        }

        public void SetForwardMessage(TelegramLib.MainClasses.Messages.Message mes,
            int? userIdToSend)
        {
            if (MainFrame.Content is MainChatPage page)
                page.SetForwardMessage(userIdToSend, mes);
        }

        public void UpdateUserChatSelectedAmount()
        {
            if (MainFrame.Content is MainChatPage page)
                page.UpdateAmountOfSelectedMessages();
        }

        public async Task SetOtherChatByUserId(int userId)
        {
            if (MainFrame.Content is MainChatPage page)
                await page.SetChatByChatterId(userId);
        }

        public void RemoveFromGodWindow(MediaWindow medWindow)
        {
            _mediaWidows.Remove(medWindow);
        }

        public bool IsMediaWindowIsExistByUserId(int userId)
        {
            for (int i = 0; i < _mediaWidows.Count; i++)
            {
                if (_mediaWidows[i].IsUsersIdsAreEqual(userId)) return false;
            }
            return false;
        }

        public void DeleteMessage(TelegramLib.MainClasses.Messages.Message mes)
        {
            if (MainFrame.Content is MainChatPage mainPage)
            {
                mainPage.DeleteMessage(mes);
            }
        }

        public void SendOneForwardMessage(TelegramLib.MainClasses.Messages.Message message)
        {
            if (MainFrame.Content is MainChatPage page) page.SetForwardedOnlyMessage(message);
        }

        public void UpdateAutoDelVis(TelegramLib.MainClasses.UserChat chat)
        {
            if (MainFrame.Content is MainChatPage page)
                page.UpdateAutoDelDurationVis(chat);
        }

        public async Task AddStatMessage(StaticMessage mes, bool isBoth,
            TelegramLib.MainClasses.UserChat chat)
        {
            if (MainFrame.Content is MainChatPage page)
            {
                page.AddStatMessage(mes, isBoth, chat);
            }
        }

        public void UpdateChatAutDelIconVisibility()
        {
            if (MainFrame.Content is MainChatPage page) page.UpdateUserChatAutoDelIconVis();
        }

        public void RemoveMessagesByDates(List<DateTime> removeDates)
        {
            if (MainFrame.Content is MainChatPage page)
                page.RemoveMessagesByDates(removeDates);
        }

        public void ScrollToMessagesByDate(DateTime dateTime)
        {
            if (MainFrame.Content is MainChatPage page) page.ScrolToMessageByDateTime(dateTime);
        }

        public void AddMenuInMenuCan(TextBoxMenu menu)
        {
            Menus.Children.Add(menu);
        }

        public void AddFolderMenu(FolderMenu menu)
        {
            Menus.Children.Add(menu);
        }

        public Size GetWindowSize()
        {
            return new Size(this.ActualWidth, this.ActualHeight);
        }

        public bool GetIsLongContnetChatState()
        {
            if (MainFrame.Content is MainChatPage page) return page._chatterInfo;

            return false;
        }

        public void SetIsLongContnetChatState(bool status)
        {
            if (MainFrame.Content is MainChatPage page)
                page.SetUserChatContactInfoStatus(status);
        }

        public bool IsSecPageIsContactInfo()
        {
            return SecondaryFrame.Content is UserInfo;
        }

        private void SecondFrameShadow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void SetContactMask(int userIdToSetMask)
        {
            if (_isOnlyChat && _bossWindow is not null)
            {
                _bossWindow.SetContactMask(userIdToSetMask);
            }

            if (MainFrame.Content is MainChatPage page)
            {
                //Set for MainChatPage
                page.SetContactMask(userIdToSetMask);

                if (_isOnlyChat)
                {
                    TelegramLib.MainClasses.User chatter = _system.GetUserById(userIdToSetMask);
                    page.UserChat.SetUserImage(chatter.GetFirstImageName().Name);
                }
            }

            if (SecondaryFrame.Content is UserInfo info)
            {
                //Set for contact info
                info.UpdateImage();
            }
        }

        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MainFrame.Content is MainChatPage chatPage) chatPage.ClearChatMouseButtonDown();
        }

        public async void SetTemporaryText(string text)
        {
            const int fadeTime = 150;
            const int stopTime = 1000;

            if (TempTextGrid.Visibility == Visibility.Visible) return;
            TempTextGrid.Visibility = Visibility.Visible;

            TempTextBlock.Text = text;

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(fadeTime),
                FillBehavior = FillBehavior.HoldEnd
            };

            TempTextGrid.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            await Task.Delay(stopTime);

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(fadeTime),
                FillBehavior = FillBehavior.Stop
            };

            fadeOut.Completed += (sender, e) =>
            {
                TempTextGrid.Visibility = Visibility.Hidden;
            };

            TempTextGrid.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        public void UpdateChat()
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.UpdateChat();
        }

        public void UpdateMyProfilePage()
        {
            if (SecondaryFrame.Content is not MyProfileSettings myProfSet) return;

            myProfSet.SetUserImage();
        }


        public bool BringWindowToView(UserChat chat)
        {
            MainWindow? window = _chatWindows.FirstOrDefault(x => x._onlyChatUserChat.Id == chat.Id &&
            _onlyChatUserChat.GetType() == chat.GetType());

            if (window is null) return false;

            if (window is not null)
            {
                window.Topmost = true;
                Dispatcher.Invoke(new Action(() =>
                {
                    window.Topmost = false;
                    window.UpperBorder.Focus();
                }));
            };
            return true;
        }

        public bool IsChattersChatIsOnOtherWindow(TelegramLib.MainClasses.User user)
        {
            return _chatWindows.Any(x =>
            (x._onlyChatUserChat is TelegramLib.MainClasses.SavedMessagesChat && _system.LoggedUser.Id == user.Id) ||
            (x._onlyChatUserChat is not TelegramLib.MainClasses.SavedMessagesChat && x._onlyChatUserChat.Chatter.Id == user.Id));
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_bossWindow is null)
            {
                this.Topmost = true;
                this.Topmost = false;
            }
        }

        public void ClearCommentChatBox()
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.ClearCommentChatBox();
        }

        public void ClearReplyRow()
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.ClearReplyRow();
        }

        public void UpdateScheduleIconVisibility()
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.UpdateScheduleGridVisibility();
        }

        public void UpdateScheduleChat()
        {
            if (MainFrame.Content is not MainChatPage page) return;
            page.UpdateScheduleChatIfNeed();
        }

        public void ClearSchedulePage()
        {
            if (SecondaryFrame.Content is SetScheduleMessage ||
                SecondaryFrame.Content is IsMakeActionOnBothSides)
            {
                ClearSecFrame();
            }
        }

        public void HideEnnesChat(int? senderChatId)
        {
            if (senderChatId is not null &&
                _bossWindow is null) CloseChatWindowsWithSameChat((int)senderChatId);

            if (_bossWindow is null) return;
            _bossWindow.HideChatIfOpened(senderChatId);
        }

        public void CloseChatWindowsWithSameChat(int chatId)
        {
            List<MainWindow> toRemove = new List<MainWindow>();

            for (int i = 0; i < _chatWindows.Count; i++)
            {
                if (_chatWindows[i].GetOnlyChat().Id == chatId)
                {
                    _chatWindows[i].Close();
                    toRemove.Add(_chatWindows[i]);
                }
            }

            foreach (var remove in toRemove)
            {
                _chatWindows.Remove(remove);
            }
        }

        public void HideChatIfOpened(int? senderChatId)
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            TelegramLib.MainClasses.UserChat chat = chatPage.GetUserChat();

            if ((chat is SavedMessagesChat && (senderChatId is null || _system.LoggedUser.Id == senderChatId)) ||
                (chat.Chatter is not null && chat.Chatter.Id == senderChatId))
            {
                //clear chat in boos window
                chatPage.HideChat();
            }
        }

        public ListBoxItem GetChatListBoxItemByMesId(int mesId)
        {
            if (MainFrame.Content is MainChatPage mainChatPage)
            {
                return mainChatPage.GetChatListBoxItemByMesId(mesId);
            }
            return null;
        }

        public void SetReplyMessage(UserControl control, List<TelegramLib.MainClasses.Messages.Message> messes)
        {
            if (MainFrame.Content is MainChatPage mainChatPage)
            {
                mainChatPage.SetReplyMessage(control, messes);
            }
        }

        public void UpdateUserTalkTickStatus(UserChat chat)
        {
            if(MainFrame.Content is MainChatPage main)
            {
                main.UpdateUserTalkTickStatus(chat);
            }
        }

        public void UpdateGlobalMedias()
        {
            if (MainFrame.Content is not MainChatPage main) return;

            TelegramLib.Enums.Messages.MediaType? type = 
                main.SearchControl.GetChosenTabType();

            if (type is null) return;

            main.SetSearchedParams((TelegramLib.Enums.Messages.MediaType)type);
        }

    }
}