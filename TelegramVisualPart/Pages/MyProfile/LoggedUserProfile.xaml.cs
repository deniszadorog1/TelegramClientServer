using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoggedUserProfile.xaml
    /// </summary>
    public partial class LoggedUserProfile : Page
    {
        private User _user;
        public LoggedUserProfile(User user)
        {
            _user = user;
            InitializeComponent();
            SetBasicParams();
        }

        public void SetBasicParams()
        {
            CloseBut.IconType.Kind = PackIconKind.Close;
            SettingsBut.IconType.Kind = PackIconKind.LeadPencil;

            UserLoginBlock.Text = _user.Login;
            LastSeenOnline.Text = _user.LastSeenOnline.ToString();
            PhoneNumberBlock.Text = _user.PhoneNumber;
            UserNameBlock.Text = _user.UserName;

        }

        private void Buts_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.White;
            Cursor = Cursors.Hand;
        }

        private void Buts_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.Gray;
            Cursor = null;
        }

        private void SettingsBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new MyProfile.MyProfileSettings(_user));
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }
    }
}
