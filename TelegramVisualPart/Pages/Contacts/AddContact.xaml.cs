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

            SetLanguageText.SetAddContact(this);
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

        private async void CreateBut_Click(object sender, RoutedEventArgs e)
        {
            User newContact = await ApiService.GetUserByPhoneNumber(PhoneBox.Text);

            if (newContact is null || string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(LastnameBox.Text) ||
                await ApiService.IsContactExist(_system.LoggedUser.Id, newContact.Id) ||
                _system.LoggedUser.PhoneNumber == PhoneBox.Text.Trim('+'))
            {
                ClearFields();
                return;
            }

            await ToAddContact(newContact);

            ClearFields();

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        public async Task ToAddContact(User newContact)
        {
            bool isUserOnline = await ApiService.IsUserOnline(newContact.Id);

            //is online
            if (isUserOnline)
            {
                await SetLoggedUserAsContact(newContact);

                await SignalRService.AddContact(newContact, _system.LoggedUser);
                return;
            }

            //is addable contact is offline

            //for logged user (which is online)
            await SetLoggedUserAsContact(newContact);

            //for addable contact(which is offline)
            await AddNewContactIsItsOffline(newContact);
        }

        public async Task AddNewContactIsItsOffline(User newContact)
        {
            //Add conatct in system
            UserContactcs contact = new UserContactcs(-1,
                _system.LoggedUser.Name,
                _system.LoggedUser.Surname,
                _system.LoggedUser.Login,
                _system.LoggedUser.BirthDay,
                _system.LoggedUser.BIO,
                _system.LoggedUser.PhoneNumber,
                _system.LoggedUser.LastSeenOnline, true,
                _system.LoggedUser.UserImages, null, true);

            contact.ContactUserId = _system.LoggedUser.Id;

            //add contact in db
            await ApiService.AddContact(newContact.Id, contact);
            contact = await ApiService.GetLastUserContact(newContact.Id);

            //Add chat in DB
            bool isChatExist = await ApiService.IsChatExist(newContact.Id, contact.ContactUserId);
            if (!isChatExist)
            {
                await ApiService.AddNewChat(newContact.Id, contact.ContactUserId);
            }
        }

        public async Task SetLoggedUserAsContact(User newContact)
        {
            UserContactcs contact = new UserContactcs(-1, NameBox.Text, LastnameBox.Text, newContact.Login, newContact.BirthDay,
                newContact.BIO, newContact.PhoneNumber, newContact.LastSeenOnline, true, newContact.UserImages, null, false);

            contact.ContactUserId = newContact.Id;

            await ApiService.AddContact(_system.LoggedUser.Id, contact);

            contact = await ApiService.GetLastUserContact(_system.LoggedUser.Id);

            _system.Contacts.Add(contact);

            //Add chat in DB
            bool isCont = _system.Chats.Select(x => x.Chatter.Id).Contains(contact.ContactUserId);
            if (!isCont)
            {
                await ApiService.AddNewChat(_system.LoggedUser.Id, contact.ContactUserId);
                AddChatInTelSystem(_system.LoggedUser.Id, contact.ContactUserId);
            }

            //To update chat(UserTalkMessage)
            //((MainWindow)Window.GetWindow(this)).UpdateUserTalkMessage(contact);
        }

        private async void AddChatInTelSystem(int userId, int contactId)
        {
            UserChat toAdd = await ApiService.GetChatByUserAndSenderId(userId, contactId);
            _system.AddChat(toAdd);
        }

        private void ClearFields()
        {
            NameBox.Text = string.Empty;
            LastnameBox.Text = string.Empty;
            PhoneBox.Text = string.Empty;
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
            if (PhoneBox.Text.Count() == 0 || PhoneBox.Text.First() == '+') return;
            //PhoneBox.Text = /*"+" +*/ new string(PhoneBox.Text.Where(x => x != '+').ToArray());

        }
    }
}
