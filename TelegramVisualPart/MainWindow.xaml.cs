using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Pages.MyProfile;
using TelegramVisualPart.Pages.Settings.ChatSettings;
using TelegramVisualPart.Pages.Settings.PrivAndSecurity;
using TelegramVisualPart.Pages.VisualPages;
using Brushes = System.Windows.Media.Brushes;

namespace TelegramVisualPart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TelSystem _system = new TelSystem();

        public MainWindow()
        {
            //FFMpegCore.FFMpeg.SetExecutablesPath(@"B:\Tools\ffmpeg\bin");

            //VisConstParamsJsonService.GetStringByName("check");

            InitializeComponent();

            ///Visuals/Images/UserImages/Minato.jpg"
            //MainFrame.Content = new EnterPage();
            MainFrame.Content = new MainChatPage(_system);

            System.Windows.Application.Current.Resources["TempActiveTextColor"] =
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(_system.LoggedUser.MainColor.R,
                    _system.LoggedUser.MainColor.G, _system.LoggedUser.MainColor.B));
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
            SetBlurEffectToMainFrame(MainFrame);
        }

        public void SetBlurEffectToMainFrame(Frame frame)
        {
            frame.Effect = null;
            frame.Effect = new BlurEffect()
            {
                Radius = 2
            };
            frame.Background = Brushes.Transparent;
        }

        public void ClearSecFrame()
        {
            //SecondaryFrame.Visibility = Visibility.Hidden;
            SecondaryFrame.Content = null;
            MainFrame.Effect = null;
        }

        public void SetMainFrame(Page page)
        {
            MainFrame.Content = page;
        }

        public void SetThirdFrame(Page page)
        {
            //ThirdFrame.Visibility = Visibility.Visible;
            ThirdFrame.Content = page;
            SetBlurEffectToMainFrame(SecondaryFrame);
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

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpperBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                    (SolidColorBrush)System.Windows.Application.Current.Resources["OtherUpperButColor"];

        }

        private void UpperBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;

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

        }

        private System.Windows.Point _mouseDownPosition;
        private bool _isMouseDown = false;

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
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
            if (frame is not null) frame.Content = null;
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
    }
}