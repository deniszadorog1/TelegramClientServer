using MaterialDesignThemes.Wpf;
using Microsoft.IdentityModel.Tokens;
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
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.FolderObjs;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Enums.Menus;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.Services;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace TelegramVisualPart.UserControls.ChatsControls
{
    /// <summary>
    /// Логика взаимодействия для UserChatMenu.xaml
    /// </summary>
    public partial class UserChatMenu : UserControl
    {
        TelegramLib.MainClasses.UserChat _chat = null;
        private TelSystem _system;

        public UserChatMenu(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetBasicParams();
        }

        public UserChatMenu(TelegramLib.MainClasses.UserChat chat, TelSystem system)
        {
            _system = system;
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
            SetBasicColors();
            SetBasicText();
            HideRightArrowIcons();

            SetAddSubMenuEventsToElems();

            SetChangeableIcons();
        }

        public void SetChangeableIcons()
        {
            if (_chat is null) return;

            Unpin.IconElement.Kind = _chat.IsPinned ?
                PackIconKind.PinOffOutline : PackIconKind.PinOutline;
            Unpin.TextElement.Text = _chat.IsPinned ?
                VisConstParamsJsonService.GetStringByName("ToUnpin") : 
                VisConstParamsJsonService.GetStringByName("ToPin");

            MarkRead.IconElement.Kind = _chat.IsMarked ?
                PackIconKind.ChatOutline : PackIconKind.ChatAlertOutline;
            MarkRead.TextElement.Text = _chat.IsMarked ?
                VisConstParamsJsonService.GetStringByName("ToMarkRead") :
                VisConstParamsJsonService.GetStringByName("ToMarkUnRead");
        }

        public void SetAddSubMenuEventsToElems()
        {
            MuteNotifs.MouseEnter += ToAddSubMenu_MouseEnter;
            MuteNotifs.PreviewMouseDown += UserChatsMenuElement_PreviewMouseDown;

            AddToFolder.MouseEnter += ToAddSubMenu_MouseEnter;
            AddToFolder.PreviewMouseDown += UserChatsMenuElement_PreviewMouseDown;

            Archive.PreviewMouseDown += Archive_PreviewMouseDown;
        }

        public void Archive_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            _system.Settings.SoundNotifSettings.ToMirrorArchive();
        }


        public void ToAddSubMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender == MuteNotifs && _system.Settings.SoundNotifSettings.IsForeverMuted()) return;
            UserChatsMenuElement_MouseEnter(sender, e);
            AddSubMenu_MouseEnter(sender, e);
        }

        public void SetBasicText()
        {
            OpenNewWindow.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToOpenNewWindow");

            Archive.TextElement.Text = _system.Settings.SoundNotifSettings.IsArchived ?
                VisConstParamsJsonService.GetStringByName("ToUnArchive") : 
                VisConstParamsJsonService.GetStringByName("ToArchive");

            Unpin.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToUnpin");
            MuteNotifs.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToMuteNotifs");
            MarkRead.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToMarkUnRead");
            AddToFolder.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToAddToFolder");
            ClearChat.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToClearChat");
            DeleteChat.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToDeleteChat");
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

            if (_system is not null && 
                _system.Settings.SoundNotifSettings.IsForeverMuted())
            {
                MuteNotifs.ArrowRightIcon.Visibility = Visibility.Hidden;
                MuteNotifs.TextElement.Text = VisConstParamsJsonService.GetStringByName("ToUnMuteNotifs"); 
            }

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


        private string _setToneTag = VisConstParamsJsonService.GetStringByName("SetTone");
        private string _disableSoundTag = VisConstParamsJsonService.GetStringByName("DisSound");
        private string _muteDurationTag = VisConstParamsJsonService.GetStringByName("MuteDur");
        private string _muteForeverTag = VisConstParamsJsonService.GetStringByName("MFor");
        private void SetMuteNotifsSubMenu()
        {
            MainPanel.Children.Clear();

            AddItemToMainPanel(PackIconKind.MusicNoteEighth,
                VisConstParamsJsonService.GetStringByName("SelectTone"), _setToneTag);
            AddItemToMainPanel(PackIconKind.MusicOff,
                VisConstParamsJsonService.GetStringByName("DisableSound"), _disableSoundTag);
            AddItemToMainPanel(PackIconKind.VolumeOff,
                VisConstParamsJsonService.GetStringByName("MuteFor"), _muteDurationTag);
            AddItemToMainPanel(PackIconKind.VolumeOff,
                VisConstParamsJsonService.GetStringByName("MuteForever"), _muteForeverTag);

            NotSubMenuMouseDownEvent();
        }

        public void NotSubMenuMouseDownEvent()
        {
            List<UserChatsMenuElement> els =
                MainPanel.Children.OfType<UserChatsMenuElement>().ToList();

            foreach (var el in els)
            {
                SetNotifSubMenuMouseDownEvents(el);
            }
        }

        public void PaintElement(SolidColorBrush color, 
            UserChatsMenuElement el)
        {
            el.IconElement.Foreground = color;
            el.TextElement.Foreground = color;
        }


        public void SetNotifSubMenuMouseDownEvents(UserChatsMenuElement item)
        {
            if (item.Tag.ToString() == _setToneTag)
            {
                item.PreviewMouseDown += AddChooseTonePage_PreviewMouseDown;

            }
            else if (item.Tag.ToString() == _disableSoundTag)
            {
                item.PreviewMouseDown += DisableSound_PreviewMouseDown;

                //Set base
                SetIsMusicEnStatus(item);
            }
            else if (item.Tag.ToString() == _muteDurationTag)
            {
                item.PreviewMouseDown += SetMuteDuration_PreviewMouseDown;
            }
            else if (item.Tag.ToString() == _muteForeverTag)
            {
                item.PreviewMouseDown += MuteForever_PreviewMouseDown;
                PaintElement(new SolidColorBrush(Colors.Red), item);
            }
        }

        public void SetMuteDuration_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow window = ((MainWindow)Window.GetWindow(this));

            if (window is null) window = _window;

            window.SetSecondaryFrame(
                 new Pages.LittleMenuPages.MuteDuration(_system));
        }

        public void MuteForever_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UserChatsMenuElement menuElem) return;
            _system.Settings.SoundNotifSettings.ToMirrorEnStatus();
            _system.Settings.SoundNotifSettings.ToMuteForever();
        }

        public void DisableSound_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UserChatsMenuElement menuElem) return;

            _system.Settings.SoundNotifSettings.ToMirrorEnStatus();

            SetIsMusicEnStatus(menuElem);
        }

        public void SetIsMusicEnStatus( UserChatsMenuElement menuElem)
        {
            bool isEnabled = _system.Settings.SoundNotifSettings.IsEnabled;

            menuElem.IconElement.Kind = isEnabled ? PackIconKind.MusicOff : PackIconKind.Music;
            menuElem.TextElement.Text = isEnabled ?
                VisConstParamsJsonService.GetStringByName("DisableSound") :
                VisConstParamsJsonService.GetStringByName("EnableSound");
        }

        public void AddChooseTonePage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow window = ((MainWindow)Window.GetWindow(this));
            if (window is null) window = _window;
            window.SetSecondaryFrame(
                 new Pages.LittleMenuPages.SelectSoundTone(_system));
        }

        private void SetAddToFolderSubMenu()
        {
            if (_chat is null) return;
            MainPanel.Children.Clear();

            //Folders elements
            for(int i = 0; i < _system.Folders.Count; i++)
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

            MainPanel.Children.Add(toAdd);
        }

        public void AddNewFolder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _window.SetSecondaryFrame(new FolderAction(_system));
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

            MainPanel.Children.Add(toAdd);
        }

        public void FolderElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not UserChatsMenuElement el) return;
            
            //Set add in folder
            if(el.IconElement.Kind == PackIconKind.None)
            {
                _system.AddContactToFolder(el.TextElement.Text, _chat.Chatter);
                return;
            }
            //Set remove from folder
            _system.RemoveContactFromFolder(el.TextElement.Text, _chat.Chatter);

        }

        public void SetSubMenu(ToAddSubMenuType type)
        {
            switch (type)
            {
                case ToAddSubMenuType.Notification:
                    {
                        if (_system.Settings.SoundNotifSettings.IsForeverMuted()) return;
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

        public void AddItemToMainPanel(PackIconKind kind, string text, string tag)
        {
            UserChatsMenuElement toAdd = new UserChatsMenuElement();

            toAdd.Tag = tag;
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
            if (_system.Settings.SoundNotifSettings.IsForeverMuted())
            {
                _system.Settings.SoundNotifSettings.MuteTime = null;
            }


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
