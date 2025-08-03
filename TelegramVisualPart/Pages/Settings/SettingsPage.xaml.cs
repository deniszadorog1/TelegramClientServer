using MaterialDesignThemes.Wpf;
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
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Advanced;
using TelegramVisualPart.Pages.Settings.ChatSettings;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.UserControls;
using TelegramVisualPart.UserControls.DifferButs;
using static System.Net.Mime.MediaTypeNames;

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
        }

        public void SetUserInfo()
        {
            UserImage.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_system.LoggedUser.GetFirstImageName().Name), UriKind.Absolute));

            Username.Text = _system.LoggedUser.UserName;
            PhoneNumber.Text = _system.LoggedUser.PhoneNumber;
            UserLogin.Text = _system.LoggedUser.Login;
        }

        private void MoreInfoBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //set info 
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void Buts_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is MenuIconTextBut icon)
            {
                Page page = GetPageByIcon(icon);
                if (page is null) return;

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
            }
        }

        public Page GetPageByIcon(MenuIconTextBut icon)
        {
            return icon.Name == MyAccount.Name.ToString() ? new LoggedUserProfile(_system.LoggedUser, _system) :
                icon.Name == NotifsSounds.Name.ToString() ? new NotifsAndSounds.NotAndSoundSettings(_system) :
                icon.Name == PrivacySecurity.Name.ToString() ? new PrivAndSecurity.PrivacyAndSecurity(_system) :
                icon.Name == Folders.Name.ToString() ? new FoldersPage(_system) :
                icon.Name == Advanced.Name.ToString() ? new AdvancedPage(_system) :
                icon.Name == ChatSettings.Name.ToString() ? new MainChatSetPage(_system) : null;
        }

        public void SetButtonsView()
        {
            MoreInfoBut.IconType.Kind = PackIconKind.DotsVertical;
            CloseBut.IconType.Kind = PackIconKind.Close;

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
        }
    }
}
