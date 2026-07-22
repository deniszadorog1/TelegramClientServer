using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramLib.UserSettings;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using Image = System.Windows.Controls.Image;
using User = TelegramLib.MainClasses.User;

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

        public async void AddNewUserBasicParams()
        {
            PageNameBlock.Text = "New Contact";
            UserLogin.Text = _user.Login;

            BgBrush.ImageSource = new BitmapImage(new Uri
                (await FilesAction.GetUserImagePath(System.IO.Path.GetFileName(_user.GetFirstImageName().Name)), UriKind.Absolute));

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

            UpdateImage();
        }


        private async void SetBasicParams()
        {
            const string resSeen = "recently";
            BgBrush.ImageSource = new BitmapImage(new Uri
                (await FilesAction.GetUserImagePath(_contact.GetFirstImageName().Name), UriKind.Absolute));

            UserLogin.Text = _contact.Login;

            FirstNameBox.Text = _contact.Name;
            LastNameBox.Text = _contact.Surname;

            PhoneNumberBox.Text = _contact.PhoneNumber;
            LastSeenBox.Text = _contact.LastSeen is null ? resSeen :
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
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
            string.IsNullOrWhiteSpace(LastNameBox.Text)) return;

            _contact.Name = string.Join(" ", FirstNameBox.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim();
            _contact.Surname = string.Join(" ", LastNameBox.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Trim();


            await ApiService.UpdateContact(_user.Id, _contact);

            //Update Contact name - surname
            ((MainWindow)Window.GetWindow(this)).UpdateContactParams(_contact);

            //Set in boss page(if chat on other window)          
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
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
        }

        public async Task AddContactIfContactOnline(User newContact)
        {
            UserContactcs contact = new UserContactcs(-1, FirstNameBox.Text, LastNameBox.Text, newContact.Login, newContact.BirthDay,
                newContact.BIO, newContact.PhoneNumber, newContact.LastSeenOnline, true, newContact.UserImages, null, false);

            contact.ContactUserId = newContact.Id;

            await ApiService.AddContact(_system.LoggedUser.Id, contact);

            _contact = await ApiService.GetLastUserContact(_system.LoggedUser.Id);

            if (!_system.IsContactExistByUserId(contact.ContactUserId)) _system.Contacts.Add(contact);

            await SignalRService.AddContact(newContact, _system.LoggedUser);
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

        private List<string> _imgsExt = new List<string>()
        {
            ".png",
            ".jpg",
            ".jpeg"
        };

        private async void ChangeContactImageGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            const string title = "Choose image";
            const string filter = "Image files|*.png;*.jpg;*.jpeg;";

            const int minImgsAmount = 1;
            Window window = Window.GetWindow(this);

            //Set change getting new image source
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = title,
                Filter = filter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                if (!_imgsExt.Contains(extension)) return;//  extension != ".png" && extension != ".jpg" && extension != ".jpeg") return;
                if (!FilesAction.IsFileIsImage(filePath) || IsMaskExist()) return;

                //is image exist (add if not)
                //Set it as image

                Image img = new Image();
                //Set file path

                if (window is MainWindow main) main.CloseAllMediaWindows();

                string newPath = await ApiService.UploadMediaAsync(filePath);

                img.Source = new BitmapImage(new Uri(await FilesAction.GetUserImagePath(newPath), UriKind.Absolute));
                //new BitmapImage(new Uri(filePath, UriKind.Absolute));
                img.Tag = newPath;

                if (_contact.MaskImage is not null &&
                    _contact.UserImages.Count >= minImgsAmount)
                {
                    //Remove added mask
                    _contact.UserImages.RemoveAt(0);
                }

                _contact.MaskImage =
                    new TelegramLib.MainClasses.UserParams.UserImage(newPath, DateTime.Now);
                _contact.UserImages.Insert(0, _contact.MaskImage);

                if (_system is not null)
                {
                    User user = _system.GetUserById(_contact.ContactUserId);

                    TelegramLib.MainClasses.UserParams.UserImage mask =
                        new TelegramLib.MainClasses.UserParams.UserImage(System.IO.Path.GetFileName(newPath), DateTime.Now);

                    user.ImageMask = mask;
                    user.UserImages.Insert(0, mask);
                }

                await ApiService.SetContactMask(_contact, _user.Id);

                SetRemoveMaskLine();

                //Set new contact image
                await UpdateVisAfterMasking();
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

        public async Task RemoveContactMask()
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
            await UpdateVisAfterMasking();
            await UpdateImage();
        }

        public async Task UpdateVisAfterMasking()
        {
            ((MainWindow)Window.GetWindow(this)).SetContactMask(_contact.ContactUserId);

            TelegramLib.MainClasses.User contactUser = _system.GetUserById(_contact.ContactUserId);

            BgBrush.ImageSource = new BitmapImage(new Uri
                (await FilesAction.GetUserImagePath(System.IO.Path.GetFileName(contactUser.GetFirstImageName().Name)), UriKind.Absolute));
        }

        public async Task UpdateImage(MainSettings settings = null)
        {
            if (_user is null || _contact is null) return;

            User user = _system.GetUserById(_contact.ContactUserId);
            if (user is null) return;

            UserChat chat = _system.GetChatByUserId(_contact.ContactUserId);
            if (chat is null) return;

            //Check for mask
            await SignalRHelperService.SetContactPhoto(user,
                chat, BgBrush, UserEllipseImage, settings: settings);
        }
    }
}
