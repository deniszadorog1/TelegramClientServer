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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.UserParams;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.MyProfileControls;

namespace TelegramVisualPart.Pages.MyProfile
{
    /// <summary>
    /// Логика взаимодействия для MyProfileSettings.xaml
    /// </summary>
    public partial class MyProfileSettings : Page
    {
        private User _user;
        private TelSystem _system;
        public MyProfileSettings(User user, TelSystem system)
        {
            _user = user;
            _system = system;

            InitializeComponent();

            SetButtonsView();
            SetUserParams();

            SetUserImage();
        }

        public void SetUserImage()
        {
            UserImage.ImageSource = new BitmapImage(new Uri(FilesAction.GetUserImagePath(_user.GetFirstImageName().Name), UriKind.Absolute));
        }

        public void SetUserParams()
        {
            UserName.Text = _user.Login;

            HelperService.SetOnlineStatusInTextBox(LastSeenOnline, _user.IsOnline, _user.LastSeenOnline);
            

            BioTextBox.Text = _user.BIO;

            Name.AdditionalText.Text = _user.Name;
            PhoneNumber.AdditionalText.Text = _user.PhoneNumber;
            Username.AdditionalText.Text = _user.Login;

            PersonalChannelBut.AdditionalText.Text = "Not invented!";
            BirthdayBut.AdditionalText.Text = _user.BirthDay is null ? string.Empty : 
                (((DateTime)_user.BirthDay).Day + " " + 
                ((DateTime)_user.BirthDay).Month + " " + 
                (((DateTime)_user.BirthDay).Year == 1 ? " " : 
                ((DateTime)_user.BirthDay).Year))
            .ToString();
        }

        public void SetButtonsView()
        {
            GetBackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            Name.IconVis.Kind = PackIconKind.AccountCircleOutline;
            Name.ButName.Text = "Name";
            Name.AdditionalText.Text = "name here";

            PhoneNumber.IconVis.Kind = PackIconKind.TelephoneInTalk;
            PhoneNumber.ButName.Text = "Phone number";
            PhoneNumber.AdditionalText.Text = "phone numb here";

            Username.IconVis.Kind = PackIconKind.AlternateEmail;
            Username.ButName.Text = "Username";
            Username.AdditionalText.Text = "username here";

            PersonalChannelBut.IconVis.Kind = PackIconKind.Bullhorn;
            PersonalChannelBut.ButName.Text = "Personal channel";
            PersonalChannelBut.AdditionalText.Text = "Add";

            BirthdayBut.IconVis.Kind = PackIconKind.Gift;
            BirthdayBut.ButName.Text = "Date of Birth";
            BirthdayBut.AdditionalText.Text = "Add";
        }

        private void Buts_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.White;
        }

        private void Buts_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.Gray;
        }

        private async void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            await ApiService.UpdateUser(_user);
            await SignalRService.UpdateContact(_user);
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private async void GetBackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            await ApiService.UpdateUser(_user);
            await SignalRService.UpdateContact(_user);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new LoggedUserProfile(_user, _system));
        }

        private async void BioTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WordCount.Text = (BioTextBox.MaxLength - BioTextBox.Text.Length).ToString();
            _user.BIO = BioTextBox.Text;

            await ApiService.UpdateUser(_user);
            await SignalRService.UpdateContact(_user);
        }

        private void But_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is MyProfileSettingsButton but)
            {
                Page? page = GetPageByName(but.Name.ToString());
                if (page is null) return;
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);
            }
        }

        public Page? GetPageByName(string name)
        {
            return name == Name.Name.ToString() ? new SetInformation.SetNameSurname(_user) :
                name == Username.Name.ToString() ? new SetInformation.SetUsername(_user) : 
                name == PhoneNumber.Name.ToString() ? new SetInformation.SetPhoneNumber(_user) : 
                name == BirthdayBut.Name.ToString() ? new SetInformation.SetBirthDate(_user) : null;
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
    }
}
