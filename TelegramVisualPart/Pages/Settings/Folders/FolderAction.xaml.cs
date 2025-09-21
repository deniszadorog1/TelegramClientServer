using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls.FoldersPrivacy;

namespace TelegramVisualPart.Pages.Settings.Folders
{
    /// <summary>
    /// Логика взаимодействия для FolderAction.xaml
    /// </summary>
    public partial class FolderAction : Page
    {
        private bool _isSaveAction = false;
        public event EventHandler FolderCreated;

        private TelSystem _system;
        private Folder _folder;

        private List<User> _toExcludeContacts = new List<User>();
        private List<User> _toAddContacts = new List<User>();

        public FolderAction(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetBlocks();

            FolderIcon.NewIconChosenEvent += NewIcon_Event;

            SetLanguageText.SetFolderAction(this);
        }

        public FolderAction(TelSystem system, Folder folder)
        {
            _isSaveAction = true;
            _system = system;
            _folder = folder;

            InitializeComponent();

            SetBlocks();
            FolderIcon.NewIconChosenEvent += NewIcon_Event;
            SetChosenFolderParams();

            SetUpdateBlocks();
            SetLanguageText.SetFolderAction(this);
        }

        public void SetUpdateBlocks()
        {
            if (_folder is null) return;

            FolderNameBox.Text = _folder.Name;
            ChosenFolderIcon.Kind = Enum.Parse<PackIconKind>(_folder.IconName);
        }

        public void SetChosenFolderParams()
        {
            //Set folders params here
            _toAddContacts = _folder.Contacts;
            SetContactsInListBox(_toAddContacts, ToMakeNewFolderListBoxItem);

            _toExcludeContacts = _folder.ExcludedContacts;
            SetContactsInListBox(_toExcludeContacts, ToExcludeListBox);

            //CreateBut.Content = "Save";
        }

        public void NewIcon_Event(object sender, EventArgs e)
        {
            if (sender is not FolderIcons icons) return;

            PackIconKind iconKind = icons.GetChosenIconName();
            ChosenFolderIcon.Kind = iconKind;
            FolderIcon.Visibility = Visibility.Hidden;
        }

        private void SetBlocks()
        {
            //CreateNewFolderBut.NewFolderText.Text = "Add chat";
            //ChatToExcludeBut.NewFolderText.Text = "Add Chats to Exclude";

            //CreateInviteLinkBut.NewFolderText.Text = "Create an Invite Link";
            //CreateInviteLinkBut.IconType.Kind = PackIconKind.LinkVariant;

            ChatToExcludeBut.IconType.Kind = PackIconKind.Minus;
        }

        private void ToChooseFolderIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            FolderIcon.Visibility = Visibility.Visible;
        }

