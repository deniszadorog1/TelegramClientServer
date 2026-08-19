using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatsSearch;

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для DeleteChat.xaml
    /// </summary>
    public partial class DeleteChat : Page
    {
        private TelegramLib.MainClasses.User _user;
        private TelSystem _system;
        private UserChat _chat;
        public DeleteChat(TelegramLib.MainClasses.User user, TelSystem system, UserChat chat)
        {
            _user = user;
            _system = system;
            _chat = chat;

            InitializeComponent();

            SetBasicParams();

            SetLanguageText.SetDeleteChat(this);
        }
        public async void SetBasicParams()
        {
/*            BgBrush.ImageSource = new BitmapImage(
                new Uri(await FilesAction.GetUserImagePath(
                    _user.GetFirstImageNameInString()), UriKind.Absolute));


            string asd = _user.GetImgName();*/

            UserContactcs contact = _system.GetContactById(_user.Id);
            if (contact is null) return;

            string imgName = _chat is not null ? _chat.Chatter.GetImgName() : contact.GetFirstImageName().Name;

            BitmapImage bitmap = ApiService.GetCachedBitmap(imgName);

            BgBrush.ImageSource = bitmap is not null ? bitmap :
                await SignalRHelperService.LoadBitmap(await FilesAction.GetUserImagePath(contact.GetFirstImageName().Name));

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
