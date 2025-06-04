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
using TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages;
using TelegramVisualPart.UserControls.DifferButs;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity
{
    /// <summary>
    /// Логика взаимодействия для PrivacyAndSecurity.xaml
    /// </summary>
    public partial class PrivacyAndSecurity : Page
    {
        private Frame _frame;
        public PrivacyAndSecurity(Frame frame)
        {
            _frame = frame;
            InitializeComponent();

            SetButsVisibility();
        }

        public void SetButsVisibility()
        {
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
            ((MainWindow)Window.GetWindow(_frame)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(_frame)).SetSecondaryFrame(new SettingsPage(_frame));
        }

        private void Buts_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is EnumPrivacyButton but)
            {
                Page page = GetPageForBut(but.Name);

                if (page is null) return;

                if (page is SetLocalCode || page is BlockedUsers)
                {
                    ((MainWindow)Window.GetWindow(_frame)).SetSecondaryFrame(page);
                }
                else ((MainWindow)Window.GetWindow(_frame)).SetThirdFrame((page));
            }
        }

        private Page GetPageForBut(string name)
        {
            return name == LocalPasscode.Name.ToString() ? new SetLocalCode(_frame) :
                name == BlockedUsers.Name.ToString() ? new BlockedUsers(_frame) :
                name == PhoneNumber.Name.ToString() ? new SetPrivacyByType(_frame, Enums.PrivacySettingType.PhoneNumber) :
                name == LastSeen.Name.ToString() ? new SetPrivacyByType(_frame, Enums.PrivacySettingType.LastSeen) :
                name == ProfilePhotos.Name.ToString() ? new SetPrivacyByType(_frame, Enums.PrivacySettingType.ProfilePhotos) :
                name == ForwardedMessages.Name.ToString() ? new SetPrivacyByType(_frame, Enums.PrivacySettingType.ForwardedMessages) : null;
        }
    }
}
