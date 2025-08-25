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
using TelegramVisualPart.UserControls.DifferButs;

namespace TelegramVisualPart.UserControls.ChatControls.UserContactControls
{
    /// <summary>
    /// Логика взаимодействия для UserContactMenu.xaml
    /// </summary>
    public partial class UserContactMenu : UserControl
    {
        private TelSystem _system;
        public UserContactMenu()
        {
            InitializeComponent();
            SetBasicBlocks();
        }

        public void SetTelSystemParam(TelSystem system)
        {
            _system = system;
        }

        public void SetBasicBlocks()
        {
            AutoDelete.IconType.Kind = PackIconKind.AvTimer;
            AutoDelete.ButName.Text = "Auto-Delete";

            ShareContact.IconType.Kind = PackIconKind.ShareOutline;
            ShareContact.ButName.Text = "Share this contact";

            EditContact.IconType.Kind = PackIconKind.PencilOutline;
            EditContact.ButName.Text = "Edit contact";

            ExportHistory.IconType.Kind = PackIconKind.Export;
            ExportHistory.ButName.Text = "Export chat history";

            AddToFolder.IconType.Kind = PackIconKind.FolderOutline;
            AddToFolder.ButName.Text = "Add to folder";

            BlockUser.IconType.Kind = PackIconKind.Hand;
            BlockUser.ButName.Text = "Block user";

            DeleteContact.IconType.Kind = PackIconKind.TrashCan;
            DeleteContact.ButName.Text = "Delete contact";
            DeleteContact.IconType.Foreground = 
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
            DeleteContact.ButName.Foreground =
                (SolidColorBrush)Application.Current.Resources["CloseWindowColor"];
        }

        private void But_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is MenuIconTextBut but)
            {
                Page page = GetPageToOpen(but.Name);
                if (page is null) return;
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);
            }
        }

        private Page GetPageToOpen(string name)
        {
            return name == AutoDelete.Name.ToString() ? new NewMessagesDeletion(_system.GetChosenChat(), _system) :
                   name == DeleteContact.Name.ToString() ? new DeleteContact(_system.ChosenChatContact, _system) :
                   name == BlockUser.Name.ToString() ? new BlockContact(_system, _system.ChosenChatContact) :
                   name == EditContact.Name.ToString() ? new EditUserContact(_system.LoggedUser, _system.ChosenChatContact) :
                   name == AddToFolder.Name.ToString() ? new FoldersPage(_system) : 
                   name == ShareContact.Name.ToString() ? new ShareContact(_system, _system.ChosenChatContact) : null;
        }
    }
}
