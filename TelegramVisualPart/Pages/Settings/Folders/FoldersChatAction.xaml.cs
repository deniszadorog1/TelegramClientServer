using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
using TelegramLib.MainClasses.FolderObjs;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.UserControls.SettingsControls.FoldersPrivacy;

namespace TelegramVisualPart.Pages.Settings.Folders
{
    /// <summary>
    /// Логика взаимодействия для FoldersChatAction.xaml
    /// </summary>
    public partial class FoldersChatAction : Page
    {
        public event EventHandler ToSetContacts;

        public List<UserContactcs> _chosenContacts = new List<UserContactcs>();

        private FolderChatActionType _type;
        private TelSystem _system;

        public FoldersChatAction(FolderChatActionType type, TelSystem system, 
            List<UserContactcs> chosenContacts)
        {
            _system = system;
            _type = type;
            _chosenContacts = chosenContacts;

            InitializeComponent();

            SetBasicBlocks();

            SetContacts();
            SetChosenContacts();
        }

        public void SetChosenContacts()
        {
            foreach(UserContactcs contact in _chosenContacts)
            {
                FolderChatType control = GetFolderChatTypeByContactName(contact.Name);
                if (control is null) continue;

                control.ChangeActivenessState();
            }
        } 

        public FolderChatType GetFolderChatTypeByContactName(string name)
        {
            ListBoxItem item = ListContacts.Items.OfType<ListBoxItem>()
                .Where(x => x.Content is FolderChatType folder && folder.TypeName.Text == name)
                .FirstOrDefault();

            return item is null ? null : (FolderChatType)item.Content;
        }

        public void SetContacts()
        {
            for (int i = 0; i < _system.Contacts.Count; i++)
            {
                string contactPath = _system.Contacts[i].GetLastImageName();

                FolderChatType control = new FolderChatType();

                control.TypeName.Text = _system.Contacts[i].Name;
                control.HideIcon();
                control.ChatEllipse.Fill = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(FilesAction.GetUserImagePath(contactPath), UriKind.Absolute)),
                    Stretch = Stretch.Fill
                };

                ListBoxItem item = new ListBoxItem()
                {
                    Content = control,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = new Thickness(0)
                };
                item.PreviewMouseDown += ChatTypes_PreviewMouseDown;

                ListContacts.Items.Add(item);
            }
        }

        public void SetBasicBlocks()
        {
            ContactsChats.IconType.Kind = PackIconKind.Account;
            ContactsChats.TypeName.Text = "Contacts";
            ContactsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderContactColor"];

            NoneContactsChats.IconType.Kind = PackIconKind.QuestionMarkCircle;
            NoneContactsChats.TypeName.Text = "Non-Contacts";
            NoneContactsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderNonContactColor"];

            GroupsChats.IconType.Kind = PackIconKind.UserGroup;
            GroupsChats.TypeName.Text = "Groups";
            GroupsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderGroupColor"];

            ChannelsChats.IconType.Kind = PackIconKind.AirHorn;
            ChannelsChats.TypeName.Text = "Channels";
            ChannelsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderChannelsColor"];

            BotsChats.IconType.Kind = PackIconKind.Android;
            BotsChats.TypeName.Text = "Bots";
            BotsChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderBotsColor"];

            /*
                        MutedChats.IconType.Kind = PackIconKind.VolumeMute;
                        MutedChats.TypeName.Text = "Muted";
                        MutedChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderBotsColor"];

                        ReadChats.IconType.Kind = PackIconKind.MessageText;
                        ReadChats.TypeName.Text = "Read";
                        ReadChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderNonContactColor"];

                        ArchivedChats.IconType.Kind = PackIconKind.Archive;
                        ArchivedChats.TypeName.Text = "Archived";
                        ArchivedChats.ChatEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FolderContactColor"];
                   */
        }

        private void ClearSearchBoxGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ClearSearchBoxBut.Foreground = Brushes.White;
        }

        private void ClearSearchBoxGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearSearchBoxBut.Foreground = Brushes.Gray;
        }

        private void ClearSearchBoxGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ChatSearchBox.Text = string.Empty;
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

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            ToSetContacts?.Invoke(this, EventArgs.Empty);

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            _chosenContacts.Clear();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void ChatTypes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBoxItem item ||
                item.Content is not FolderChatType control) return;

            UserContactcs contact =  _system.GetContactByName(control.TypeName.Text);
            if (contact is null) return;

            if (_chosenContacts.Contains(contact))
                _chosenContacts.Remove(contact);
            else
                _chosenContacts.Add(contact);
        }

        public List<UserContactcs> GetChosenContacts()
        {
            return _chosenContacts;
        }
    }
}
