using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Security;
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
using TelegramLib.MainClasses.Messages;
using TelegramLib.Services;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ContactsControls;
using Image = System.Windows.Controls.Image;

namespace TelegramVisualPart.Pages.UserInfoContact.ActionsFolder
{
    /// <summary>
    /// Логика взаимодействия для EditUserContact.xaml
    /// </summary>
    public partial class EditUserContact : Page
    {
        private UserContactcs _contact;
        private User _user;

        private TelSystem _system;
        bool _isAddNewContact = false;
        public EditUserContact(User user, TelSystem system) //Add new Contact
        {
            _isAddNewContact = true;
            _user = user;
            _system = system;

            InitializeComponent();

            AddNewUserBasicParams();
            SetMaskParamRowsVis();
        }

        public void AddNewUserBasicParams()
        {
            PageNameBlock.Text = "New Contact";
            UserLogin.Text = _user.Login;

            BgBrush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(System.IO.Path.GetFileName(_user.GetFirstImageName().Name)), UriKind.Absolute));

            FirstNameBox.Text = _user.Name;
            LastNameBox.Text = _user.Surname;

            PhoneNumberBox.Text = _user.PhoneNumber;

            LastSeenBox.Text = /*_user.LastSeenOnline is null ? "recently" :*/
                $"{_user.LastSeenOnline.Day}.{_user.LastSeenOnline.Month}.{_user.LastSeenOnline.Year}";
        }

        public EditUserContact(User user, UserContactcs contact, TelSystem system)
        {
            _contact = contact;
            _user = user;
            _system = system;

            InitializeComponent();

            SetBasicParams();
            SetRemoveMaskLine();
            SetMaskParamRowsVis();
        }


        private void SetBasicParams()
        {
            BgBrush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(_contact.GetFirstImageName().Name), UriKind.Absolute));

            UserLogin.Text = _user.Login;

            FirstNameBox.Text = _contact.Name;
            LastNameBox.Text = _contact.Surname;

            PhoneNumberBox.Text = _contact.PhoneNumber;
            LastSeenBox.Text = _contact.LastSeen is null ? "recently" :
                $"{_contact.LastSeen.Value.Day}.{_contact.LastSeen.Value.Month}.{_contact.LastSeen.Value.Year}";
        }

        public void SetMaskParamRowsVis()
        {
            if (_contact is not null) return;

            Height -= SetContactInfoRow.Height.Value + SetContactMaskRow.Height.Value;

            SetContactInfoRow.Height = new GridLength(0);
            SetContactMaskRow.Height = new GridLength(0);
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                               (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private async void DoneBut_Click(object sender, RoutedEventArgs e)
        {
            if (_isAddNewContact)
            {
                //Add new contact
                ToAddContact(_user);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text)) return;

                _contact.Name = FirstNameBox.Text;
                _contact.Surname = LastNameBox.Text;

                await ApiService.UpdateContact(_user.Id, _contact);

                //Update Contact name - surname
                ((MainWindow)Window.GetWindow(this)).UpdateContactParams(_contact);

                //Set in boss page(if chat on other window)          
                ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
            }
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        public async Task ToAddContact(User newContact)
        {
            bool isUserOnline = await ApiService.IsUserOnline(newContact.Id);

            //is online
            if (isUserOnline)
            {
                await AddContactIfContactOnline(newContact);

                SetWindowParams();
                return;
            }

            //is addable contact is offline

            //for logged user (which is online)
            await AddContactIfContactOnline(newContact);

            //for addable contact(which is offline)
            await AddContactIfContactOffline(newContact);

            SetWindowParams();
        }

        public void SetWindowParams()
        {

            //Update Contact name - surname
            ((MainWindow)Window.GetWindow(this)).UpdateContactParams(_contact);

            //Set in boss page(if chat on other window)          
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);

        }

        public async Task AddContactIfContactOffline(User newContcat)
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

            //add cotact in db
            await ApiService.AddContact(newContcat.Id, contact);

            //contact = await ApiService.GetLastUserContact(newContcat.Id);

            //Add chat in DB must exist
        }

        public async Task AddContactIfContactOnline(User newContact)
        {
            UserContactcs contact = new UserContactcs(-1, FirstNameBox.Text, LastNameBox.Text, newContact.Login, newContact.BirthDay,
                newContact.BIO, newContact.PhoneNumber, newContact.LastSeenOnline, true, newContact.UserImages, null, false);

            contact.ContactUserId = newContact.Id;

            await ApiService.AddContact(_system.LoggedUser.Id, contact);

            _contact = await ApiService.GetLastUserContact(_system.LoggedUser.Id);

            if (!_system.IsContactExistByUserId(contact.ContactUserId)) _system.Contacts.Add(contact);

            //Add chat in DB. (MUST EXIST)

            //Add backwards (add temp user in added user contact);
            await SignalRService.AddContact(newContact, _system.LoggedUser);

            //To update chat(UserTalkMessage)

            //((MainWindow)Window.GetWindow(this)).UpdateUserTalkMessage(contact);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeContactImageGrid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
            Cursor = Cursors.Hand;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeContactImageGrid.Background =
                new SolidColorBrush(Colors.Transparent);
            Cursor = null;
        }

        private async void ChangeContactImageGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set change getting new image source
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Choose image or video",
                Filter = "Image and Video files|*.png;*.jpg;*.jpeg;*.mp4;*.mov;*.avi"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    //is image exist (add if not)
                    //Set it as image
                    if (FilesAction.IsFileIsImage(filePath))
                    {
                        Image img = new Image();
                        //Set file path

                        if(IsMaskExist())
                        {
                            return;
                        }

                        img.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                        img.Tag = filePath;

                        if (_contact.MaskImage is not null && 
                            _contact.UserImages.Count >= 1)
                        {
                            //Remove added mask
                            _contact.UserImages.RemoveAt(0);
                        }

                        _contact.MaskImage = 
                            new TelegramLib.MainClasses.UserParams.UserImage(filePath, DateTime.Now);
                        _contact.UserImages.Insert(0, _contact.MaskImage);

                        if(_system is not null)
                        {
                            User user = _system.GetUserById(_contact.ContactUserId);

                            TelegramLib.MainClasses.UserParams.UserImage mask = 
                                new TelegramLib.MainClasses.UserParams.UserImage(System.IO.Path.GetFileName(filePath), DateTime.Now);

                            user.ImageMask = mask;
                            user.UserImages.Insert(0, mask);
                        }

                        await ApiService.SetContactMask(_contact, _user.Id);

                        SetRemoveMaskLine();

                        //Set new contact image
                        UpdateVisAfterMasking();
                    }
                }
            }
        }

        public bool IsMaskExist()
        {
            if (_contact is null || _contact.MaskImage is null) return false;

            if (_system is not null)
            {
                User user = _system.GetUserById(_contact.ContactUserId);
                if (user.ImageMask is null) return false;
            }

            return true;
        }

        private const int _baseRemoveRowHeight = 50;
        public void SetRemoveMaskLine()
        {
            if (_contact.MaskImage is null)
            {
                SetContactMaskRow.Height = new GridLength(0);
                return;
            }
            SetContactMaskRow.Height = new GridLength(_baseRemoveRowHeight);
        }

        private void RemoveContactMaskGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            RemoveContactMaskGrid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
            Cursor = Cursors.Hand;
        }

        private void RemoveContactMaskGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            RemoveContactMaskGrid.Background =
                new SolidColorBrush(Colors.Transparent);
            Cursor = null;
        }

        private void RemoveContactMaskGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RemoveContactMask();
        }

        public async void RemoveContactMask()
        {
            //Remove from system

            //Remove mask from contact
            _contact.RemoveMask();

            //remove mask from user(if in chat)
            if (_system is not null)
            {
                User user = _system.GetUserById(_contact.ContactUserId);
                user.RemoveMask();
            }

            //Hide line
            SetRemoveMaskLine();
            
            //Remove from db
            await ApiService.SetContactMask(_contact, _user.Id);

            //Set in visual part
            UpdateVisAfterMasking();
        }

        public void UpdateVisAfterMasking()
        {
            ((MainWindow)Window.GetWindow(this)).SetContactMask(_contact.ContactUserId);

            TelegramLib.MainClasses.User contactUser = _system.GetUserById(_contact.ContactUserId);

            BgBrush.ImageSource = new BitmapImage(new Uri
                (FilesAction.GetUserImagePath(System.IO.Path.GetFileName(contactUser.GetFirstImageName().Name)), UriKind.Absolute));
        }
    }
}
