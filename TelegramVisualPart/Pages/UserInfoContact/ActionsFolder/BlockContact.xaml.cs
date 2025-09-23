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
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.UserInfoContact.ActionsFolder
{
    /// <summary>
    /// Логика взаимодействия для BlockContact.xaml
    /// </summary>
    public partial class BlockContact : Page
    {
        private TelSystem _system;
        private User _contact;

        public BlockContact(TelSystem system, User contact)
        {
            _system = system;
            _contact = contact;

            InitializeComponent();

            SetBaseParams();
        }

        public void SetBaseParams()
        {
            UserContactName.Text = _contact.Name;
        }

        private async void BlockBut_Click(object sender, RoutedEventArgs e)
        {
            //Added in system
            _system.LoggedUser.AddBlockedContact(_contact);

            //Add in db
            await ApiService.AddBlockedContact(_system.LoggedUser.Id, _contact.Id);

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).SetFramesAfterBlockingContact();
        }

        private void BlockBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                    (SolidColorBrush)Application.Current.Resources["CloseButBg"];
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                    (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }
    }
}
