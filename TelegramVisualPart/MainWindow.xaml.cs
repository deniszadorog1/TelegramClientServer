using MaterialDesignThemes.Wpf;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity.Core.EntityClient;
using System.Diagnostics.Eventing.Reader;
using System.Formats.Tar;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.Services;
using TelegramVisualPart.CustWindows;
using TelegramVisualPart.EnterInAccount;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Enums.Menus;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Pages.MyProfile;
using TelegramVisualPart.Pages.MyProfile.SetInformation;
using TelegramVisualPart.Pages.Settings.ChatSettings;
using TelegramVisualPart.Pages.Settings.PrivAndSecurity;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls;
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
        private UserChat _onlyChatUserChat;

        List<MainWindow> _chatWindows = new List<MainWindow>();
        private MainWindow _bossWindow;

        //Basic start
        public MainWindow()
        {

            VisConstParamsJsonService.SetFileName("EnglishLang.json");
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            OutWindowBorder.BorderThickness = new Thickness(0);

            ///Visuals/Images/UserImages/Minato.jpg"
            SetLoginPage();
            SetWindowSizeState();

            SignalRService.UpdateContactDel += UpdateUserSignalR;
        }

        //Chat in other Window
        public MainWindow(TelSystem system, TelegramLib.MainClasses.UserChat chat,
            MainWindow boss)
        {
            InitializeComponent();

            _isOnlyChat = true;
            _system = system;
            _bossWindow = boss;
            _onlyChatUserChat = chat;

            AddChatMainWindow();

            SetMainPage(system);

            //Set chat page
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

            bossChatPage.UpdateUserTalkChat();// bossChatPage.GetChtControlByChatterName(_onlyChatUserChat.Chatter.Name);

            return _onlyChatUserChat;
        }

        public void SetOnlyChatPage()
        {
            if (!_isOnlyChat ||
                MainFrame.Content is not MainChatPage page) return;
            page.SetOnlyChatPage(_onlyChatUserChat);
        }

        private void UpdateUserSignalR(User updated)
        {
            Dispatcher.Invoke(() =>
            {
                if (_system is null) return;
                UserContactcs? contactToUpdate =
                    _system.Contacts.FirstOrDefault(x => x.ContactUserId == updated.Id);
                if (contactToUpdate is null) return;

                contactToUpdate.UpdateByUser(updated);
            });
        }

        private void SetLoginPage()
        {
            EnterPage page = new EnterInAccount.EnterPage();
            MainFrame.Content = page;
        }

        public async void SetMainPage(TelSystem system)
        {
            _system = system;
            SetLanguageFile();

            SignalRService.SetSystem(_system);

            await SignalRService.SetBasicSignalRConnetion();
            SignalRService.UpdateOnlineStatus(_system.LoggedUser);


            MainChatPage page = new MainChatPage(_system);

            page.PageLoadedAction += SetOnlyChatPage;

            ((MainWindow)Window.GetWindow(this)).
                SetMainFrameContent(page);
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
            await SetTestSystem();

            Application.Current.Resources["TempActiveTextColor"] =
                new SolidColorBrush(Color.FromRgb(_system.LoggedUser.MainColor.R,
                _system.LoggedUser.MainColor.G, _system.LoggedUser.MainColor.B));

            MainFrame.Content = new MainChatPage(_system);
        }

        public async Task SetTestSystem()
        {
            string testLogPas = "qwe";

            User user = await ApiService.GetUser(testLogPas, testLogPas);
            if (user is not null)
            {
                _system = await ApiService.GetTelSystem(testLogPas, testLogPas);

                if (_system.Settings.GetChatSettings().Wallpaper is null)
                    _system.Settings.GetChatSettings().Wallpaper =
                        new TelegramLib.UserSettings.SettingsTypes.SubSettings.ChatWallpaper();
                return;
            }

            await ApiService.AddNewUser(testLogPas, testLogPas, "testName", "testSurname", "testPhoneNumber", DateTime.Now);

            user = await ApiService.GetUser(testLogPas, testLogPas);

            await ApiService.AddUserBasicColor(user.Id);
            await ApiService.AddUserSettings(user.Id);

            _system = await ApiService.GetTelSystem(testLogPas, testLogPas);

            if (_system.Settings.GetChatSettings().Wallpaper is null)
                _system.Settings.GetChatSettings().Wallpaper =
                    new TelegramLib.UserSettings.SettingsTypes.SubSettings.ChatWallpaper();
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

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            if (_isOnlyChat)
            {
                this.Close();
                RemoveChatMainWindow();
            }

            ClearAllChatWindows();
            LogOut();
        }

        public async void LogOut()
        {
            if (MainFrame.Content is EnterPage)
            {
                this.Close();
                return;
            }

            if (_system is not null && _system.LoggedUser is not null)
            {
                await ApiService.SetUserOnlineStatus(_system.LoggedUser.Id, false);

                _system.LoggedUser.IsOnline = (await ApiService.GetUserById(_system.LoggedUser.Id)).IsOnline;

                SignalRService.UpdateOnlineStatus(_system.LoggedUser);
            };

            ClearThirdFrame();
            ClearSecFrame();
            SetLoginPage();
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
                page.ClearMenusCanvas();
            }
            if (SecondaryFrame.Content is UserInfo info)
            {
                info.ContactInfo.ContactMenu.Visibility = Visibility.Hidden;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight < SystemParameters.WorkArea.Height ||
                this.ActualWidth < SystemParameters.WorkArea.Width)
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
                IsWindowIsMaxSize() && GetMaxState())
                {
                    mainChatPage.ClearAllLevels();
                }
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            /*            if (_isOnlyChat) return;
                        SetMainChatPagePartsSize();*/
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

                //Temp chat messages glues to one part(left)
                if (this.ActualWidth < 1500)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.FirstLevel);

                    SetWindowSizeType(Enums.SizerActionType.FirstLevel);
                    mainChatPage.SetWindowSizerAction();
                }
                //Temp chat messages in glued to differ borders
                if (this.Width < 1200)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.SecondLevel);

                    SetWindowSizeType(Enums.SizerActionType.SecondLevel);
                    mainChatPage.SetWindowSizerAction(isClearPrev);
                }
                //AllChats Closing
                if (this.Width < 1000)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.ThirdLevel);

                    SetWindowSizeType(Enums.SizerActionType.ThirdLevel);
                    mainChatPage.SetWindowSizerAction(isClearPrev);
                }
                //Temp chat is closing + Tabs is going to top
                if (this.ActualWidth < 800)
                {
                    SetWindowSizeType(Enums.SizerActionType.FourthLevel);
                    mainChatPage.SetWindowSizerAction();
                }
            }
            return;
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

        public void SetChatsMessages()
        {
            if (MainFrame.Content is not MainChatPage) return;
            ((MainChatPage)MainFrame.Content).SetMessageGridMagnifier();
        }

        public void AddEmojiInChat(string text)
        {
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
            SetSecondaryFrame(new MyProfileSettings(_system.LoggedUser, _system));
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
        }

        public void UpdateUserChatTalkControl()
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;
            chatPage.UpdateUserTalkChat();
        }

        public void SetChosenFolderByName(string folderName)
        {
            if (MainFrame.Content is not MainChatPage chatPage) return;

            Folder folder = _system.GetFolderByName(folderName);
            if (folder is null) return;

            chatPage.SetChosenFolder(folder);
        }

        public void UpdateFolders()
        {
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

        public void SetAllChatsInMainPage()
        {
            if (MainFrame.Content is not MainChatPage page) return;

            page.SetActiveChats();
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

        public bool ChatIsOnOtherWindow(TelegramLib.MainClasses.UserChat chat)
        {
            return _chatWindows.FirstOrDefault(x => x._onlyChatUserChat.Id == chat.Id) is not null;
        }

        public void SetOtherChatWindowOnFront(TelegramLib.MainClasses.UserChat chat)
        {
            MainWindow? window = _chatWindows.FirstOrDefault(x => x._onlyChatUserChat.Id == chat.Id);
            if (window is null) return;

            //window.Activate();          
            window.Topmost = true;
        }

        //from chat window
        public void AddChatMainWindow()
        {
            _bossWindow._chatWindows.Add(this);

            StringBuilder asd = new StringBuilder();

            (asd[0], asd[1]) = (asd[1], asd[2]);

            asd.Append(asd.ToString());
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
                info.ContactRemoveAction();
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

        public void ClearTempPageFrame(Page page)
        {
            if (SecondaryFrame.Content == page) ClearSecFrame();
            else if (ThirdFrame.Content == page) ClearThirdFrame();
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
    }
}