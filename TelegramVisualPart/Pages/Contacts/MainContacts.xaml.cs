using ControlzEx.Standard;
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
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ContactsControls;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace TelegramVisualPart.Pages.Contacts
{
    /// <summary>
    /// Логика взаимодействия для MainContacts.xaml
    /// </summary>
    public partial class MainContacts : Page
    {
        public event EventHandler ContactClicked;

        private ContactsPageAction _type;
        private TelSystem _system;
        private bool _isBlock;

        public MainContacts(ContactsPageAction type, TelSystem system, bool isBlock)
        {
            _type = type;
            _system = system;
            _isBlock = isBlock;

            InitializeComponent();
            SetBasicParams();

            SetContactsParams();

            SetLanguageText.SetUserContacts(this);
        }

        public async Task SetContactsParams()
        {
            List<UserContactcs> toAdd = !_isBlock ? _system.Contacts : 
                _system.Contacts.Where(x => !_system.LoggedUser.BlockedContacts.Select(y => y.Name).Contains(x.Name)).ToList();

            for(int i = 0; i < toAdd.Count; i++)
            {
                TelegramLib.MainClasses.User user = await ApiService.GetUserById(toAdd[i].ContactUserId);
                UserContact contact = new UserContact(user);

                contact.PreviewMouseDown += Contact_PreviewMouseDown;

                ListBoxItem item = new ListBoxItem
                {
                    Content = contact,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Tag = toAdd[i].Login 
                };
                ContactsListBox.Items.Add(item);
            }
        }

        private void Contact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UserContact contact) return;

            if (_isBlock) //block in logic
            {
                UserContactcs toBlock = 
                    _system.Contacts.Where(x => x.Name == contact.UserLogin.Text).First();

                _system.LoggedUser.BlockedContacts.Add(toBlock);

                ContactClicked?.Invoke(sender, EventArgs.Empty);
                ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
                return;
            }
            

            ContactClicked?.Invoke(sender, EventArgs.Empty);

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
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
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new AddContact(_system));
        }

        private void CloseBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
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

        public void AddBlockedContact()
        {
            //Set it here
        }
    }
}
