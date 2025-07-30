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

namespace TelegramVisualPart.Pages.Contacts
{
    /// <summary>
    /// Логика взаимодействия для AddContact.xaml
    /// </summary>
    public partial class AddContact : Page
    {
        private TelSystem _system;
        public AddContact(TelSystem system)
        {
            _system = system; 
            InitializeComponent();
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

        private void CreateBut_Click(object sender, RoutedEventArgs e)
        {
            //Set adding contact
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new MainContacts(Enums.ContactsPageAction.AddContact, _system, false));
        }

        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            return text.All(char.IsDigit);
        }

        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PhoneBox.Text.First() == '+') return;
            PhoneBox.Text = "+" + new string(PhoneBox.Text.Where(x => x != '+').ToArray());

        }
    }
}
