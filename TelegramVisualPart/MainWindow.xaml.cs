using MaterialDesignThemes.Wpf;
using Microsoft.IdentityModel.Tokens;
using System.Data.Entity.Core.EntityClient;
using System.Diagnostics.Eventing.Reader;
using System.Formats.Tar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramLib.Services;
using TelegramVisualPart.EnterInAccount;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Pages.MyProfile;
using TelegramVisualPart.Pages.Settings.ChatSettings;
using TelegramVisualPart.Pages.Settings.PrivAndSecurity;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using Brushes = System.Windows.Media.Brushes;

namespace TelegramVisualPart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TelSystem _system;// = new TelSystem();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            ///Visuals/Images/UserImages/Minato.jpg"
            SetLoginPage();
            SetWindowSizeState();

            SignalRService.UpdateContactDel += UpdateUserSignalR;
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

            SignalRService.SetSystem(_system);

            await SignalRService.SetBasicSignalRConnetion();
            SignalRService.UpdateOnlineStatus(_system.LoggedUser);

            ((MainWindow)Window.GetWindow(this)).
                SetMainFrameContent(new MainChatPage(_system));
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
            ClearShadowGridsAndEffects();
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

            UpdatePrivacyAndScurity();

            //ThirdFrame.Visibility = Visibility.Hidden;
            ThirdFrame.Content = null;

            SecondaryFrame.Effect = null;
            SecondaryFrame.Background = null;
            MainFrame.Background = Brushes.Transparent;
            ClearShadowGridsAndEffects();
        }

        public void UpdatePrivacyAndScurity()
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
            if (MainFrame.Content is MainChatPage page)
            {
                page.ChatsColumn.Width = new GridLength(300);
            }
        }

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
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
            }
            if (SecondaryFrame.Content is UserInfo info)
            {
                info.ContactInfo.ContactMenu.Visibility = Visibility.Hidden;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Height != SystemParameters.WorkArea.Height ||
                this.ActualWidth != SystemParameters.WorkArea.Width)
            {
                WindowSizerIcon.Kind = PackIconKind.WindowMaximize;

            }
            else
            {
                WindowSizerIcon.Kind = PackIconKind.WindowRestore;
            }
            //Set page size for second and third frame pages
            if (SecondaryFrame.Content is Page page) SetFramePageHeight(page);


            //Set Window Parts Visibility
            //if (MainFrame.Content is not MainChatPage chatPage) return;


            Dispatcher.BeginInvoke(new Action(() =>
            {
                SetMainChatPagePartsSize();

                if (MainFrame.Content is MainChatPage mainChatPage &&
                IsWindowIsMaxSize() && GetMaxState())
                {
                    mainChatPage.ClearAllLevels();
                }
            }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            SetMainChatPagePartsSize();
        }

        private Enums.SizerActionType? _chosenWindowSizeType;

        public Enums.SizerActionType? GetWindowSizeType()
        {
            return _chosenWindowSizeType;
        }

        public void SetWindowSizeType(Enums.SizerActionType? type)
        {
            _chosenWindowSizeType = type;
        }

        private (double width, double height) _tempSize = (0,0);

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
                if (this.ActualWidth < 1800 /*&& (this.Width > 1700 || !_isMax)*/)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.FirstLevel);

                    SetWindowSizeType(Enums.SizerActionType.FirstLevel);
                    mainChatPage.SetWindowSizerAction();
                }
                //Temp chat messages in glued to differ borders
                if (this.Width < 1700 /*&& (this.Width > 1500 || !_isMax)*/)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.SecondLevel);

                    SetWindowSizeType(Enums.SizerActionType.SecondLevel);
                    mainChatPage.SetWindowSizerAction(isClearPrev);
                }
                //AllChats Closing
                if (this.Width < 1500 /*&& (this.Width > 1300 || !_isMax)*/)
                {
                    bool isClearPrev = tempSizer is null ? false :
                        ((int)tempSizer) > ((int)Enums.SizerActionType.ThirdLevel);

                    SetWindowSizeType(Enums.SizerActionType.ThirdLevel);
                    mainChatPage.SetWindowSizerAction(isClearPrev);
                }
                //Temp chat is closing + Tabs is going to top
                if (this.ActualWidth < 1300 /*&& (this.Width > 1000 || !_isMax)*/)
                {
                    SetWindowSizeType(Enums.SizerActionType.FourthLevel);
                    mainChatPage.SetWindowSizerAction();
                }
            }

            return;
            //Optional (the smallest one)
            //page which is not on secondary frame moves to main frame
            //When window getting bigger it moves to Secondary frame



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

    }
}