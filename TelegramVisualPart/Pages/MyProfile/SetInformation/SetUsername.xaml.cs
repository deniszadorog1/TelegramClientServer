using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.MyProfile.SetInformation
{
    /// <summary>
    /// Логика взаимодействия для SetUsername.xaml
    /// </summary>
    public partial class SetUsername : Page
    {
        private const int _minAmountOfSymbols = 2;
        private User _user;
        public SetUsername(User user)
        {
            _user = user;
            InitializeComponent();

            SetBasicParams();

            SetLanguageText.SetUsername(this);
        }

        public void SetBasicParams()
        {
            UserNameBox.Text = _user.Login;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private async void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameBox.Text.Count() <= _minAmountOfSymbols ||
                string.IsNullOrWhiteSpace(UserNameBox.Text))
            {
                MessageBox.Show("Stop acting weird!!");
                UserNameBox.Text = _user.Login;
                return;
            };

            //Set checks if this is exist
            if (await ApiService.IsLoginExist(UserNameBox.Text))
            {
                MessageBox.Show("This is already exist");
                UserNameBox.Text = _user.Login;
                return;
            };

            //+ Set Changings in DB
            await ApiService.UpdateUserLogin(_user.Id, UserNameBox.Text);
            
            _user.Login = UserNameBox.Text;

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
        }

        private void UserNameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[a-zA-Z0-9_]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void UserNameBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }
    }
}
