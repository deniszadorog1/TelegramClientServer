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

namespace TelegramVisualPart.Pages.MyProfile.SetInformation
{
    /// <summary>
    /// Логика взаимодействия для SetUsername.xaml
    /// </summary>
    public partial class SetUsername : Page
    {
        private User _user;
        public SetUsername(User user)
        {
            _user = user;
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            UserNameBox.Text = _user.UserName;
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

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            //Set checks if this is exist
            //+ Set Changings in DB

            _user.UserName = UserNameBox.Text;

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
        }
    }
}
