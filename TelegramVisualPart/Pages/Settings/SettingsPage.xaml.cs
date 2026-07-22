using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Advanced;
using TelegramVisualPart.Pages.Settings.ChatSettings;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.Pages.Settings.Language;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.DifferButs;
using TelegramVisualPart.Windows;

namespace TelegramVisualPart.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private TelSystem _system;
        public SettingsPage(TelSystem system)
        {
            _system = system;

            InitializeComponent();
            SetButtonsView();

            SetUserInfo();


            LogOutMenu.SettSystem(_system);
        }

        private readonly SolidColorBrush _textColor = new SolidColorBrush(Colors.White);
        public void SetColorToSettingBut(MenuIconTextBut but)
        {
            but.IconType.Foreground = _textColor;
            but.ButName.Foreground = _textColor;
        }
        public async void SetUserInfo()
        {
            string path = await FilesAction.GetUserImagePath(_system.LoggedUser.GetFirstImageName().Name);
            UserImage.ImageSource = ApiService.GetCachedBitmap(path) is BitmapImage b and not null ? b : await SignalRHelperService.LoadBitmap(path);

            Username.Text = _system.LoggedUser.Login;
            PhoneNumber.Text = _system.LoggedUser.PhoneNumber;
            UserLogin.Text = _system.LoggedUser.Login;
        }

        private void MoreInfoBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //set info 
            //Log out 
            //((MainWindow)Window.GetWindow(this)).LogOut();

            LogOutMenu.Visibility = LogOutMenu.Visibility == Visibility.Visible ?
                Visibility.Hidden : Visibility.Visible;
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void Buts_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is MenuIconTextBut icon)
            {
                Page? page = GetPageByIcon(icon);
                if (page is null) return;

                if (page is LanguagePage langPage)
                {
                    ((MainWindow)Window.GetWindow(this)).SetThirdFrame(langPage);
                    return;
                }

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
            }
        }

        public Page? GetPageByIcon(MenuIconTextBut icon)
        {
            return icon.Name == MyAccount.Name.ToString() ? new LoggedUserProfile(_system.LoggedUser, _system) :
                icon.Name == NotifsSounds.Name.ToString() ? new NotifsAndSounds.NotAndSoundSettings(_system) :
                icon.Name == PrivacySecurity.Name.ToString() ? new PrivAndSecurity.PrivacyAndSecurity(_system) :
                icon.Name == Folders.Name.ToString() ? new FoldersPage(_system, true) :
                icon.Name == Advanced.Name.ToString() ? new AdvancedPage(_system) :
                icon.Name == ChatSettings.Name.ToString() ? new MainChatSetPage(_system) :
                icon.Name == Language.Name.ToString() ? new LanguagePage(_system) : null;
        }

        public void SetButtonsView()
        {
            MoreInfoBut.IconType.Kind = PackIconKind.DotsVerticalCircleOutline;
            CloseBut.IconType.Kind = PackIconKind.Close;
            //AddImageBut.IconType.Kind = PackIconKind.ImageOutline;

            MyAccount.IconType.Kind = PackIconKind.AccountCircleOutline;
            MyAccount.ButName.Text = "My account";


            NotifsSounds.IconType.Kind = PackIconKind.BellOutline;
            NotifsSounds.ButName.Text = "Notifications and Sounds";

            PrivacySecurity.IconType.Kind = PackIconKind.LockOutline;
            PrivacySecurity.ButName.Text = "Privacy and Security";

            ChatSettings.IconType.Kind = PackIconKind.ChatOutline;
            ChatSettings.ButName.Text = "Chat settings";

            Folders.IconType.Kind = PackIconKind.FolderOutline;
            Folders.ButName.Text = "Folders";

            Advanced.IconType.Kind = PackIconKind.MixerSettings;
            Advanced.ButName.Text = "Advanced";

            SpeakersAndCamera.IconType.Kind = PackIconKind.Speakerphone;
            SpeakersAndCamera.ButName.Text = "Speakers and Camera";

            BatteryAnimation.IconType.Kind = PackIconKind.BatteryOutline;
            BatteryAnimation.ButName.Text = "Battery and Animations";

            Language.IconType.Kind = PackIconKind.Language;
            Language.ButName.Text = "Language";

        }

        private void UserLogin_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock block) return;
            Cursor = Cursors.Hand;
            block.TextDecorations = TextDecorations.Underline;
        }

        private void UserLogin_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock block) return;
            Cursor = null;
            block.TextDecorations = null;
        }

        private void UserLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock block) return;
            Clipboard.SetText(block.Text);


            Window window = Window.GetWindow(this);

            if (window is MainWindow main)
            {
                main.SetTemporaryText("Login is copied!");
            }
        }

        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            LogOutMenu.Visibility = Visibility.Hidden;
        }

        private void UserIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MediaWindow mediaWindow = new MediaWindow(
                _system.LoggedUser, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.UserImages, _system);

            //mediaWindow.ToRe

            //Is exist
            if (((MainWindow)Window.GetWindow(this))
                .IsMediaWindowIsExistByUserId(_system.LoggedUser.Id)) return;
            mediaWindow.Show();
        }

        private async void ToRemoveUserImage_MouseDown(object sender, EventArgs e)
        {
            UserImage.ImageSource = new BitmapImage(new Uri(
                await FilesAction.GetUserImagePath(_system.LoggedUser.GetFirstImageName().Name), UriKind.Absolute));
        }

        private void UserIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
