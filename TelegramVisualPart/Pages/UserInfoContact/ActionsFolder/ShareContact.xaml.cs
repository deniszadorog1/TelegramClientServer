using Microsoft.AspNetCore.Mvc;
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
using TelegramVisualPart.UserControls.ContactsControls;

namespace TelegramVisualPart.Pages.UserInfoContact.ActionsFolder
{
    /// <summary>
    /// Логика взаимодействия для ShareContact.xaml
    /// </summary>
    public partial class ShareContact : Page
    {
        private TelSystem _system;
        private UserContactcs _contact;
        public ShareContact(TelSystem system, UserContactcs contact)
        {
            _system = system;
            _contact = contact;

            InitializeComponent();

            SetContactsToShareWith();
        }

        private User _checkedContact;
        public async Task SetContactsToShareWith()
        {
            ToShareWithPanel.Children.Clear();

            for (int i = 0; i < _system.Contacts.Count; i++)
            {
                //is contact has temp contact in contacts (WTF)
                bool isContains = await ApiService.IsContactContainsInContacts(_system.Contacts[i], _contact);
                if (isContains || _contact is null || _contact.Id == _system.Contacts[i].Id) continue;

                _checkedContact = await ApiService.GetUserById(_system.Contacts[i].ContactUserId);
                UserContact toAdd = new UserContact(_checkedContact);
                toAdd.Tag = _system.GetChatByChatterId(_system.Contacts[i].ContactUserId).Id;

                toAdd.MouseEnter += UserControl_MouserEnter;
                toAdd.MouseLeave += UserControl_MouseLeave;

                toAdd.PreviewMouseDown += UserControl_PreviewMouseDown;

                //add in contacts if not contatins
                ToShareWithPanel.Children.Add(toAdd);
            }
        }

        public async void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UserContact contact) return;

            int.TryParse(contact.Tag.ToString(), out int tagId);

            //Send share message
            ((MainWindow)Window.GetWindow(this))
                .SetSharedContact(tagId, _contact);

            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);

            return;
            //set in db 
            User user = await ApiService.GetUserById(_contact.ContactUserId);

            //set in system
            AddContacts(user, _checkedContact);
            SetContactsToShareWith();
        }

        public void AddContacts(User user, User toAdd)
        {
            AddContact(user, toAdd);
            AddContact(toAdd, user);
        }

        public async void AddContact(User first, User second)
        {
            if (await ApiService.IsUserOnline(first.Id))
            {
                await SignalRService.AddContact(first, second);
                return;
            }
            AddContactIfOffline(first, second);
        }

        private async void AddContactIfOffline(User user, User toAdd)
        {
            //Add conatct in system
            UserContactcs contact = new UserContactcs(-1, toAdd.Name, toAdd.Surname, toAdd.Login, toAdd.BirthDay,
                toAdd.BIO, toAdd.PhoneNumber, toAdd.LastSeenOnline, true, toAdd.UserImages, null, true);

            contact.ContactUserId = toAdd.Id;
            //add cotact in db

            await ApiService.AddContact(user.Id, contact);

            contact = await ApiService.GetLastUserContact(user.Id);

            //Add chat in DB
            await ApiService.AddNewChat(user.Id, contact.Id);

        }

        public void UserControl_MouserEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        public void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void UserContact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                              (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            ClearText.Foreground = Brushes.White;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            ClearText.Foreground = Brushes.Gray;
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
        }
    }
}
