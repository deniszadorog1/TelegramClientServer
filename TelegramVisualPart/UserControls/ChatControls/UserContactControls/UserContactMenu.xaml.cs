using MaterialDesignThemes.Wpf;
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
using TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.Pages.UserInfoContact.ActionsFolder;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls.ChatButsControls;
using TelegramVisualPart.UserControls.ChatsControls;
using TelegramVisualPart.UserControls.DifferButs;

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
        }

        public void SetTelSystemParam(TelSystem system, TelegramLib.MainClasses.UserChat chat)
        {
            _system = system;
            _chat = chat;
        }

        public void SetBasicBlocks()
        {
            AutoDelete.IconType.Kind = PackIconKind.AvTimer;
            //AutoDelete.ButName.Text = "Auto-Delete";

            ShareContact.IconType.Kind = PackIconKind.ShareOutline;
            //ShareContact.ButName.Text = "Share this contact";

            EditContact.IconType.Kind = PackIconKind.PencilOutline;
            //EditContact.ButName.Text = "Edit contact";

            ExportHistory.IconType.Kind = PackIconKind.Export;
            //ExportHistory.ButName.Text = "Export chat history";

            AddToFolder.IconType.Kind = PackIconKind.FolderOutline;
            //AddToFolder.ButName.Text = "Add to folder";

            BlockUser.IconType.Kind = PackIconKind.Hand;
            //BlockUser.ButName.Text = "Block user";

            DeleteContact.IconType.Kind = PackIconKind.TrashCan;
            //DeleteContact.ButName.Text = "Delete contact";
            DeleteContact.IconType.Foreground =
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
            DeleteContact.ButName.Foreground =
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
        }

        private void But_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UserChatMenuButton but)
            {
                Page? page = GetPageToOpen(but.Name);
                if (page is null) return;
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);
            }
        }

        private Page? GetPageToOpen(string name)
        {
            UserContactcs contact = _system.GetContactByUserId(_chat.Chatter.Id);

            return name == AutoDelete.Name.ToString() ? new NewMessagesDeletion(/*_system.GetChosenChat()*/ _chat, _system) :
                   name == DeleteContact.Name.ToString() ? new DeleteContact(/*_system.ChosenChatContact */contact, _system) :
                   name == BlockUser.Name.ToString() ? new BlockContact(_system, /*_system.ChosenChatContact*/ _chat.Chatter) :
                   name == EditContact.Name.ToString() ? new EditUserContact(_system.LoggedUser, /*_system.ChosenChatContact*/contact) :
                   name == AddToFolder.Name.ToString() ? new FoldersPage(_system, false) :
                   name == ShareContact.Name.ToString() ? new ShareContact(_system, /*_system.ChosenChatContact*/ contact) : null;
        }

        public void SetFoldersParams()
        {
            //Set panel children
            //Set height
            //hide upper panel
            if (_chat is null) return;
            ButPanel.Children.Clear();
            Height = 15;
            Width = 250;

            //Folders elements
            for (int i = 0; i < _system.Folders.Count; i++)
            {
                User? isIncluded =
                    _system.Folders[i].Contacts.FirstOrDefault(x => x.Id == _chat.Chatter.Id);

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

            //Set add in folder
            if (el.IconElement.Kind == PackIconKind.None)
            {
                _system.AddContactToFolder(el.TextElement.Text, _chat.Chatter);
            }
           else _system.RemoveContactFromFolder(el.TextElement.Text, _chat.Chatter); //Set remove from folder


            //Remove from db

            //Clear from Vis
            _wind.UpdateFolder();

            //clear element (sub thing)
            ClearThis?.Invoke();
        }

        public double GetAddFolderButPos()
        {
           return AddToFolder.Height * 6 + 15;
        }

        MainWindow _wind;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _wind = Window.GetWindow(this) as MainWindow;
        }
    }
}
