using MaterialDesignThemes.Wpf;
using System.IO;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramVisualPart.Pages;

namespace TelegramVisualPart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ///Visuals/Images/UserImages/Minato.jpg"
            MainFrame.Content = new MainChatPage();
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
         
            //ThirdFrame.Visibility = Visibility.Hidden;
            ThirdFrame.Content = null;
            
            SecondaryFrame.Effect = null;
            SecondaryFrame.Background = null;
            MainFrame.Background = Brushes.Transparent;
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
            if(sender is Button but) but.Background = 
                    (SolidColorBrush)Application.Current.Resources["OtherUpperButColor"];

        }

        private void UpperBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;

        }

        private void CloseWindowBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                    (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(MainFrame.Content is MainChatPage page)
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

        private Point _mouseDownPosition;
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
    }
}