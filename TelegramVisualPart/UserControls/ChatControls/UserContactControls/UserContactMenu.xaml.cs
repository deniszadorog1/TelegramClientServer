using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;
using TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.Pages.UserInfoContact.ActionsFolder;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls.ChatButsControls;
using TelegramVisualPart.UserControls.ChatsControls;

namespace TelegramVisualPart.UserControls.ChatControls.UserContactControls
{
    /// <summary>
    /// Логика взаимодействия для UserContactMenu.xaml
    /// </summary>
    public partial class UserContactMenu : UserControl
    {
        private TelSystem _system;
        private TelegramLib.MainClasses.UserChat _chat;

        public UserContactMenu()
        {
            InitializeComponent();
            SetBasicBlocks();

            AddClearSubMenuAction();
        }

        public void SetTelSystemParam(TelSystem system,
            TelegramLib.MainClasses.UserChat chat)
        {
            _system = system;
            _chat = chat;

            SetBlockButText();
        }

        public void SetBlockButText()
        {
            if (BlockUser.Visibility == Visibility.Hidden ||
                _chat.GetChatter() is null) return;

            if (_system.IsChatterBlocked(_chat.GetChatter()))
            {
                BlockUser.IconType.Kind = PackIconKind.BlockHelper;
                BlockUser.ButName.Text = "UnBlock user";
            }
            else
            {

                BlockUser.IconType.Kind = PackIconKind.Hand;
                BlockUser.ButName.Text = "Block user";
            }
        }

        public void UpdateParamsIsChatterIsNotContact()
        {
            if (_chat is TelegramLib.MainClasses.SavedMessagesChat) return;
            //Is chatter is contact  
            bool isContact = _system.IsChatterIdIsContact(_chat.Chatter.Id);

            //Set  vis for edit contact + folder + add contact
            EditContact.Visibility = isContact ? Visibility.Visible : Visibility.Hidden;
            ShareContact.Visibility = isContact ? Visibility.Visible : Visibility.Hidden;
            DeleteContact.Visibility = isContact ? Visibility.Visible : Visibility.Hidden;

            AddContact.Visibility = isContact ? Visibility.Hidden : Visibility.Visible;

            //Update Height
            UpdateMenuHeight();
        }

        public void RemoveParamsIfIsSavedMessagesChat()
        {
            AutoDelete.Visibility = Visibility.Hidden;
            AddContact.Visibility = Visibility.Hidden;
            EditContact.Visibility = Visibility.Hidden;
            BlockUser.Visibility = Visibility.Hidden;
            DeleteContact.Visibility = Visibility.Hidden;

            ButPanel.Children.Remove(LineDevider);

            UpdateMenuHeight();
        }

        public void UpdateMenuHeight()
        {
            Height = 0;
            for (int i = 0; i < ButPanel.Children.Count; i++)
            {
                if (ButPanel.Children[i] is UserChatMenuButton but)
                {
                    if (but.Visibility == Visibility.Visible)
                    {
                        but.Height = ExportHistory.Height;
                        Height += but.Height;
                    }
                    else but.Height = 0;
                }
            }
            Height += ButPanel.Margin.Top + ButPanel.Margin.Bottom + LineDevider.Margin.Top;
        }

        public void SetBasicBlocks()
        {
            AutoDelete.IconType.Kind = PackIconKind.AvTimer;
            //AutoDelete.ButName.Text = "Auto-Delete";

            AddContact.IconType.Kind = PackIconKind.PersonAddOutline;
            AddContact.ButName.Text = "Add Contact";

            ShareContact.IconType.Kind = PackIconKind.ShareOutline;
            //ShareContact.ButName.Text = "Share this contact";

            EditContact.IconType.Kind = PackIconKind.PencilOutline;
            //EditContact.ButName.Text = "Edit contact";

            ExportHistory.IconType.Kind = PackIconKind.Export;
            //ExportHistory.ButName.Text = "Export chat history";

            AddToFolder.IconType.Kind = PackIconKind.FolderOutline;
            //AddToFolder.ButName.Text = "Add to folder";
            AddToFolder.MemeIcon.Visibility = Visibility.Visible;

            BlockUser.IconType.Kind = PackIconKind.Hand;
            //BlockUser.ButName.Text = "Block user";

            DeleteContact.IconType.Kind = PackIconKind.TrashCan;
            //DeleteContact.ButName.Text = "Delete contact";
            DeleteContact.IconType.Foreground =
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
            DeleteContact.ButName.Foreground =
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
        }

        public event Action UnblockUser;
        private void But_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UserChatMenuButton but)
            {
                if (but.Name == BlockUser.Name.ToString() &&
                   _system.IsChatterBlocked(_chat.GetChatter()))
                {
                    UnblockUser?.Invoke();
                    this.Visibility = Visibility.Hidden;
                    return;
                }

                Page? page = GetPageToOpen(but.Name);
                if (page is null) return;
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);

