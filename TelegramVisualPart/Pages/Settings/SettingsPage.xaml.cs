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
using TelegramVisualPart.Pages.Advanced;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.UserControls;

namespace TelegramVisualPart.Pages.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            SetButtonsView();
        }

        private void PackIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.White;
            Cursor = Cursors.Hand;
        }

        private void PackIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.Gray;
            Cursor = null;
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
            if (sender is IconTextBut icon)
            {
                Page page = GetPageByIcon(icon);
                if (page is null) return;

                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
            }
        }

        public Page GetPageByIcon(IconTextBut icon)
        {
            return icon.Name == MyAccount.Name.ToString() ? new LoggedUserProfile() :
                icon.Name == NotifsSounds.Name.ToString() ? new NotifsAndSounds.NotAndSoundSettings() : 
                icon.Name == PrivacySecurity.Name.ToString() ? new PrivAndSecurity.PrivacyAndSecurity() :
                icon.Name == Folders.Name.ToString() ? new FoldersPage() : 
                icon.Name == Advanced.Name.ToString() ? new AdvancedPage() : null;
        }

        public void SetButtonsView()
        {
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

    }
}
