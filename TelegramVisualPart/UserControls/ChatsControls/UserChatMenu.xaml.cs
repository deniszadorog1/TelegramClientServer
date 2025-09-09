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

namespace TelegramVisualPart.UserControls.ChatsControls
{
    /// <summary>
    /// Логика взаимодействия для UserChatMenu.xaml
    /// </summary>
    public partial class UserChatMenu : UserControl
    {
        public UserChatMenu()
        {
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            SetBasicIcons();
            HideRightArrowIcons();
            SetBasicColors();
            SetBasicText();
        }

        public void SetBasicText()
        {
            OpenNewWindow.TextElement.Text = "Open in Window";
            Archive.TextElement.Text = "Archive";
            Unpin.TextElement.Text = "Unpin";
            MuteNotifs.TextElement.Text = "Mute notifications";
            MarkRead.TextElement.Text = "Mark as unread";
            AddToFolder.TextElement.Text = "Add to folder";
            LeaveChannel.TextElement.Text = "Leave channel";
        }

        public void SetBasicColors()
        {
            LeaveChannel.IconElement.Foreground = Brushes.Red;
            LeaveChannel.TextElement.Foreground = Brushes.Red;
        }

        public void HideRightArrowIcons()
        {
            OpenNewWindow.ArrowRightIcon.Visibility = Visibility.Hidden;
            Archive.ArrowRightIcon.Visibility = Visibility.Hidden;
            Unpin.ArrowRightIcon.Visibility = Visibility.Hidden;
            MarkRead.ArrowRightIcon.Visibility = Visibility.Hidden;
            LeaveChannel.ArrowRightIcon.Visibility = Visibility.Hidden;
        }

        public void SetBasicIcons()
        {
            OpenNewWindow.IconElement.Kind = PackIconKind.WindowRestore;
            Archive.IconElement.Kind = PackIconKind.Archive;
            Unpin.IconElement.Kind = PackIconKind.PinOffOutline;
            MuteNotifs.IconElement.Kind = PackIconKind.VolumeMute;
            MarkRead.IconElement.Kind = PackIconKind.MessageOutline;
            AddToFolder.IconElement.Kind = PackIconKind.FolderOutline;

            LeaveChannel.IconElement.Kind = PackIconKind.ExitToApp;

        }

        private void AddSubMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            ToAddSubMenuType? type =
                sender == MuteNotifs ? ToAddSubMenuType.Notification :
                sender == AddToFolder ? ToAddSubMenuType.Folder : null;
            if (type is null) return;


            Point enteredElPoint = GetEnteredItemCord((ToAddSubMenuType)type);

            ((MainWindow)Window.GetWindow(this)).AddSubMenu((ToAddSubMenuType)type, enteredElPoint);
        }

        public Point GetEnteredItemCord(ToAddSubMenuType type)
        {
            UserChatsMenuElement? el =
                type == ToAddSubMenuType.Notification ? MuteNotifs :
                type == ToAddSubMenuType.Folder ? AddToFolder : null;

            return el is null ? new Point(0,0) :
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

    }
}
