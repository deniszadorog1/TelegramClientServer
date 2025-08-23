using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.Enums.Settings.PrivacyAndSecurity;
using TelegramLib.MainClasses;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.DifferButs;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity
{
    /// <summary>
    /// Логика взаимодействия для SetPrivacyByType.xaml
    /// </summary>
    public partial class SetPrivacyByType : Page
    {
        private PrivacySettingType _type;
        private PrivAndSecSettings _settings;
        private List<UserContactcs> _contacts;
        private TelSystem _system;

        public SetPrivacyByType(PrivacySettingType type,
            PrivAndSecSettings settings, List<UserContactcs> contacts,
            TelSystem system)
        {
            _settings = settings;
            _type = type;
            _contacts = contacts;
            _system = system;

            InitializeComponent();

            SetStartGridsSize();
            SetVisualPart();
            //EverybodyRadio.IsChecked = true;
        }

        public void SetVisualPart()
        {
            switch (_type)
            {
                case PrivacySettingType.PhoneNumber:
                    {
                        SetPhoneGridBlocks();
                        SetWhoCanSeeParam(_settings.PhonePrivacy.ShareType);

                        if (_settings.PhonePrivacy.WhoCanSearch == AllOrNone.Everybody)
                            EverybodyRadioSub.IsChecked = true;
                        else if (_settings.PhonePrivacy.WhoCanSearch == AllOrNone.Contacts)
                            MyContactsRadioSub.IsChecked = true;

                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        SetLastSeenBlocks();
                        SetWhoCanSeeParam(_settings.LastSeenPrivacy.ShareType);
                        break;
                    }
                case PrivacySettingType.ProfilePhotos:
                    {
                        SetProfilePhotosBlocks();
                        SetWhoCanSeeParam(_settings.ProfPhotoPrivacy.ShareType);
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        SetForwardMessagesBlocks();
                        SetWhoCanSeeParam(_settings.ForwardMesPrivacy.ShareType);
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        SetDateOfBirthBlocks();
                        SetWhoCanSeeParam(_settings.DateBirthPrivacy.ShareType);
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        SetBioBlocks();
                        SetWhoCanSeeParam(_settings.BioPrivacy.ShareType);
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
            SetPubPhotoBut.IconType.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
            SetPubPhotoBut.ButName.Text = "Set public photo";
            SetPubPhotoBut.ButName.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

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

            HideReedTimeToggleBut.Toggle.IsChecked = _settings.LastSeenPrivacy.IsHideReadAction;
            HideReedTimeToggleBut.Toggle.Checked += ToggleButton_StateChanged;
            HideReedTimeToggleBut.Toggle.Unchecked += ToggleButton_StateChanged;
        }

        private void ToggleButton_StateChanged(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleButton;
            _settings.LastSeenPrivacy.IsHideReadAction = (bool)toggle.IsChecked;
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

        public void SetWhoCanSeeParam(ShareWith toShare)
        {
            if (toShare == ShareWith.Everybody) EverybodyRadio.IsChecked = true;
            else if (toShare == ShareWith.Contacts) ContactsRadio.IsChecked = true;
            else if (toShare == ShareWith.Nobody) NobodyRadio.IsChecked = true;
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
                        _settings.PhonePrivacy.WhoCanSearch = AllOrNone.Everybody;
                        SetShareParam(_settings.PhonePrivacy, ShareWith.Everybody);
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenEverybodyRow.Height = new GridLength();
                        Height = 450;
                        SetShareParam(_settings.LastSeenPrivacy, ShareWith.Everybody);
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
                        SetShareParam(_settings.ProfPhotoPrivacy, ShareWith.Everybody);
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        ForewarMesSHardButsRow.Height = new GridLength(35);

                        ForwardMeesagesShareButsPanel.Children.Clear();
                        ForwardMeesagesShareButsPanel.Children.Add(_neverShare);

                        Height = 370;
                        SetShareParam(_settings.ForwardMesPrivacy, ShareWith.Everybody);
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        BirtDateShardButsRow.Height = new GridLength(35);

                        BirthDateShareButsPanel.Children.Clear();
                        BirthDateShareButsPanel.Children.Add(_neverShare);

                        Height = 370;
                        SetShareParam(_settings.DateBirthPrivacy, ShareWith.Everybody);
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        BioShardButsRow.Height = new GridLength(35);

                        BioShareButsPanel.Children.Clear();
                        BioShareButsPanel.Children.Add(_neverShare);

                        Height = 370;
                        SetShareParam(_settings.BioPrivacy, ShareWith.Everybody);
                        break;
                    }
            }
        }

        public async Task SetShareParam(PrivacySub param, ShareWith value)
        {
            param.ShareType = value;
            //await ApiService.UpdatePrivSettings(_settings);
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
                        SetShareParam(_settings.PhonePrivacy, ShareWith.Contacts);
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenOtherRow.Height = new GridLength();
                        ExecLastSeenButs.Height = new GridLength(65);
                        Height = 720;
                        SetShareParam(_settings.LastSeenPrivacy, ShareWith.Contacts);
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
                        SetShareParam(_settings.ProfPhotoPrivacy, ShareWith.Contacts);
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        ForewarMesSHardButsRow.Height = new GridLength(65);

                        ForwardMeesagesShareButsPanel.Children.Clear();
                        ForwardMeesagesShareButsPanel.Children.Add(_alwaysShare);
                        ForwardMeesagesShareButsPanel.Children.Add(_neverShare);

                        Height = 400;
                        SetShareParam(_settings.ForwardMesPrivacy, ShareWith.Contacts);
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        BirtDateShardButsRow.Height = new GridLength(65);

                        BirthDateShareButsPanel.Children.Clear();
                        BirthDateShareButsPanel.Children.Add(_alwaysShare);
                        BirthDateShareButsPanel.Children.Add(_neverShare);

                        Height = 400;
                        SetShareParam(_settings.DateBirthPrivacy, ShareWith.Contacts);
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        BioShardButsRow.Height = new GridLength(65);

                        BioShareButsPanel.Children.Clear();
                        BioShareButsPanel.Children.Add(_alwaysShare);
                        BioShareButsPanel.Children.Add(_neverShare);

                        Height = 400;
                        SetShareParam(_settings.BioPrivacy, ShareWith.Contacts);
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
                        SetShareParam(_settings.PhonePrivacy, ShareWith.Nobody);
                        break;
                    }
                case PrivacySettingType.LastSeen:
                    {
                        LastSeenOtherRow.Height = new GridLength();
                        ExecLastSeenButs.Height = new GridLength(35);
                        Height = 690;
                        SetShareParam(_settings.LastSeenPrivacy, ShareWith.Nobody);
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
                        SetShareParam(_settings.ProfPhotoPrivacy, ShareWith.Nobody);
                        break;
                    }
                case PrivacySettingType.ForwardedMessages:
                    {
                        ForewarMesSHardButsRow.Height = new GridLength(35);

                        ForwardMeesagesShareButsPanel.Children.Clear();
                        ForwardMeesagesShareButsPanel.Children.Add(_alwaysShare);

                        Height = 370;
                        SetShareParam(_settings.ForwardMesPrivacy, ShareWith.Nobody);
                        break;
                    }
                case PrivacySettingType.DateBirth:
                    {
                        BirtDateShardButsRow.Height = new GridLength(35);

                        BirthDateShareButsPanel.Children.Clear();
                        BirthDateShareButsPanel.Children.Add(_alwaysShare);

                        Height = 370;
                        SetShareParam(_settings.DateBirthPrivacy, ShareWith.Nobody);
                        break;
                    }
                case PrivacySettingType.Bio:
                    {
                        BioShardButsRow.Height = new GridLength(35);

                        BioShareButsPanel.Children.Clear();
                        BioShareButsPanel.Children.Add(_alwaysShare);

                        Height = 370;
                        SetShareParam(_settings.BioPrivacy, ShareWith.Nobody);
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

        private async void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            //Set in DB
            await ApiService.UpdatePrivSettings(_settings);       
            
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();

            UpdateStatesWithSignalR();
        }

        public void UpdateStatesWithSignalR()
        {
            SignalRService.SetUserPhonenumberVisibility(
                _settings.PhonePrivacy.ShareType == ShareWith.Nobody ? false : true, 
                _system.LoggedUser);

            SignalRService.SetContactLastSeenVisState(_system.LoggedUser);
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void Exps_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not EnumPrivacyButton openExps) return;

            if (openExps == PhoneEverybodyUsersExcepts) SetToChoosePage(ChooseType.NeverShare, _settings.PhonePrivacy);
            else if (openExps == PhoneContactAlwaysShareBut) SetToChoosePage(ChooseType.AlwaysShare, _settings.PhonePrivacy);
            else if (openExps == PhoneContactNeverShareBut) SetToChoosePage(ChooseType.NeverShare, _settings.PhonePrivacy);
            else if (openExps == PhoneNobodyAlwaysShareBut) SetToChoosePage(ChooseType.AlwaysShare, _settings.PhonePrivacy);

            else if (openExps == LastSeenEverybodyUsersExcepts) SetToChoosePage(ChooseType.NeverShare, _settings.LastSeenPrivacy);
            else if (openExps == LastSeenOtherAlwaysShare) SetToChoosePage(ChooseType.AlwaysShare, _settings.LastSeenPrivacy);
            else if (openExps == LastSeenOtherNeverShare) SetToChoosePage(ChooseType.NeverShare, _settings.LastSeenPrivacy);

            else if (openExps == ProfPhotosAlwaysShareBut) SetToChoosePage(ChooseType.AlwaysShare, _settings.ProfPhotoPrivacy);
            else if (openExps == ProfPhotosNeverShareBut) SetToChoosePage(ChooseType.NeverShare, _settings.ProfPhotoPrivacy);

            else if (openExps == ForwardMeesagesAlwaysShareBut) SetToChoosePage(ChooseType.AlwaysShare, _settings.ForwardMesPrivacy);
            else if (openExps == ForwardMeesagesNeverShareBut) SetToChoosePage(ChooseType.NeverShare, _settings.ForwardMesPrivacy);

            else if (openExps == BirthDateAlwaysShareBut) SetToChoosePage(ChooseType.AlwaysShare, _settings.DateBirthPrivacy);
            else if (openExps == BirthDateNeverShareBut) SetToChoosePage(ChooseType.NeverShare, _settings.DateBirthPrivacy);

            else if (openExps == BioAlwaysShareBut) SetToChoosePage(ChooseType.AlwaysShare, _settings.BioPrivacy);
            else if (openExps == BioNeverShareBut) SetToChoosePage(ChooseType.NeverShare, _settings.BioPrivacy);
        }

        public void SetToChoosePage(ChooseType shareType, PrivacySub sub)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new ToChooseChats(shareType, _contacts, sub));
        }

        public PrivacySub GetSettingsTypeByPrivButton(EnumPrivacyButton but)
        {
            if (but == PhoneEverybodyUsersExcepts) return _settings.PhonePrivacy;
            return new PhoneNumberSub();
        }

        private void EverybodyRadioSub_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _settings.PhonePrivacy.WhoCanSearch = AllOrNone.Everybody;
        }

        private void MyContactsRadioSub_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _settings.PhonePrivacy.WhoCanSearch = AllOrNone.Contacts;
        }

        private void SetPubPhotoBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ToChooseImage();
        }

        public void ToChooseImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose image or video",
                Filter = "Image and Video files|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    FilesAction.AddNewUserImage(filePath);

                    string name = System.IO.Path.GetFileName(filePath);

                    //Set it To first place
                    _system.LoggedUser.UserImages.Insert(0,
                         new TelegramLib.MainClasses.UserParams.UserImage(name, DateTime.Now));

                    //Add image to User Images (in db + signalRing this)
                    AddUserImage(System.IO.Path.GetFileName(name));
                }
            }
        }

        private void AddUserImage(string userImageName)
        {
            //add image in db
            ApiService.AddUserImage(_system.LoggedUser, userImageName);

            //Update this with signalR
            SignalRService.AddUserImage(_system.LoggedUser);
        }
    }
}
