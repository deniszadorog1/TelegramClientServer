using System;
using System.Collections.Generic;
using System.Drawing;
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
using TelegramLib.Services;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ContactsControls;

namespace TelegramVisualPart.Pages.UserInfoContact.ActionsFolder
{
    /// <summary>
    /// Логика взаимодействия для EditUserContact.xaml
    /// </summary>
    public partial class EditUserContact : Page
    {
        private UserContactcs _contact;
        private User _user;
        public EditUserContact(User user, UserContactcs contact)
        {
            _contact = contact;
            _user = user;

            InitializeComponent();

            SetBasicParams();
        }

        private void SetBasicParams()
        {
            BgBrush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(_contact.GetFirstImageName().Name), UriKind.Absolute));

            FirstNameBox.Text = _contact.Name;
            LastNameBox.Text = _contact.Surname;

            PhoneNumberBox.Text = _contact.PhoneNumber;
            LastSeenBox.Text = _contact.LastSeen is null ? "recently" :
                $"{_contact.LastSeen.Value.Day}.{_contact.LastSeen.Value.Month}.{_contact.LastSeen.Value.Year}";
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                               (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private async void DoneBut_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text)) return;

            _contact.Name = FirstNameBox.Text;
            _contact.Surname = LastNameBox.Text;

            await ApiService.UpdateContact(_user.Id, _contact);

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }
    }
}
