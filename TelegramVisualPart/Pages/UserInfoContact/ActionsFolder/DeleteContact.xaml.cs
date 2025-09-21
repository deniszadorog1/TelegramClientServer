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
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.UserInfoContact.ActionsFolder
{
    /// <summary>
    /// Логика взаимодействия для DeleteContact.xaml
    /// </summary>
    public partial class DeleteContact : Page
    {
        private UserContactcs _contact;
        private TelSystem _system;
        public DeleteContact(UserContactcs contact, TelSystem system)
        {
            _contact = contact;
            _system = system;

            InitializeComponent();

            SetBasicParams();
        }

        private void SetBasicParams()
        {
            BgBrush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(_contact.GetFirstImageName().Name), UriKind.Absolute));

            UsernamePlace.Text = _contact.Name;
        }

        private async void DeleteBut_Click(object sender, RoutedEventArgs e)
        {
            await ApiService.RemoveContact(_contact, _system.LoggedUser);

            //_system.RemoveContact(_contact);

            //update window after contact remove 
            ((MainWindow)Window.GetWindow(this)).UpdateDeletedUser(_contact);

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void DeleteBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["CloseButBg"];
        }

        private void DeleteBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }
    }
}
