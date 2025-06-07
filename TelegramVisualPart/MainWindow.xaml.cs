using MaterialDesignThemes.Wpf;
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
            MainFrame.Content = new MainChatPage(MainFrame);
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

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
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
        }
    }
}