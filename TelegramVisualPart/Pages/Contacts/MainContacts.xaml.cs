using MaterialDesignThemes.Wpf;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TelegramLib.MainClasses;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ContactsControls;

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

        public event Action ToCheckEnd;

        public MainContacts(ContactsPageAction type, TelSystem system,
            bool isBlock)
        {
            _type = type;
            _system = system;
            _isBlock = isBlock;

            InitializeComponent();
            SetBasicParams();

            SetParams();

            SetLanguageText.SetUserContacts(this);
        }

        public async void SetParams()
        {
            _isLoading = true;
            await SetContactsParams(0);
            _isLoading = false;

        }

        private List<int> _addedIds = new List<int>();
        private const int _stepAmount = 9;

        public async Task SetContactsParams(int id)
        {
            if (_isBlock)
            {
                await SetUsersToBlock();
                ToCheckEnd?.Invoke();
                return;
            }

            List<UserContactcs> toAdd = !_isBlock ? _system.Contacts :
                _system.Contacts.Where(x => !_system.LoggedUser.BlockedUsers.Select(y => y.Name).Contains(x.Name)).ToList();

            toAdd = toAdd.Where(x => !_addedIds.Contains(x.Id)).Take(_stepAmount).ToList();

            _addedIds.AddRange(toAdd.Select(x => x.Id));

            foreach (var val in toAdd)
            {
                TelegramLib.MainClasses.User user = await ApiService.GetUserById(val.ContactUserId);

                string imgName = user.GetFirstImageNameInString();
                string imgPath = await FilesAction.GetUserImagePath(imgName);
                if (ApiService.GetCachedBitmap(imgPath) is null)
                    await SignalRHelperService.LoadBitmap(imgPath);

                UserContact contact = new UserContact(user);
                await contact.SetBasicParams();

                ListBoxItem item = new ListBoxItem
                {
                    Content = contact,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Tag = val.Login
                };

                item.PreviewMouseDown += Contact_PreviewMouseDown;
                
                ContactsListBox.Items.Add(item);
            }
            Visibility = Visibility.Visible;

            //if (toAdd.Count == 0) Visibility = Visibility.Visible;
            ToCheckEnd?.Invoke();
        }


        private async Task SetUsersToBlock()
        {
            List<TelegramLib.MainClasses.User> toAdd =
                _system.Chats
                    .Where(x => !_system.LoggedUser.BlockedUsers.Select(y => y.Id)
                        .Contains(x.Chatter.Id))
                    .Select(x => x.Chatter)
                        .Where(x => !_addedIds.Contains(x.Id))
                        .Take(_stepAmount)
                    .ToList();

            _addedIds.AddRange(toAdd.Select(x => x.Id));

            foreach (var val in toAdd)
            {
                string imgName = val.GetFirstImageNameInString();
                string imgPath = await FilesAction.GetUserImagePath(imgName);
                if (ApiService.GetCachedBitmap(imgPath) is null)
                    await SignalRHelperService.LoadBitmap(imgPath);

                UserContact contact = new UserContact(val);
                await contact.SetBasicParams();
                
                contact.ImgSet += () =>
                {
                    Visibility = Visibility.Visible;
                };

                ListBoxItem item = new ListBoxItem
                {
                    Content = contact,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    Tag = val.Id
                };

                item.PreviewMouseDown += Contact_PreviewMouseDown;
                ContactsListBox.Items.Add(item);
            }
        }

        private async void Contact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                item.Content is not UserContact contact) return;

            if (_isBlock) //block in logic
            {
                //Tag is userId
                TelegramLib.MainClasses.UserChat? chat =
                    _system.Chats
                    .FirstOrDefault(x => x.Chatter.Id.ToString() == item.Tag.ToString());
                if (chat is null) return;

                _system.LoggedUser.BlockedUsers.Add(chat.Chatter);
                ContactsListBox.Items.Remove(item);

                ContactClicked?.Invoke(sender, EventArgs.Empty);
                ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);

                //Update if its temp chat 
                ((MainWindow)Window.GetWindow(this)).UpdateChatParamsVis();

                await ((MainWindow)Window.GetWindow(this)).SetBlockedUserVisParams(true, chat.GetChatter());
                return;
            }

            //is chat exist 
            if (_system.GetChatByChatterId(contact._user.Id) is null)
            {
                UserContactcs toAdd = _system.GetContactByUserId(contact._user.Id);
                await ((MainWindow)Window.GetWindow(this)).AddChatInMainPage(toAdd);
            }

            ContactClicked?.Invoke(sender, EventArgs.Empty);

            Window window = Window.GetWindow(this);
            if (window is not null && window is MainWindow main) main.ClearSecFrame();
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

        private void SortGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            SortBut.Foreground =
                new SolidColorBrush(Colors.White);
        }

        private void SortGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            SortBut.Foreground =
                new SolidColorBrush(Colors.Gray);
        }

        private PackIconKind _aKind = PackIconKind.HamburgerPlus;
        private PackIconKind _bKind = PackIconKind.HamburgerCheck;

        private void SortGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SortBut.IconType.Kind == PackIconKind.HamburgerMenu)
            {
                SortBut.IconType.Kind = _aKind;
                //Sort by name
                SortByContactByName();
                return;
            }

            SortBut.IconType.Kind = SortBut.IconType.Kind ==
                _aKind ? _bKind : _aKind;

            //Sort by Name
            if (SortBut.IconType.Kind == _aKind)
            {
                SortByContactByName();
                return;
            }

            //Sort by last seen time
            SortByLastSeenTime();
        }

        public void SortByLastSeenTime()
        {
            var sortedItems = ContactsListBox.Items
                .Cast<ListBoxItem>()
                .OrderBy(i =>
                    (i.Content as UserContact)?
                        .LastSennOnline?
                        .Text ?? string.Empty,
                    StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            ContactsListBox.Items.Clear();

            foreach (var item in sortedItems)
            {
                ContactsListBox.Items.Add(item);
            }
        }

        public void SortByContactByName()
        {
            var sortedItems = ContactsListBox.Items
                .Cast<ListBoxItem>()
                .OrderBy(i => i.Tag?.ToString(), StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            ContactsListBox.Items.Clear();

            foreach (var item in sortedItems)
            {
                ContactsListBox.Items.Add(item);
            }
        }

        private bool _isLoading = false;

        private const int _extHeightToLoad = 5;
        private async void ContactsListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_isLoading) return;
            try
            {
                _isLoading = true;
                if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - _extHeightToLoad)
                {
                    ListBoxItem? item =
                         ContactsListBox.Items.OfType<ListBoxItem>().LastOrDefault();
                    if (item is null || item.Tag is null) return;

                    int.TryParse(item.Tag.ToString(), out int id);

                    await SetContactsParams(id);
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

    }
}
