using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для DeleteChat.xaml
    /// </summary>
    public partial class DeleteChat : Page
    {
        private TelegramLib.MainClasses.User _user;
        public DeleteChat(TelegramLib.MainClasses.User user)
        {
            _user = user;
            InitializeComponent();

            SetBasicParams();

            SetLanguageText.SetDeleteChat(this);
        }
        public void SetBasicParams()
        {
            BgBrush.ImageSource = new BitmapImage(
                new Uri(FilesAction.GetUserImagePath(
                    _user.GetFirstImageNameInString()), UriKind.Absolute));

            FirstUsername.Text = _user.Login;
            UsernameRunBlock.Text = _user.Login;
        }

        private void DeleteBut_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);

            if (window is MainWindow main)
            {
                main.DeleteChat(_user, (bool)ShowChatNameBox.IsChecked);
                main.CloseAllMediaWindows();
                main.ClearTempPageFrame(this);
            }
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
            //((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void DeleteBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["CloseButBg"];
        }

        private void DeleteBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void BothCheckPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowChatNameBox.IsChecked = !ShowChatNameBox.IsChecked;
        }

        private void BothCheckPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void BothCheckPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