        private void ToChooseFolderIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            // FolderIcon.Visibility = Visibility.Hidden;
        }

        private async void CreateBut_Click(object sender, RoutedEventArgs e)
        {
            //CHECK FOLDER SETTINGS (IS name exist etc...)

            //Apply folder settings
            if (_isSaveAction)
            {
                //Update folder
                _folder.SetContacts(_toAddContacts);
                _folder.SetExcludeContacts(_toExcludeContacts);
                _folder.SetIconName(ChosenFolderIcon.Kind.ToString());
                _folder.SetName(FolderNameBox.Text);


                await ApiService.UpdateFolder(_folder, _system.LoggedUser.Id);

                ((MainWindow)Window.GetWindow(this)).UpdateFolders();
                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new FoldersPage(_system, true));
                return;
            }

            //To add new Folder
            if (!await CreateNewFolder()) return;

            FolderCreated?.Invoke(this, EventArgs.Empty);

            ((MainWindow)Window.GetWindow(this)).UpdateFolders();
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new FoldersPage(_system, true));
        }

        public async Task<bool> CreateNewFolder()
        {
            if (string.IsNullOrWhiteSpace(FolderNameBox.Text) ||
                _system.IsFolderNameExists(FolderNameBox.Text)) return false;

            _system.AddFolder(FolderNameBox.Text, ChosenFolderIcon.Kind.ToString(),
                _toAddContacts, _toExcludeContacts);

            await ApiService.AddFolder(_system.GetLastFolder(), _system.LoggedUser.Id);

            return true;
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new FoldersPage(_system, true));
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

        private void FolderIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            FolderIcon.Visibility = Visibility.Hidden;
        }

        private void CreateNewFolderBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FoldersChatAction setContacts = new FoldersChatAction(
                Enums.FolderChatActionType.AddChatInFolder, _system, _toAddContacts);

            setContacts.ToSetContacts += SetAddContacts_Chosen;

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(setContacts);
        }

        public void SetAddContacts_Chosen(object sender, EventArgs e)
        {
            if (sender is not FoldersChatAction action) return;

            _toAddContacts = action.GetChosenContacts();
            ClearAlreadyChosenItems(_toAddContacts);
            ClearAlreadyChosenContacts(_toAddContacts, _toExcludeContacts);

            SetContactsInListBox(_toAddContacts, ToMakeNewFolderListBoxItem);
        }

        private void ChatToExcludeBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FoldersChatAction excludePage = new FoldersChatAction(
                Enums.FolderChatActionType.ExcludeChat, _system, _toExcludeContacts);

            excludePage.ToSetContacts += SetExcludeContacts_Chosen;

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(excludePage);
        }

        public void SetExcludeContacts_Chosen(object sender, EventArgs e)
        {
            if (sender is not FoldersChatAction action) return;

            _toExcludeContacts = action.GetChosenContacts();

            ClearAlreadyChosenItems(_toExcludeContacts);
            ClearAlreadyChosenContacts(_toExcludeContacts, _toAddContacts);

            SetContactsInListBox(_toExcludeContacts, ToExcludeListBox);
        }

        public void ClearAlreadyChosenItems(List<User> chosenContacts)
        {
            List<ListBoxItem> itemsToRemove = MainListBox.Items
                .OfType<ListBoxItem>()
                .Where(x => x.Content is FoldersChat folder &&
                            chosenContacts.Any(c => c.Name == folder.GetFolderChatName()))
                .ToList();

            foreach (ListBoxItem item in itemsToRemove)
            {
                MainListBox.Items.Remove(item);
            }
        }

        public void ClearAlreadyChosenContacts(List<User> chosen, List<User> toClear)
        {
            toClear.RemoveAll(contact => chosen.Any(x => x.Name == contact.Name));


            /*            foreach (UserContactcs contact in toClear)
                        {
                            UserContactcs toRemove = chosen.Where(x => x.Name == contact.Name).FirstOrDefault();
                            if (toRemove is not null)
                            {
                                toClear.Remove(toRemove);
                            }
                        }*/
        }

        public void SetContactsInListBox(List<User> contacts, ListBoxItem addFolder)
        {
            int butIndex = MainListBox.Items.IndexOf(addFolder);
            UpdateFolderChats(butIndex);

            for (int i = 0; i < contacts.Count; i++)
            {
                FoldersChat folderChat = new FoldersChat();
                folderChat.Width = this.Width - 10;
                folderChat.NewFoldersChatText.Text = contacts[i].Name;

                folderChat.RemoveControl += RemoveFolderChat_PreviewMouseDown;

                folderChat.ChatEllipse.Fill = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(
                        FilesAction.GetUserImagePath(contacts[i].GetFirstImageName().Name), UriKind.Absolute)),

                };

                ListBoxItem item = new ListBoxItem()
                {
                    Content = folderChat,
                    Padding = new Thickness(0, 5, 5, 5)
                };


                MainListBox.Items.Insert(butIndex + 1, item);
            }
        }

        private void RemoveFolderChat_PreviewMouseDown(object sender, EventArgs e)
        {
            if (sender is not FoldersChat userInfo) return;

            RemoveUserContact(_toAddContacts, userInfo.NewFoldersChatText.Text);
            RemoveUserContact(_toExcludeContacts, userInfo.NewFoldersChatText.Text);

            //clear from view
            ListBoxItem item = ItemsControl.ContainerFromElement(MainListBox, userInfo) as ListBoxItem;
            MainListBox.Items.Remove(item);
        }

        public void RemoveUserContact(List<User> contacts, string contactName)
        {
            contacts.Remove(contacts.Where(x => x.Name == contactName).FirstOrDefault());
        }


        public void UpdateFolderChats(int boxIndex)
        {
            List<ListBoxItem> chatElems = new List<ListBoxItem>();
            for (int i = boxIndex + 1; i < MainListBox.Items.Count; i++)
            {
                if (MainListBox.Items[i] is ListBoxItem item &&
                    item.Content is FoldersChat chat)
                {
                    chatElems.Add(item);
                }
                else break;
            }

            foreach (ListBoxItem chat in chatElems)
            {
                MainListBox.Items.Remove(chat);
            }
        }

        public string GetFolderName()
        {
            return FolderNameBox.Text;
        }


    }
}
