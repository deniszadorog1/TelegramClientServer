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
using TelegramVisualPart.Enums;
using TelegramVisualPart.UserControls.DifferButs;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity
{
    /// <summary>
    /// Логика взаимодействия для SetPrivacyByType.xaml
    /// </summary>
    public partial class SetPrivacyByType : Page
    {
        private Frame _frame;
        private PrivacySettingType _type;

        public SetPrivacyByType(Frame frame, PrivacySettingType type)
        {
            _frame = frame;
            _type = type;
            InitializeComponent();

            SetStartGridsSize();
            SetVisualPart();
            EverybodyRadio.IsChecked = true;
        }

        public void SetVisualPart()
        {
            switch (_type)
            {
                case PrivacySettingType.PhoneNumber:
                    {
                        SetPhoneGridBlocks();
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        SetLastSeenBlocks();
                        break;
                    }
                case PrivacySettingType.ProfilePhotos:
                    {
                        SetProfilePhotosBlocks();
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        SetForwardMessagesBlocks();
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        SetDateOfBirthBlocks();
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        SetBioBlocks();
                        break;
                    }
            }
        }

        private EnumPrivacyButton _alwaysShare;
        private EnumPrivacyButton _neverShare;

        private void SetBioBlocks()
        {
            ActionType.Text = "Bio";
            WhoCanUseBlock.Text = "Who can see my bio";

            BioAlwaysShareBut.NamePart.Text = "Always share with";
            BioAlwaysShareBut.EnumPart.Text = "Add users";

            BioNeverShareBut.NamePart.Text = "Never share with";
            BioNeverShareBut.EnumPart.Text = "Add users";

            _alwaysShare = BioAlwaysShareBut;
            _neverShare = BioNeverShareBut;
        }

        private void SetDateOfBirthBlocks()
        {
            ActionType.Text = "Date of birth privacy";
            WhoCanUseBlock.Text = "Who can see my date of birth";

            BirthDateAlwaysShareBut.NamePart.Text = "Always share with";
            BirthDateAlwaysShareBut.EnumPart.Text = "Add users";

            BirthDateNeverShareBut.NamePart.Text = "Never share with";
            BirthDateNeverShareBut.EnumPart.Text = "Add users";

            _alwaysShare = BirthDateAlwaysShareBut;
            _neverShare = BirthDateNeverShareBut;
        }

        private void SetForwardMessagesBlocks()
        {
            ActionType.Text = "Forwarded Messages";
            WhoCanUseBlock.Text = "Who can add a link to my account when forwarding my messages";

            ForwardMeesagesAlwaysShareBut.NamePart.Text = "Always share with";
            ForwardMeesagesAlwaysShareBut.EnumPart.Text = "Add users";

            ForwardMeesagesNeverShareBut.NamePart.Text = "Never share with";
            ForwardMeesagesNeverShareBut.EnumPart.Text = "Add users";

            _alwaysShare = ForwardMeesagesAlwaysShareBut;
            _neverShare = ForwardMeesagesNeverShareBut;
        }

        public void SetProfilePhotosBlocks()
        {
            ActionType.Text = "Profile Photos";
            WhoCanUseBlock.Text = "Who can see my last seen time";

            SetPubPhotoBut.IconType.Kind = PackIconKind.CameraOutline;
            SetPubPhotoBut.IconType.Foreground = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButForeGround"];
            SetPubPhotoBut.ButName.Text = "Set public photo";
            SetPubPhotoBut.ButName.Foreground = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButForeGround"];

            ProfPhotosAlwaysShareBut.NamePart.Text = "Always share with";
            ProfPhotosAlwaysShareBut.EnumPart.Text = "Add users";

            ProfPhotosNeverShareBut.NamePart.Text = "Never share with";
            ProfPhotosNeverShareBut.EnumPart.Text = "Add users";

            _alwaysShare = ProfPhotosAlwaysShareBut;
            _neverShare = ProfPhotosNeverShareBut;

        }

        public void SetLastSeenBlocks()
        {
            ActionType.Text = "Last seen & online";
            WhoCanUseBlock.Text = "Who can see my last seen time";


            LastSeenEverybodyUsersExcepts.NamePart.Text = "Never share with";
            LastSeenEverybodyUsersExcepts.EnumPart.Text = "Add users";

            LastSeenOtherAlwaysShare.NamePart.Text = "Always share with";
            LastSeenOtherAlwaysShare.EnumPart.Text = "Add users";
            LastSeenOtherNeverShare.NamePart.Text = "Never share with";
            LastSeenOtherNeverShare.EnumPart.Text = "Add users";

            HideReedTimeToggleBut.TextBlock.Text = "Hide read time";
        }

        public void SetPhoneGridBlocks()
        {
            ActionType.Text = "Phone number privacy";
            WhoCanUseBlock.Text = "Who can see my phone number";

            PhoneEverybodyUsersExcepts.NamePart.Text =
                "Never share with";
            PhoneEverybodyUsersExcepts.EnumPart.Text =
                "Add Users";
            DescriptionPhoneEverybodyText.Text =
                 "Users who have your number saved in their contacts " +
                "will also see it on Telegram";

            PhoneContactAlwaysShareBut.NamePart.Text =
                "Always share with";
            PhoneContactAlwaysShareBut.EnumPart.Text =
                "Add users";
            PhoneContactNeverShareBut.NamePart.Text =
                "Never share with";
            PhoneContactNeverShareBut.EnumPart.Text =
                "Add users";
            DescriptionPhoneContactText.Text =
                "Users who have your number saved in their contacts " +
                "will also see it on Telegram";

            PhoneNobodyAlwaysShareBut.NamePart.Text =
                "Always share with";
            PhoneNobodyAlwaysShareBut.EnumPart.Text =
                "Add users";
            DescriptionPhoneNobodyText.Text =
                "Users who add your number to their contacts will see" +
                "it on Telegram only if they are your contacts";

        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void EverybodyRadio_Checked(object sender, RoutedEventArgs e)
        {
            HideAllSubGrids();
            switch (_type)
            {
                case PrivacySettingType.PhoneNumber:
                    {
                        PhoneEverybody.Height = new GridLength();
                        Height = 450;
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenEverybodyRow.Height = new GridLength();
                        Height = 450;
                        break;
                    }
                case PrivacySettingType.ProfilePhotos:
                    {
                        PubPhotoButRow.Height = new GridLength(0);
                        ProfPhotoTextRow.Height = new GridLength(0);
                        ShareButsRow.Height = new GridLength(35);

                        ProfPhotoShareButsPanel.Children.Clear();
                        ProfPhotoShareButsPanel.Children.Add(_neverShare);

                        Height = 360;
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        ForewarMesSHardButsRow.Height = new GridLength(35);

                        ForwardMeesagesShareButsPanel.Children.Clear();
                        ForwardMeesagesShareButsPanel.Children.Add(_neverShare);

                        Height = 370;
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        BirtDateShardButsRow.Height = new GridLength(35);

                        BirthDateShareButsPanel.Children.Clear();
                        BirthDateShareButsPanel.Children.Add(_neverShare);

                        Height = 370;
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        BioShardButsRow.Height = new GridLength(35);

                        BioShareButsPanel.Children.Clear();
                        BioShareButsPanel.Children.Add(_neverShare);

                        Height = 370;
                        break;
                    }

            }
        }

        private void ContactsRadio_Checked(object sender, RoutedEventArgs e)
        {
            HideAllSubGrids();
            switch (_type)
            {
                case PrivacySettingType.PhoneNumber:
                    {
                        PhoneContacts.Height = new GridLength();
                        Height = 480;
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenOtherRow.Height = new GridLength();
                        ExecLastSeenButs.Height = new GridLength(65);
                        Height = 720;
                        break;
                    }
                case PrivacySettingType.ProfilePhotos:
                    {
                        PubPhotoButRow.Height = new GridLength(40);
                        ProfPhotoTextRow.Height = new GridLength(60);
                        ShareButsRow.Height = new GridLength(65);

                        ProfPhotoShareButsPanel.Children.Clear();
                        ProfPhotoShareButsPanel.Children.Add(_alwaysShare);
                        ProfPhotoShareButsPanel.Children.Add(_neverShare);

                        Height = 490;
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        ForewarMesSHardButsRow.Height = new GridLength(65);

                        ForwardMeesagesShareButsPanel.Children.Clear();
                        ForwardMeesagesShareButsPanel.Children.Add(_alwaysShare);
                        ForwardMeesagesShareButsPanel.Children.Add(_neverShare);

                        Height = 400;
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        BirtDateShardButsRow.Height = new GridLength(65);

                        BirthDateShareButsPanel.Children.Clear();
                        BirthDateShareButsPanel.Children.Add(_alwaysShare);
                        BirthDateShareButsPanel.Children.Add(_neverShare);

                        Height = 400;
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        BioShardButsRow.Height = new GridLength(65);

                        BioShareButsPanel.Children.Clear();
                        BioShareButsPanel.Children.Add(_alwaysShare);
                        BioShareButsPanel.Children.Add(_neverShare);

                        Height = 400;
                        break;
                    }

            }
        }

        private void NobodyRadio_Checked(object sender, RoutedEventArgs e)
        {
            HideAllSubGrids();
            switch (_type)
            {
                case PrivacySettingType.PhoneNumber:
                    {
                        PhoneNobody.Height = new GridLength();
                        Height = 500;
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenOtherRow.Height = new GridLength();
                        ExecLastSeenButs.Height = new GridLength(35);
                        Height = 690;
                        break;
                    }
                case PrivacySettingType.ProfilePhotos:
                    {
                        PubPhotoButRow.Height = new GridLength(40);
                        ProfPhotoTextRow.Height = new GridLength(60);
                        ShareButsRow.Height = new GridLength(35);

                        ProfPhotoShareButsPanel.Children.Clear();
                        ProfPhotoShareButsPanel.Children.Add(_alwaysShare);

                        Height = 460;
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        ForewarMesSHardButsRow.Height = new GridLength(35);

                        ForwardMeesagesShareButsPanel.Children.Clear();
                        ForwardMeesagesShareButsPanel.Children.Add(_alwaysShare);

                        Height = 370;
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        BirtDateShardButsRow.Height = new GridLength(35);

                        BirthDateShareButsPanel.Children.Clear();
                        BirthDateShareButsPanel.Children.Add(_alwaysShare);

                        Height = 370;
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        BioShardButsRow.Height = new GridLength(35);

                        BioShareButsPanel.Children.Clear();
                        BioShareButsPanel.Children.Add(_alwaysShare);

                        Height = 370;
                        break;
                    }
            }
        }

        private void HideAllSubGrids()
        {
            switch (_type)
            {
                case PrivacySettingType.PhoneNumber:
                    {
                        PhoneEverybody.Height = new GridLength(0);
                        PhoneContacts.Height = new GridLength(0);
                        PhoneNobody.Height = new GridLength(0);
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenEverybodyRow.Height = new GridLength(0);
                        LastSeenOtherRow.Height = new GridLength(0);
                        break;
                    }
                case PrivacySettingType.ProfilePhotos:
                    {
                        break;
                    }
            }
        }

        public void SetStartGridsSize()
        {
            MakeRowsLittle();
            switch (_type)
            {
                case PrivacySettingType.PhoneNumber:
                    {
                        PhoneNumberRow.Height = new GridLength();
                       
                        Height = 450;
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenRow.Height = new GridLength();

                        Height = 450;
                        break;
                    }
                case PrivacySettingType.ProfilePhotos:
                    {
                        ProfilePhotosRow.Height = new GridLength();

                        Height = 450;
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        ForwardedMessagesRow.Height = new GridLength();

                        Height = 400;
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        DateOfBirthRow.Height = new GridLength();

                        Height = 400;
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        BIORow.Height = new GridLength();

                        Height = 400;
                        break;
                    }
            };
        }

        public void MakeRowsLittle()
        {
            PhoneNumberRow.Height = new GridLength(0);
            LastSeenRow.Height = new GridLength(0);
            ProfilePhotosRow.Height = new GridLength(0);
            ForwardedMessagesRow.Height = new GridLength(0);
            DateOfBirthRow.Height = new GridLength(0);
            BIORow.Height = new GridLength(0);
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(_frame)).ClearThirdFrame();
        }
    }
}