                this.Visibility = Visibility.Hidden;
            }
        }

        private Page? GetPageToOpen(string name)
        {
            if (_chat is null ||
                (_chat.Chatter is null && name != ShareContact.Name.ToString()))
            {
                return null;
            }

            UserContactcs contact = _chat.Chatter is null ? null : _system.GetContactByUserId(_chat.Chatter.Id);
            if (contact is null &&
                name != AddContact.Name.ToString() &&
                name != AutoDelete.Name.ToString() &&
                name != ShareContact.Name.ToString()) return null;

            return name == AutoDelete.Name.ToString() ? new NewMessagesDeletion(/*_system.GetChosenChat()*/ _chat, _system) :
                   name == DeleteContact.Name.ToString() ? new DeleteContact(/*_system.ChosenChatContact */contact, _system) :

                   name == BlockUser.Name.ToString() ? new BlockContact(_system, /*_system.ChosenChatContact*/ _chat.Chatter) :

                   name == EditContact.Name.ToString() ? new EditUserContact(_system.LoggedUser, /*_system.ChosenChatContact*/contact, _system) :
                   name == AddToFolder.Name.ToString() ? new FoldersPage(_system, false) :
                   name == ShareContact.Name.ToString() ? new ShareContact(_system, /*_system.ChosenChatContact*/ contact) :
                   name == AddContact.Name.ToString() ? new EditUserContact(_chat.Chatter, _system) : null;
        }

        public void SetFoldersParams()
        {
            Size size = new Size(250, 15);

            //Set panel children
            //Set height
            //hide upper panel
            if (_chat is null) return;
            ButPanel.Children.Clear();
            Width = size.Width;
            Height = size.Height;

            int chatterId = _chat is TelegramLib.MainClasses.SavedMessagesChat ?
                _system.LoggedUser.Id : _chat.Chatter.Id;

            //Folders elements
            for (int i = 0; i < _system.Folders.Count; i++)
            {
                User? isIncluded =
                    _system.Folders[i].Contacts.FirstOrDefault(x => x.Id == chatterId);

                AddFolderElement(_system.Folders[i].Name, isIncluded is not null);
            }

            //To Add Folder element
            AddFolderElement();
        }

        public void AddFolderElement()
        {
            UserChatsMenuElement toAdd = new UserChatsMenuElement();

            //Is included
            toAdd.IconElement.Kind = PackIconKind.Folder;
            toAdd.IconElement.Foreground =
                (SolidColorBrush)Application.Current.Resources["UsualTextColor"];

            //folder name
            toAdd.TextElement.Text = VisConstParamsJsonService.GetStringByName("CreateNewFolder");

            //Set folder icon
            toAdd.ArrowRightIcon.Visibility = Visibility.Hidden;

            //Set events
            toAdd.PreviewMouseDown += AddNewFolder_PreviewMouseDown;

            Height += toAdd.Height;

            FoldersMenu.Children.Add(toAdd);
        }

        public void AddNewFolder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new FolderAction(_system));
        }

        public void AddFolderElement(string folderName, bool isIncluded)
        {
            UserChatsMenuElement toAdd = new UserChatsMenuElement();

            //Is included
            toAdd.IconElement.Kind = isIncluded ? PackIconKind.Tick : PackIconKind.None;
            toAdd.IconElement.Foreground =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            //folder name
            toAdd.TextElement.Text = folderName;

            //Set folder icon
            toAdd.ArrowRightIcon.Kind = PackIconKind.FolderOutline;
            toAdd.ArrowRightIcon.Foreground =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            toAdd.ArrowColumn.Width = new GridLength(60);
            //Set events
            toAdd.PreviewMouseDown += FolderElement_PreviewMouseDown;

            Height += toAdd.Height;

            FoldersMenu.Children.Add(toAdd);
        }

        public event Action ClearThis;
        public void FolderElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UserChatsMenuElement el) return;
            TelegramLib.MainClasses.User user = _chat is TelegramLib.MainClasses.SavedMessagesChat ?
                _system.LoggedUser : _chat.GetChatter();
            //Set add in folder
            if (el.IconElement.Kind == PackIconKind.None)
            {
                _system.AddContactToFolder(el.TextElement.Text, user);
            }
            else _system.RemoveContactFromFolder(el.TextElement.Text, user); //Set remove from folder


            //Remove from db

            //Clear from Vis
            _wind.UpdateFolder();

            //clear element (sub thing)
            ClearThis?.Invoke();
        }

        public double GetAddFolderButPos()
        {
            const int minMult = 4;
            const int maxMult = 6;
            const int addButPos = 15;
            const int addMult = 1;

            int multiplier =
                _chat is TelegramLib.MainClasses.SavedMessagesChat ?
                minMult : maxMult;

            //Get Amount of VisParapms
            bool isContact = _chat is TelegramLib.MainClasses.SavedMessagesChat ? false : _system.IsChatterIdIsContact(_chat.Chatter.Id);
            return AddToFolder.Height * (isContact ? multiplier : multiplier - addMult) + addButPos;
        }

        MainWindow _wind;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _wind = Window.GetWindow(this) as MainWindow;
        }

        public void AddClearSubMenuAction()
        {
            AutoDelete.MouseEnter += ClearSubMenu_MouseEnter;
            AddContact.MouseEnter += ClearSubMenu_MouseEnter;
            ShareContact.MouseEnter += ClearSubMenu_MouseEnter;
            EditContact.MouseEnter += ClearSubMenu_MouseEnter;
            ExportHistory.MouseEnter += ClearSubMenu_MouseEnter;
            BlockUser.MouseEnter += ClearSubMenu_MouseEnter;
            DeleteContact.MouseEnter += ClearSubMenu_MouseEnter;
        }

        public void ClearSubMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            //clear element (sub thing)
            ClearThis?.Invoke();
        }
    }
}
