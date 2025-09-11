using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using TelegramVisualPart.Enums;
using TelegramVisualPart.Enums.Menus;

namespace TelegramVisualPart.UserControls.ChatsControls
{
    /// <summary>
    /// Логика взаимодействия для UserChatMenu.xaml
    /// </summary>
    public partial class UserChatMenu : UserControl
    {
        TelegramLib.MainClasses.UserChat _chat = null;

        public UserChatMenu()
        {
            InitializeComponent();

            SetBasicParams();
        }

        public UserChatMenu(TelegramLib.MainClasses.UserChat chat)
        {
            _chat = chat;
            InitializeComponent();

            SetBasicParams();
        }

        private MainWindow _window;
        public void SetWindow(MainWindow window)
        {
            _window = window;
        }

        public void SetBasicParams()
        {
            SetBasicIcons();
            HideRightArrowIcons();
            SetBasicColors();
            SetBasicText();
            SetAddSubMenuEventsToElems();

            SetChangeableIcons();
        }

        public void SetChangeableIcons()
        {
            if (_chat is null) return;

            Unpin.IconElement.Kind = _chat.IsPinned ? 
                PackIconKind.PinOffOutline : PackIconKind.PinOutline;
            Unpin.TextElement.Text = _chat.IsPinned ? "Unpin" : "Pin";

            MarkRead.IconElement.Kind = _chat.IsMarked ?
                PackIconKind.ChatOutline : PackIconKind.ChatAlertOutline;
            MarkRead.TextElement.Text = _chat.IsMarked ? "Mark as read" :
                "Mark as unread";
        } 

        public void SetAddSubMenuEventsToElems()
        {

            MuteNotifs.MouseEnter += ToAddSubMenu_MouseEnter;
            MuteNotifs.PreviewMouseDown += UserChatsMenuElement_PreviewMouseDown;

            AddToFolder.MouseEnter += ToAddSubMenu_MouseEnter;
            AddToFolder.PreviewMouseDown += UserChatsMenuElement_PreviewMouseDown;
        }

        public void ToAddSubMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            UserChatsMenuElement_MouseEnter(sender, e);
            AddSubMenu_MouseEnter(sender, e);
        }

        public void SetBasicText()
        {
            OpenNewWindow.TextElement.Text = "Open in Window";
            Archive.TextElement.Text = "Archive";
            Unpin.TextElement.Text = "Unpin";
            MuteNotifs.TextElement.Text = "Mute notifications";
            MarkRead.TextElement.Text = "Mark as unread";
            AddToFolder.TextElement.Text = "Add to folder";
            ClearChat.TextElement.Text = "Clear Chat";
            DeleteChat.TextElement.Text = "Delete Chat";
        }

        public void SetBasicColors()
        {
            DeleteChat.IconElement.Foreground = Brushes.Red;
            DeleteChat.TextElement.Foreground = Brushes.Red;
        }

        public void HideRightArrowIcons()
        {
            OpenNewWindow.ArrowRightIcon.Visibility = Visibility.Hidden;
            Archive.ArrowRightIcon.Visibility = Visibility.Hidden;
            Unpin.ArrowRightIcon.Visibility = Visibility.Hidden;
            MarkRead.ArrowRightIcon.Visibility = Visibility.Hidden;
            ClearChat.ArrowRightIcon.Visibility = Visibility.Hidden;
            DeleteChat.ArrowRightIcon.Visibility = Visibility.Hidden;
        }

        public void SetBasicIcons()
        {
            OpenNewWindow.IconElement.Kind = PackIconKind.WindowRestore;
            Archive.IconElement.Kind = PackIconKind.Archive;
            Unpin.IconElement.Kind = PackIconKind.PinOffOutline;
            MuteNotifs.IconElement.Kind = PackIconKind.VolumeMute;
            MarkRead.IconElement.Kind = PackIconKind.MessageOutline;
            AddToFolder.IconElement.Kind = PackIconKind.FolderOutline;

            ClearChat.IconElement.Kind = PackIconKind.Broom;
            DeleteChat.IconElement.Kind = PackIconKind.BucketOutline;

        }

        private void AddSubMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            ToAddSubMenuType? type =
                sender == MuteNotifs ? ToAddSubMenuType.Notification :
                sender == AddToFolder ? ToAddSubMenuType.Folder : null;
            if (type is null) return;

            Point enteredElPoint = GetEnteredItemCord((ToAddSubMenuType)type);

            _window.AddSubMenu((ToAddSubMenuType)type, enteredElPoint);
        }

        public Point GetEnteredItemCord(ToAddSubMenuType type)
        {
            UserChatsMenuElement? el =
                type == ToAddSubMenuType.Notification ? MuteNotifs :
                type == ToAddSubMenuType.Folder ? AddToFolder : null;

            return el is null ? new Point(0, 0) :
                el.TransformToAncestor(this)
                           .Transform(new Point(0, 0));
        }

        private void SetMuteNotifsSubMenu()
        {
            MainPanel.Children.Clear();

            AddItemToMainPanel(PackIconKind.MusicNoteEighth, "Select Tone");
            AddItemToMainPanel(PackIconKind.MusicOff, "Disable Sound");
            AddItemToMainPanel(PackIconKind.VolumeOff, "Mute for");
            AddItemToMainPanel(PackIconKind.VolumeOff, "Mute forever");
        }

        private void SetAddToFolderSubMenu()
        {
            MainPanel.Children.Clear();
        }

        public void SetSubMenu(ToAddSubMenuType type)
        {
            switch (type)
            {
                case ToAddSubMenuType.Notification:
                    {
                        SetMuteNotifsSubMenu();
                        return;
                    }
                case ToAddSubMenuType.Folder:
                    {
                        SetAddToFolderSubMenu();
                        return;
                    }
            }
        }

        public void AddItemToMainPanel(PackIconKind kind, string text)
        {
            UserChatsMenuElement toAdd = new UserChatsMenuElement();

            toAdd.IconElement.Kind = kind;
            toAdd.TextElement.Text = text;
            toAdd.ArrowRightIcon.Visibility = Visibility.Hidden;

            MainPanel.Children.Add(toAdd);
        }

        private void UserChatsMenuElement_MouseEnter(object sender, MouseEventArgs e)
        {
            _window.ClearSubMenus();
        }

        private void UserChatsMenuElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UserChatsMenuElement but) return;

            //Get menu action
            UserTalkControlButTypes? type = GetButType(but);
            if (type is null) return;

            //Set menu Action

            _window.SetSubMenuAction((UserTalkControlButTypes)type);
        }

        public UserTalkControlButTypes? GetButType(UserChatsMenuElement but)
        {
            return
                but == OpenNewWindow ? UserTalkControlButTypes.OpenInNewWindow :
                but == Archive ? UserTalkControlButTypes.Archive :
                but == Unpin ? UserTalkControlButTypes.Unpin :
                but == MuteNotifs ? UserTalkControlButTypes.MuteNotifs :
                but == MarkRead ? UserTalkControlButTypes.MarkRead :
                but == AddToFolder ? UserTalkControlButTypes.AddToFolder :
                but == ClearChat ? UserTalkControlButTypes.ClearChat :
                but == DeleteChat ? UserTalkControlButTypes.DeleteChat : null;
        }
    }
}
