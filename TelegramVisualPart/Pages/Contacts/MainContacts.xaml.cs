using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TelegramVisualPart.Enums;

namespace TelegramVisualPart.Pages.Contacts
{
    /// <summary>
    /// Логика взаимодействия для MainContacts.xaml
    /// </summary>
    public partial class MainContacts : Page
    {
        private ContactsPageAction _type;
        public MainContacts(ContactsPageAction type)
        {
            _type = type;
            InitializeComponent();
            SetBasicParams();
        }

        public void SetBasicParams()
        {
            SortBut.IconType.Kind = PackIconKind.HamburgerMenu;

            ClearBox.IconType.Kind = PackIconKind.Close;
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

        private void SortBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set sorting action
        }

        private void SortBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.White;
        }

        private void SortBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon) icon.Foreground = Brushes.Gray;
        }

        private void AddContactBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new AddContact());
        }

        private void CloseBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void UserContact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (_type)
            {
                case ContactsPageAction.AddContact:
                    {
                        break;
                    }
                case ContactsPageAction.Block:
                    {
                        break;
                    }
            }
        }

        private void ClearBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Text = string.Empty;
        }
    }
}
