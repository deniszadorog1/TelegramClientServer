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
using TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages;
using TelegramVisualPart.UserControls.DifferButs;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity
{
    /// <summary>
    /// Логика взаимодействия для PrivacyAndSecurity.xaml
    /// </summary>
    public partial class PrivacyAndSecurity : Page
    {
        private TelSystem _system;
        public PrivacyAndSecurity(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetButsVisibility();
        }

        public void SetButsVisibility()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            LocalPasscode.NamePart.Text = "Local passcode";
            LocalPasscode.EnumPart.Text = "STATUS";

            BlockedUsers.NamePart.Text = "Blocked users";
            BlockedUsers.EnumPart.Text = "AMOUNT";

            PhoneNumber.NamePart.Text = "Phone number";
            PhoneNumber.EnumPart.Text = "STATUS";

            LastSeen.NamePart.Text = "Last seen & online";
            LastSeen.EnumPart.Text = "STATUS";

            ProfilePhotos.NamePart.Text = "Profile photos";
            ProfilePhotos.EnumPart.Text = "STATUS";

            ForwardedMessages.NamePart.Text = "Forwarded messages";
            ForwardedMessages.EnumPart.Text = "STATUS";

            Messages.NamePart.Text = "Messages";
            Messages.EnumPart.Text = "STATUS";

            DateOfBirth.NamePart.Text = "Date of Birth";
            DateOfBirth.EnumPart.Text = "STATUS";

            BioBut.NamePart.Text = "BIO";
            BioBut.EnumPart.Text = "STATUS";

            ClearPayments.NamePart.Text = "Clear Payments and Shipping Info";
            ClearPayments.EnumPart.Visibility = Visibility.Hidden; ;

            DeleteAway.NamePart.Text = "If away for...";
            DeleteAway.EnumPart.Text = "STATUS";
        }

        private void Buts_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;

            if (sender is PackIcon icon)
            {
                icon.Foreground = Brushes.White;
            }
        }

        private void Buts_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is PackIcon icon)
            {
                icon.Foreground = Brushes.Gray;
            }
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SettingsPage(_system));
        }

        private void Buts_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is EnumPrivacyButton but)
            {
                Page page = GetPageForBut(but.Name);

                if (page is null) return;

                if (page is SetLocalCode || page is BlockedUsers)
                {
                    ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
                }
                else ((MainWindow)Window.GetWindow(this)).SetThirdFrame((page));
            }
        }

        private Page GetPageForBut(string name)
        {
            return name == LocalPasscode.Name.ToString() ? new SetLocalCode(_system) :
                name == BlockedUsers.Name.ToString() ? new BlockedUsers(_system) :
                name == PhoneNumber.Name.ToString() ? new SetPrivacyByType(Enums.PrivacySettingType.PhoneNumber) :
                name == LastSeen.Name.ToString() ? new SetPrivacyByType(Enums.PrivacySettingType.LastSeen) :
                name == ProfilePhotos.Name.ToString() ? new SetPrivacyByType(Enums.PrivacySettingType.ProfilePhotos) :
                name == ForwardedMessages.Name.ToString() ? new SetPrivacyByType(Enums.PrivacySettingType.ForwardedMessages) :
                name == DateOfBirth.Name.ToString() ? new SetPrivacyByType(Enums.PrivacySettingType.DateBirth) :
                name == BioBut.Name.ToString() ? new SetPrivacyByType(Enums.PrivacySettingType.Bio) :
                name == DeleteAway.Name.ToString() ? new PrivacyDeleteAccount() :
                name == Messages.Name.ToString() ? new PrivacyMessages() : null;
                
        }
    }
}
