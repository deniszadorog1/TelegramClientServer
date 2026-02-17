using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
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
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.MyProfile.SetInformation
{
    /// <summary>
    /// Логика взаимодействия для SetNameSurname.xaml
    /// </summary>
    public partial class SetNameSurname : Page
    {
        private User _user;
        public SetNameSurname(User user)
        {
            _user = user;
            InitializeComponent();

            SetBasicParams();

            SetLanguageText.SetNameSurname(this);
        }

        private void SetBasicParams()
        {
            FirstNameBox.Text = _user.Name;
            LastNameBox.Text = _user.Surname;
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

        private int _minAmountOfNums = 5;

        private async void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            //Clean text
            FirstNameBox.Text = VisHelper.CleanText(FirstNameBox.Text);
            LastNameBox.Text = VisHelper.CleanText(LastNameBox.Text);
            
            if (FirstNameBox.Text.Count() <= _minAmountOfNums ||
                string.IsNullOrWhiteSpace(FirstNameBox.Text) ||

                LastNameBox.Text.Count() <= _minAmountOfNums || 
                string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Stop acting weird!!!");

                FirstNameBox.Text = _user.Name;
                LastNameBox.Text = _user.Surname;
                return;
            }

            //Set changings in DB
            await ApiService.UpdateUserNameSurname(_user.Id, FirstNameBox.Text, LastNameBox.Text);

            //Set save action
            _user.Name = FirstNameBox.Text;
            _user.Surname = LastNameBox.Text;

            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }


    }
}
