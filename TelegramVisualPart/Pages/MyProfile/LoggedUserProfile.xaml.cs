using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoggedUserProfile.xaml
    /// </summary>
    public partial class LoggedUserProfile : Page
    {
        private User _user;
        private TelSystem _system;
        public LoggedUserProfile(User user, TelSystem system)
        {
            _user = user;
            _system = system;

            InitializeComponent();
            SetBasicParams();

            SetLanguageText.SetLoggedUserProfile(this);
        }

        public void SetBasicParams()
        {
            CloseBut.IconType.Kind = PackIconKind.Close;
            SettingsBut.IconType.Kind = PackIconKind.LeadPencil;

            UserLoginBlock.Text = _user.Login;

            SetOnlineStatus();
            //LastSeenOnline.Text = _user.LastSeenOnline.ToString();

       
            PhoneNumberBlock.Text = _user.PhoneNumber;
            UserNameBlock.Text = _user.Login;

            UserImage.ImageSource = 
                new BitmapImage(new Uri(FilesAction.GetUserImagePath(_user.GetFirstImageName().Name), UriKind.Absolute));
        }

        private void SetOnlineStatus()
        {
            if (_system.LoggedUser.IsOnline)
            {
                LastSeenOnline.Foreground =
                    (SolidColorBrush)Application.Current.FindResource("TempActiveTextColor");
                LastSeenOnline.Text = VisConstParamsJsonService.GetStringByName("OnlineStat");
                return;
            }
            LastSeenOnline.Text = $"{_system.LoggedUser.LastSeenOnline.Day}.{_system.LoggedUser.LastSeenOnline.Month}.{_system.LoggedUser.LastSeenOnline.Year}";
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
                new MyProfile.MyProfileSettings(_user, _system));
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void Ellipse_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string firstImage = _user.GetFirstImageName().Name;
            Image chosen = FilesAction.GetUserImage(firstImage);

            List<Image> imgs = FilesAction.GetUserImages(_user.GetImagesNames());
            
            VisualActionPage page = new VisualActionPage(chosen, imgs);
            page.SetUserImages(_user.UserImages, _system, _user.Name, true, null);

            page.ToRemoveImage += ToRemoveUserImage_MouseDown;

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);
        }

        private void ToRemoveUserImage_MouseDown(object sender, EventArgs e)
        {
            UserImage.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_user.GetFirstImageName().Name), UriKind.Absolute));
        }

        private void UserNameBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            UserNameBlock.TextDecorations = TextDecorations.Underline;
        }

        private void UserNameBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            UserNameBlock.TextDecorations = null;
        }

        private void UserNameBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(UserNameBlock.Text);
        }
    }
}
