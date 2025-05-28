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
using TelegramVisualPart.UserControls.MyProfileControls;

namespace TelegramVisualPart.Pages.MyProfile
{
    /// <summary>
    /// Логика взаимодействия для MyProfileSettings.xaml
    /// </summary>
    public partial class MyProfileSettings : Page
    {
        private Frame _frame;
        public MyProfileSettings(Frame frame)
        {
            _frame = frame;
            InitializeComponent();

            SetButtonsView();
        }

        public void SetButtonsView()
        {
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

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(_frame)).ClearSecFrame();
        }

        private void GetBackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(_frame)).SetSecondaryFrame(new LoggedUserProfile(_frame));
        }

        private void BioTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WordCount.Text = (BioTextBox.MaxLength - BioTextBox.Text.Length).ToString();
        }

        private void But_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is MyProfileSettingsButton but)
            {
                Page page = GetPageByName(but.Name.ToString());
                if (page is null) return;

                ((MainWindow)Window.GetWindow(_frame)).SetThirdFrame(page);
            }
        }

        public Page GetPageByName(string name)
        {
            return name == Name.Name.ToString() ? new SetInformation.SetNameSurname(_frame) :
                name == Username.Name.ToString() ? new SetInformation.SetUsername(_frame) : 
                name == PhoneNumber.Name.ToString() ? new SetInformation.SetPhoneNumber(_frame) : null;
        }
    }
}
