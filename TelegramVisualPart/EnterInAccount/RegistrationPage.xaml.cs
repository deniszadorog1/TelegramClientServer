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
using TelegramLib.Models;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;
using User = TelegramLib.MainClasses.User;

namespace TelegramVisualPart.EnterInAccount
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void GetBackGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ReuturnToPrevious.TextDecorations = TextDecorations.Underline;
        }

        private void GetBackGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ReuturnToPrevious.TextDecorations = null;
        }

        private void GetBackGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).
                SetMainFrameContent(new EnterPage());
        }

        private async void RegisterBut_Click(object sender, RoutedEventArgs e)
        {
            //if params are exist
            //login, password, phone number

            bool isExist = await ApiService.IsUserRegistrationParamsAreExist(LoginBox.Text, PhoneBox.Text);

            //is somthing is emty
            if (string.IsNullOrWhiteSpace(LoginBox.Text) ||
               string.IsNullOrWhiteSpace(PasswordBox.Text) ||
               string.IsNullOrWhiteSpace(PhoneBox.Text) ||
               string.IsNullOrWhiteSpace(NameBox.Text) ||
               string.IsNullOrWhiteSpace(SurnameBox.Text) ||
               PhoneBox.Text.Where(x => char.IsLetter(x)).Any() || //problems with boxes
               isExist) //params are exist
            {
                MessageBox.Show("Cant be add!");
                ClearBoxes();
                return;
            }
            await RegisterUserInDb();
        }

        private async Task RegisterUserInDb()
        {
            User user = new User();

            await ApiService.AddNewUser(LoginBox.Text, PasswordBox.Text, NameBox.Text, SurnameBox.Text, PhoneBox.Text, null);

            user = await ApiService.GetUser(LoginBox.Text, PasswordBox.Text);

            await ApiService.AddUserBasicColor(user.Id);
            await ApiService.AddUserSettings(user.Id);


            MessageBox.Show("New user was Created!");

            ClearBoxes();
        }

        private void ClearBoxes()
        {
            LoginBox.Text = string.Empty;
            PasswordBox.Text = string.Empty;
            PhoneBox.Text = string.Empty;
            NameBox.Text = string.Empty;
            SurnameBox.Text = string.Empty;
        }

    }
}
